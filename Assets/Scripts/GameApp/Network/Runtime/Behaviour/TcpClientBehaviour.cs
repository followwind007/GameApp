using System.Threading.Tasks;
using GameApp.Util;
using UnityEngine;

namespace GameApp.Network
{
    public class TcpClientBehaviour : MonoBehaviour
    {
        public string serverIp = "127.0.0.1";
        public int serverPort = 9527;
        
        public bool connectOnAwake;

        public OnMessageEvent onMessageEvent = new OnMessageEvent();

        public TcpCommunicator client;

        public void Send(object obj)
        {
            client?.Send(MessageUtil.Build(obj));
        }

        public void Connect()
        {
            client = new TcpCommunicator(serverIp, serverPort, onMessageEvent);
            Task.Run(() => { var _ = client.Listen(); });
        }

        public void Disconnect()
        {
            client?.Dispose();
            client = null;
        }
        
        protected virtual void Awake()
        {
            onMessageEvent.AddListener(MessageUtil.Dispatch);
            Timer.Add(1, SendHeartBeatMessage, -1, true);
        }

        protected virtual void OnEnable()
        {
            if (connectOnAwake) Connect();
        }

        protected virtual void OnDisable()
        {
            Disconnect();
        }

        protected void SendHeartBeatMessage()
        {
            if (client != null && client.IsRunning)
            {
                var msg = new HeartBeatMessage
                {
                    stamp = Utils.GetTimeStamp()
                };
                Send(msg);
            }
        }
        
    }
}