using System.Net.Sockets;
using System.Threading;
using GameApp.Util;

namespace GameApp.Network
{
    public class ClientConnection
    {
        public readonly TcpClient client;
        public int lastStamp;
        public readonly CancellationTokenSource readToken;

        public ClientConnection(TcpClient c)
        {
            client = c;
            lastStamp = Utils.GetTimeStamp();
            readToken = new CancellationTokenSource();
        }
    }
}