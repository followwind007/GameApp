using System;
using System.Collections.Generic;
using System.Text;

namespace GameApp.DynResolver
{
    public class DynHandler
    {
        private class WrongTypeException : Exception { }
        
        public readonly DynField field;
        private byte[] _bytes;

        private DynObject _dynObject;
        public byte[] Bytes => _dynObject != null ? _dynObject.ToBytes() : _bytes;

        public int Int
        {
            get
            {
                if (!field.IsVarint) throw new WrongTypeException();
                if (_bytes == null) return default;
                return VarintUtil.GetValue(_bytes);
            }
            set
            {
                if (!field.IsVarint) throw new WrongTypeException();
                _bytes = VarintUtil.WriteRawZig(value);
            }
        }

        public float Float
        {
            get
            {
                if (field.type != DynType.Float) throw new WrongTypeException();
                if (_bytes == null) return default;
                return Convert.ToSingle(_bytes);
            }
            set
            {
                if (field.type != DynType.Float) throw new WrongTypeException();
                _bytes = BitConverter.GetBytes(value);
            }
        }

        public string String
        {
            get
            {
                if (field.type != DynType.String) throw new WrongTypeException();
                if (_bytes == null) return default;
                return Convert.ToString(_bytes);
            }
            set
            {
                if (field.type != DynType.String) throw new WrongTypeException();
                _bytes = Encoding.UTF8.GetBytes(value);
            }
        }

        public DynObject Object
        {
            get
            {
                if (field.type != DynType.Struct) throw new WrongTypeException();

                if (_dynObject == null)
                {
                    _dynObject = _bytes == null ? new DynObject(field.descriptor) : new DynObject(_bytes);
                }
                
                return _dynObject;
            }
            set
            {
                if (field.type != DynType.Struct) throw new WrongTypeException();
                _dynObject = value;
            }
        }

        public DynHandler(DynField field, byte[] bytes = null)
        {
            this.field = field;
            _bytes = bytes;
        }
        
    }

    public class DynArray
    {
        public readonly DynField field;
        public readonly List<DynHandler> list;
        public DynArray(DynField field, List<DynHandler> list = null)
        {
            this.field = field;
            this.list = list ?? new List<DynHandler>();
        }

        public DynHandler Add()
        {
            list.Add(new DynHandler(field));
            return list[list.Count - 1];
        }
    }
    
    public class DynObject
    {
        private DynDescriptor _descriptor;

        private Dictionary<string, DynHandler> _fields;
        private Dictionary<string, DynArray> _arrFields;

        public DynObject(string descriptorName) : this(Dyn.GetDescriptor(descriptorName)) { }

        public DynObject(byte[] data) : this(null, data) { }
        
        public DynObject(DynDescriptor descriptor, byte[] data = null)
        {
            _fields = new Dictionary<string, DynHandler>();
            _arrFields = new Dictionary<string, DynArray>();
            _descriptor = descriptor;
            if (data != null)
            {
                ResolveUtil.ResolveData(data, out _descriptor, _fields, _arrFields);
            }
        }

        public DynHandler GetField(string field)
        {
            var f = _descriptor.GetField(field);
            if (f == null) throw new Exception($"Try to get not invalid field {field}");
            if (f.IsArray) throw new Exception($"Field {field} is array, try to use GetList()");
            
            if (!_fields.ContainsKey(field))
            {
                _fields[field] = new DynHandler(f);
            }
            
            return _fields[field];
        }

        public DynArray GetList(string field)
        {
            var f = _descriptor.GetField(field);
            if (f == null) throw new Exception($"Try to get not invalid field {field}");
            if (!f.IsArray) throw new Exception($"Field {field} is not array, try to use GetField()");

            if (!_arrFields.ContainsKey(field))
            {
                _arrFields[field] = new DynArray(f);
            }
            
            return _arrFields[field];
        }

        public void RemoveField(string field)
        {
            if (_fields.ContainsKey(field))
            {
                _fields.Remove(field);
            }
            else if (_arrFields.ContainsKey(field))
            {
                _arrFields.Remove(field);
            }
        }

        public byte[] ToBytes()
        {
            return ResolveUtil.ToBytes(_descriptor, _fields, _arrFields);
        }

    }
}