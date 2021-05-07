using System;
using System.Text;

namespace GameApp.DynResolver
{
    public static class ByteUtil
    {
        public static short GetShort(byte[] bytes, int start)
        {
            var sub = new byte[2];
            Array.Copy(bytes, start, sub, 0, 2);
            return Convert.ToInt16(sub);
        }

        public static byte[] GetSub(byte[] bytes, int start, int count)
        {
            var sub = new byte[count];
            Array.Copy(bytes, start, sub, 0, count);
            return sub;
        }

        public static string TransToString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append($"{b} ");
            }
            return sb.ToString();
        }

    }
}