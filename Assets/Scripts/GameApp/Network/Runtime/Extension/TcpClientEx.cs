using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameApp.Network
{
    public static class TcpClientEx
    {
        public static async Task StartReadAsync(this TcpClient client, Action<byte[]> onReceive)
        {
            await client.StartReadAsync(onReceive, CancellationToken.None);
        }
        
        public static async Task StartReadAsync(this TcpClient client, Action<byte[]> onReceive, CancellationToken token)
        {
            while (client.Connected && !token.IsCancellationRequested)
            {
                try
                {
                    var stream = client.GetStream();
                    var msg = await stream.ReadMessageAsync(token);
                    if (msg != null)
                    {
                        onReceive?.Invoke(msg);
                    }
                    else
                    {
                        client.Dispose();
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Receive message failed", e);
                }
            }
        }
        
    }
}