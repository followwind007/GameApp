using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApp.Serialize;
using UnityEngine;

namespace GameApp.Network
{
    public static class MessageUtil
    {
        public const short HeadLength = 4;

        public static byte[] Build(object obj)
        {
            var serialized = new SerializedJsonObject(obj);
            var sj = JsonUtility.ToJson(serialized);
            var ct = Encoding.UTF8.GetBytes(sj);
            return Composite(ct);
        }

        private static byte[] Composite(byte[] data)
        {
            var len = BitConverter.GetBytes(data.Length + HeadLength);
            return len.Concat(data).ToArray();
        }

        public static byte[] Decomposite(byte[] data)
        {
            var ct = new byte[data.Length - HeadLength];
            Array.Copy(data, HeadLength, ct, 0, ct.Length);
            return ct;
        }

        public static int GetLength(IEnumerable<byte> data)
        {
            var len = data.Take(HeadLength).ToArray();
            return BitConverter.ToInt32(len, 0);
        }

        public static string GetString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public static SerializedJsonObject GetSerialized(byte[] data)
        {
            return JsonUtility.FromJson<SerializedJsonObject>(GetString(data));
        }

        public static object GetMessageContent(string msg)
        {
            var serialized = JsonUtility.FromJson<SerializedJsonObject>(msg);
            return serialized.GetData();
        }

        public static void Dispatch(SerializedJsonObject serialized)
        {
            EventDispatcher.Instance.Dispatch(serialized.type, serialized.GetData());
        }
    }
}