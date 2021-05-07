using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace GameApp.Network
{
    public class TcpServerBehaviour : MonoBehaviour
    {
        public int port = 9527;
        
        public bool listenOnAwake;

        public OnMessageEvent onMeesage = new OnMessageEvent();
        public OnEstablishedEvent onEstablished = new OnEstablishedEvent();
        public OnDisconnectedEvent onDisconnected = new OnDisconnectedEvent();

        public TcpServer Server => _server;

        public bool showHeartBeat;
        
        private TcpServer _server;

        public void Listen()
        {
            _server = new TcpServer(new IPEndPoint(IPAddress.Any, port), onMeesage, onEstablished, onDisconnected);
            var _ = _server.Listen();
        }

        public void BroadCast(object message)
        {
            _server.BroadcastToClients(MessageUtil.Build(message));
        }

        public void Send(TcpClient client, string message)
        {
            _server.SendMessageToClient(client, MessageUtil.Build(message));
        }
        
        protected virtual void Awake()
        {
            onMeesage.AddListener(MessageUtil.Dispatch);
        }

        protected virtual void OnEnable()
        {
            if (listenOnAwake) Listen();
            EventDispatcher.Instance.AddListener<object>(typeof(HeartBeatMessage).FullName, OnReceiveHeartBeatMessage);
        }

        protected virtual void OnDisable()
        {
            _server.Dispose();
            EventDispatcher.Instance.RemoveListener<object>(typeof(HeartBeatMessage).FullName, OnReceiveHeartBeatMessage);
        }

        private void OnReceiveHeartBeatMessage(object obj)
        {
            var command = (HeartBeatMessage) obj;
            if (showHeartBeat)
            {
                Debug.Log($"Stamp: {command.stamp}");
            }
        }
    }
}