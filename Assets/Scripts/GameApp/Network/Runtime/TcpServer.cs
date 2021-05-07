using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GameApp.Util;
using Timer = System.Threading.Timer;

namespace GameApp.Network
{
    public class TcpServer : IDisposable
    {
        private const float ExpireDuration = 30f;
        private const int ExpireCheckPeriod = 1000;
        
        private readonly IPEndPoint _endpoint;

        private TcpListener _listener;

        private readonly SynchronizationContext _mainContext;
        private volatile bool _acceptLoop = true;
        
        public List<ClientConnection> Connections { get; } = new List<ClientConnection>();

        private readonly OnMessageEvent _onMessage;
        private readonly OnEstablishedEvent _onEstablished;
        private readonly OnDisconnectedEvent _onDisconnected;

        private readonly List<ClientConnection> _expired = new List<ClientConnection>();
        private readonly Timer _checkExpireTimer;

        public TcpServer(IPEndPoint endpoint, OnMessageEvent onMessage)
        {
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            // Set Unity main thread
            _mainContext = SynchronizationContext.Current;

            _onMessage = onMessage;
        }

        public TcpServer(IPEndPoint endpoint, OnMessageEvent onMessage, OnEstablishedEvent onEstablished, OnDisconnectedEvent onDisconnected) : 
            this(endpoint, onMessage)
        {
            _onEstablished = onEstablished;
            _onDisconnected = onDisconnected;
            
            _checkExpireTimer = new Timer(CheckConnections, null, ExpireCheckPeriod, ExpireCheckPeriod);
        }
        
        public async Task Listen()
        {
            lock (this)
            {
                if (_listener != null) throw new InvalidOperationException("Already started");

                _acceptLoop = true;
                _listener = new TcpListener(_endpoint);
            }

            _listener.Start();

            while (_acceptLoop)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    var _ = Task.Run(() => OnConnectClient(client));
                }
                catch (ObjectDisposedException) { }
                catch (SocketException) { }
            }
        }

        public void BroadcastToClients(byte[] data)
        {
            Connections.ForEach(c => { SendMessageToClient(c.client, data); });
        }

        public void SendMessageToClient(TcpClient c, byte[] data)
        {
            if (c.Connected)
            {
                try
                {
                    var s = c.GetStream();
                    var _ = s.WriteAsync(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Attempt to send failed.", e);
                }
            }
        }
        
        public void Dispose()
        {
            _checkExpireTimer.Dispose();
            Stop();
        }
        
        private void Stop()
        {
            lock (this)
            {
                if (_listener == null) throw new InvalidOperationException("Not started");

                _acceptLoop = false;
                
                _listener.Stop();
                _listener = null;
            }

            var ts = new List<TcpClient>(Connections.Count);
            Connections.ForEach(c => { ts.Add(c.client); });
            ts.ForEach(c => c.Close());
        }

        private async Task OnConnectClient(TcpClient client)
        {
            var clientEndpoint = client.Client.RemoteEndPoint;
            
            _mainContext.Post(_ => { _onEstablished?.Invoke(client); }, null);
            var conn = new ClientConnection(client);
            Connections.Add(conn);

            try
            {
                await client.StartReadAsync(msg =>
                {
                    _mainContext.Post(_ =>
                    {
                        var c = MessageUtil.GetSerialized(msg);
                        if (c.T == typeof(HeartBeatMessage))
                        {
                            conn.lastStamp = Utils.GetTimeStamp();
                        }
                        _onMessage.Invoke(c);
                    }, null);
                }, conn.readToken.Token);
            }
            catch (Exception e)
            {
                OnReadDone(conn, clientEndpoint);
                throw new ApplicationException("Read exception", e);
            }
            
            OnReadDone(conn, clientEndpoint);
        }

        private void CheckConnections(object obj)
        {
            _expired.Clear();
            var cur = Utils.GetTimeStamp();
            Connections.ForEach(c => { if (cur - c.lastStamp > ExpireDuration) _expired.Add(c); });
            _expired.ForEach(c =>
            {
                c.client.Close();
                c.readToken.Cancel();
            });
        }

        private void OnReadDone(ClientConnection conn, EndPoint clientEndpoint)
        {
            _mainContext.Post(_ => { _onDisconnected?.Invoke(clientEndpoint); }, null);
            Connections.Remove(conn);
        }
        
    }
}