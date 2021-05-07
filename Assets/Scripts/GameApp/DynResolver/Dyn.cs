using System.Collections.Generic;

namespace GameApp.DynResolver
{
    public static class Dyn
    {
        public const int FloatLength = 4;

        private static Dictionary<string, int> _nameIndexMap = new Dictionary<string, int>();
        private static Dictionary<int, string> _descs = new Dictionary<int, string>(); 
        
        private static Dictionary<int, DynDescriptor> _descIndexMap = new Dictionary<int, DynDescriptor>();

        public static void RegisterDescriptor(string name, short index, string desc)
        {
            _nameIndexMap[name] = index;
            _descs[index] = desc;
        }

        public static void AddDescritor(DynDescriptor descriptor)
        {
            _descIndexMap[descriptor.index] = descriptor;
            _nameIndexMap[descriptor.name] = descriptor.index;
        }
        
        public static DynDescriptor GetDescriptor(string name)
        {
            return _nameIndexMap.ContainsKey(name) ? GetDescriptor(_nameIndexMap[name]) : null;
        }

        public static DynDescriptor GetDescriptor(int index)
        {
            if (!_descIndexMap.ContainsKey(index))
            {
                if (_descs.ContainsKey(index))
                {
                    DescriptorUtil.Parse(_descs[index]);
                }
            }

            _descIndexMap.TryGetValue(index, out var desc);
            return desc;
        }
    }
}