
using System;

namespace GameApp.DynResolver
{
    /// <summary>
    /// varint编码
    /// </summary>
    public class VarintUtil
    {
        public static byte[] WriteRawZig(int value)
        {
            value = (value << 1) ^ (value >> 31);
            return WriteRaw(value);
        }
        
        public static int GetValueZig(byte[] buffer)
        {
            var value = GetValue(buffer);
            return (value >> 1) ^ -(value & 1);
        }
        
        /// <summary>
        /// 把value采用varint编码写入outBuffer
        /// </summary>
        /// <param name="value"></param>
        public static byte[] WriteRaw(int value)
        {
            // 数据长度
            var varIntBuffer = new byte[5];
            var index = 0;
            while (true)
            {
                if ((value & ~0x7f) == 0)
                {
                    varIntBuffer[index] = (byte)(value & 0x7f);
                    break;
                }
                else
                {
                    varIntBuffer[index] = (byte)((value & 0x7f) | 0x80);
                    value = value >> 7;
                }
                index++;
            }

            var bytes = new byte[index + 1];
            Array.Copy(varIntBuffer, bytes, index + 1);
            
            return bytes;
        }

        /// <summary>
        /// 计算value实际长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ComputeRawSize(int value)
        {
            if ((value & (0xffffffff << 7)) == 0)
            {
                return 1;
            }
            if ((value & (0xffffffff << 14)) == 0)
            {
                return 2;
            }
            if ((value & (0xffffffff << 21)) == 0)
            {
                return 3;
            }
            if ((value & (0xffffffff << 28)) == 0)
            {
                return 4;
            }
            return 5;
        }

        /// <summary>
        /// 某个byte是否是varint编码的结束字节
        /// </summary>
        public static bool IsEnd(byte value)
        {
            return (value & 0x80) == 0;
        }

        /// <summary>
        /// 将byte[]转化为对应的int
        /// </summary>
        public static int GetValue(byte[] buffer, out int length, int start = 0)
        {
            var result = 0;
            int i;
            for(i = start; i < buffer.Length; i++)
            {
                var value = buffer[i];
                result = result | (value & 0x7f) << (7 * (i - start));
                if ((value & 0x80) == 0)
                {
                    break;
                }
            }

            length = i - start + 1;
            return result;
        }

        public static int GetValue(byte[] buffer)
        {
            return GetValue(buffer, out _);
        }
        
        public void VarintTest(int value)
        {
            var varLength = ComputeRawSize(value);
            UnityEngine.Debug.Log($"value length:{varLength} value:{value}");
            var lengthBuffer = WriteRaw(value);

            var str = "";
            foreach (var t in lengthBuffer)
            {
                str += t;
            }
            var lengthBuffer2 = new byte[5];
            var index = 0;
            while (true)
            {
                var b = lengthBuffer[index];
                lengthBuffer2[index] = b;
                index++;
                if (IsEnd(b))
                {
                    break;
                }
            }
            var length = GetValue(lengthBuffer2, out _);
            UnityEngine.Debug.Log($"receive length:{length} bytes:{str}");
        }

    }
}