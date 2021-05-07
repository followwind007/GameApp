using System.Collections.Generic;
using UnityEngine;

namespace GameApp.DynResolver
{
    public static class ResolveUtil
    {
        public static byte[] ToBytes(DynDescriptor descriptor, 
            Dictionary<string, DynHandler> fields, 
            Dictionary<string, DynArray> arrFields)
        {
            var l = new List<byte>();
            l.AddRange(VarintUtil.WriteRaw(descriptor.index));
            foreach (var h in fields.Values)
            {
                var bytes = h.Bytes;
                if (bytes == null) continue;
                var f = h.field;
                l.AddRange(VarintUtil.WriteRaw(f.index));
                if (f.HasLength || descriptor.CombineLength) 
                    l.AddRange(VarintUtil.WriteRaw(bytes.Length));
                l.AddRange(bytes);
            }

            foreach (var kv in arrFields)
            {
                var f = descriptor.GetField(kv.Key);
                var arr = kv.Value.list;
                l.AddRange(VarintUtil.WriteRaw(f.index));
                
                if (descriptor.CombineLength)
                {
                    var arrData = new List<byte>();
                    arrData.AddRange(VarintUtil.WriteRaw(arr.Count));
                    foreach (var h in arr)
                    {
                        var bytes = h.Bytes;
                        if (f.HasLength) arrData.AddRange(VarintUtil.WriteRaw(bytes.Length));
                        arrData.AddRange(bytes);
                    }
                    l.AddRange(VarintUtil.WriteRaw(arrData.Count));
                    l.AddRange(arrData);
                }
                else
                {
                    l.AddRange(VarintUtil.WriteRaw(arr.Count));
                    foreach (var h in arr)
                    {
                        var bytes = h.Bytes;
                        if (f.HasLength) l.AddRange(VarintUtil.WriteRaw(bytes.Length));
                        l.AddRange(bytes);
                    }
                }
            }
            return l.ToArray();
        }
        
        public static void ResolveData(byte[] data, out DynDescriptor descriptor, 
            Dictionary<string, DynHandler> fields, 
            Dictionary<string, DynArray> arrFields)
        {
            var cur = 0;
            var descIndex = VarintUtil.GetValue(data, out var descLen);
            cur += descLen;
            
            descriptor = Dyn.GetDescriptor(descIndex);

            if (descriptor == null)
            {
                Debug.LogError($"can not resolve descriptor {descIndex}");
                return;
            }

            switch (descriptor.mode)
            {
                case DescriptorMode.Default:
                    ResolveDataDefault(data, descriptor, cur, fields, arrFields);
                    break;
                case DescriptorMode.Strict:
                    ResolveDataStrict(data, descriptor, cur, fields, arrFields);
                    break;
            }
        }

        private static void ResolveDataStrict(byte[] data, DynDescriptor descriptor, int cur,
            Dictionary<string, DynHandler> fields, Dictionary<string, DynArray> arrFields)
        {
            var dataLen = data.Length;
            while (cur < dataLen)
            {
                var fieldIndex = VarintUtil.GetValue(data, out var fieldLen, cur);
                cur += fieldLen;
                var field = descriptor.GetField(fieldIndex);
                
                if (field == null)
                {
                    Debug.LogError($"can not resolve field {fieldIndex} in {descriptor.name}");
                    break;
                }

                if (field.desc == DynDesc.Single)
                {
                    if (field.IsVarint)
                    {
                        VarintUtil.GetValue(data, out var vLen, cur);
                        fields[field.name] = new DynHandler(field, ByteUtil.GetSub(data, cur, vLen));
                        cur += vLen;
                    }
                    else if (field.type == DynType.Float)
                    {
                        fields[field.name] = new DynHandler(field, ByteUtil.GetSub(data, cur, Dyn.FloatLength));
                        cur += Dyn.FloatLength;
                    }
                    else if (field.HasLength)
                    {
                        var fLen = VarintUtil.GetValue(data, out var vLen, cur);
                        cur += vLen;
                        fields[field.name] = new DynHandler(field, ByteUtil.GetSub(data, cur, fLen));
                        cur += fLen;
                    }
                }
                else if (field.desc == DynDesc.Array)
                {
                    arrFields[field.name] = ResolveArrayData(data, ref cur, field);
                }
            }
        }
        
        private static void ResolveDataDefault(byte[] data, DynDescriptor descriptor, int cur,
            Dictionary<string, DynHandler> fields, Dictionary<string, DynArray> arrFields)
        {
            var dataLen = data.Length;
            while (cur < dataLen)
            {
                var fieldIndex = VarintUtil.GetValue(data, out var fieldLen, cur);
                cur += fieldLen;
                var field = descriptor.GetField(fieldIndex);

                var valueLen = VarintUtil.GetValue(data, out var vLen, cur);
                cur += vLen;
                if (field == null)
                {
                    cur += valueLen;
                    continue;
                }

                if (field.desc == DynDesc.Single)
                {
                    fields[field.name] = new DynHandler(field, ByteUtil.GetSub(data, cur, valueLen));
                    cur += valueLen;
                }
                else if (field.desc == DynDesc.Array)
                {
                    arrFields[field.name] = ResolveArrayData(data, ref cur, field);
                }
            }
        }

        private static DynArray ResolveArrayData(byte[] data, ref int cur, DynField field)
        {
            var arrLen = VarintUtil.GetValue(data, out var vLen, cur);
            cur += vLen;
            var arr = new List<DynHandler>(arrLen);
                    
            if (field.IsVarint)
            {
                for (var i = 0; i < arrLen; i++)
                {
                    VarintUtil.GetValue(data, out vLen, cur);
                    arr.Add(new DynHandler(field, ByteUtil.GetSub(data, cur, vLen)));
                    cur += vLen;
                }
            }
            else if (field.type == DynType.Float)
            {
                for (var i = 0; i < arrLen; i++)
                {
                    arr.Add(new DynHandler(field, ByteUtil.GetSub(data, cur, Dyn.FloatLength)));
                    cur += Dyn.FloatLength;
                }
            }
            else if (field.HasLength)
            {
                for (var i = 0; i < arrLen; i++)
                {
                    var fLen = VarintUtil.GetValue(data, out vLen, cur);
                    cur += vLen;
                    arr.Add(new DynHandler(field, ByteUtil.GetSub(data, cur, fLen)));
                    cur += fLen;
                }
            }

            return new DynArray(field, arr);
        }

    }
}