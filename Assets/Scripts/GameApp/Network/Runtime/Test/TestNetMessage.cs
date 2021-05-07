using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace GameApp.Network
{
    public class TestNetMessage : MonoBehaviour
    {
        public TcpServerBehaviour tcpServer;
        public TcpClientBehaviour tcpClient;

        private readonly List<string> _list = new List<string>();

        public string text;

        public void OnMessage(string message)
        {
            var c = MessageUtil.GetMessageContent(message);
            _list.Add(c.ToString());

            Debug.Log($"<color=cyan>Received</color> : {c}");
        }

        public void OnEstablished(TcpClient client)
        {
            var endpoint = client.Client.RemoteEndPoint;
            Debug.Log($"<color=yellow>Established</color> : {endpoint}");

            tcpServer.Send(client, "Established");
        }

        public void OnDisconnected(EndPoint endpoint)
        {
            Debug.Log($"<color=red>Disconnected</color> : {endpoint}");
        }

        private void Awake()
        {
            tcpServer.gameObject.SetActive(true);
            tcpClient.gameObject.SetActive(true);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Client Send", GUILayout.Width(150), GUILayout.Height(50)))
            {
                tcpClient.Send(text);
            }

            if (GUILayout.Button("Server Broadcast", GUILayout.Width(150), GUILayout.Height(50)))
            {
                tcpServer.BroadCast(text);
            }

            if (tcpServer.Server != null)
            {
                GUILayout.Label($"Clients: {tcpServer.Server.Connections.Count}");
            }

            GUILayout.EndHorizontal();

            using(new GUILayout.VerticalScope())
            {
                _list.ForEach(elem =>
                {
                    GUILayout.Label(elem);
                });
            }
        }
        
    }
}