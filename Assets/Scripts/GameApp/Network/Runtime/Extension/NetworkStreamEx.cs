using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GameApp.Network
{
    public static class NetworkStreamEx
    {
        private const short HLen = MessageUtil.HeadLength;
        
        public static async Task<byte[]> ReadMessageAsync(this NetworkStream stream)
        {
            return await stream.ReadMessageAsync(CancellationToken.None);
        }
        
        public static async Task<byte[]> ReadMessageAsync(this NetworkStream stream, CancellationToken token)
        {
            var head = await stream.ReadMessageAsync(HLen, token);
            
            var len = MessageUtil.GetLength(head);
            var msgLen = len - HLen;

            var msg = await stream.ReadMessageAsync(msgLen, token);

            return msg;
        }

        public static async Task<byte[]> ReadMessageAsync(this NetworkStream stream, int length)
        {
            return await ReadMessageAsync(stream, length, CancellationToken.None);
        }
        
        public static async Task<byte[]> ReadMessageAsync(this NetworkStream stream, int length, CancellationToken token)
        {
            var msg = new byte[length];

            var cur = 0;

            while (cur < length && !token.IsCancellationRequested)
            {
                cur += await stream.ReadAsync(msg, 0, length - cur, token);
            }

            return msg;
        }

    }
}