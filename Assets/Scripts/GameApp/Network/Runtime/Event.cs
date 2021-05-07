using System.Net;
using System.Net.Sockets;
using GameApp.Serialize;
using UnityEngine.Events;

namespace GameApp.Network
{
    [System.Serializable]
    public class OnMessageEvent : UnityEvent<SerializedJsonObject>
    {
    }

    [System.Serializable]
    public class OnEstablishedEvent : UnityEvent<TcpClient>
    {
    }

    [System.Serializable]
    public class OnDisconnectedEvent : UnityEvent<EndPoint>
    {
    }
}