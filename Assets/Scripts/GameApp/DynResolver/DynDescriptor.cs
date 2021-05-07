using System;
using System.Collections.Generic;

namespace GameApp.DynResolver
{
    public enum DynType
    {
        Int,
        Float,
        String,
        Struct
    }

    public enum DynDesc
    {
        Single,
        Array
    }
    
    public class DynField
    {
        public readonly string name;
        public readonly string descriptor;
        public readonly int index;
        
        public readonly DynType type;
        public readonly DynDesc desc;

        public bool IsVarint => type == DynType.Int;
        public bool IsArray => desc == DynDesc.Array;
        
        public bool HasLength => type == DynType.String || type == DynType.Struct;

        public DynField(string descriptor, string name, int index, bool isArray = false)
        {
            this.descriptor = descriptor;
            this.name = name;
            this.index = index;
            if (!Enum.TryParse(descriptor, true, out type)) type = DynType.Struct;
            desc = isArray ? DynDesc.Array : DynDesc.Single;
        }

        public override string ToString()
        {
            return $"{descriptor} {index} {type} {desc}";
        }
    }

    public enum DescriptorMode
    {
        Default,
        Strict
    }
    
    public class DynDescriptor
    {
        public readonly string name;
        public readonly int index;
        public readonly DescriptorMode mode;

        public bool CombineLength => mode != DescriptorMode.Strict;

        private readonly Dictionary<int, DynField> _indexFieldMap = new Dictionary<int, DynField>();
        private readonly Dictionary<string, DynField> _nameFieldMap = new Dictionary<string, DynField>();

        public DynDescriptor(string name, int index, string mode)
        {
            this.name = name;
            this.index = index;
            if (!Enum.TryParse(mode, true, out this.mode)) this.mode = DescriptorMode.Default;
        }
        
        public void AddField(DynField dynField)
        {
            _indexFieldMap[dynField.index] = dynField;
            _nameFieldMap[dynField.name] = dynField;
        }

        public DynField GetField(int fieldIndex)
        {
            _indexFieldMap.TryGetValue(fieldIndex, out var field);
            return field;
        }
        
        public DynField GetField(string fieldName)
        {
            _nameFieldMap.TryGetValue(fieldName, out var field);
            return field;
        }
    }
}