using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameApp.Network
{
    public class TcpCommunicator : IDisposable
    {
        public string Name => Socket.RemoteEndPoint.ToString();
        public bool IsConnecting { 
            get 
            {
                try
                {
                    if (TcpClient == null || !TcpClient.Connected) return false;
                    if (Socket == null) return false;

                    return !(Socket.Poll(1, SelectMode.SelectRead) && Socket.Available <= 0);
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsRunning => _running;

        private TcpClient TcpClient { get; }
        private Socket Socket => TcpClient?.Client;

        private readonly SynchronizationContext _mainContext;
        private readonly OnMessageEvent _onMessage;

        private bool _running;

        public TcpCommunicator(TcpClient tcpClient, OnMessageEvent onMessage)
        {
            TcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));

            _mainContext = SynchronizationContext.Current;
            _onMessage = onMessage;
        }

        public TcpCommunicator(string host, int port, OnMessageEvent onMessage) : this(new TcpClient(host, port), onMessage) { }

        public void Dispose()
        {
            if (TcpClient != null)
            {
                _running = false;

                TcpClient.Close();
                (TcpClient as IDisposable).Dispose();
            }
        }

        public void Send(byte[] data)
        {
            if (data == null) return;
            if (!IsConnecting) return;

            try
            {
                var s = TcpClient.GetStream();
                var _ = s.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Attempt to send failed.", ex);
            }
        }
        
        public async Task Listen()
        {
            if (TcpClient == null) return;
            
            _running = true;

            while (_running && IsConnecting)
            {
                await TcpClient.StartReadAsync(msg =>
                {
                    _mainContext.Post(_ => _onMessage.Invoke(MessageUtil.GetSerialized(msg)), null);
                }, CancellationToken.None);
            }
        }

    }
}