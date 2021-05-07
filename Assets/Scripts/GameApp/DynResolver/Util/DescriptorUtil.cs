
/*

struct MessageA 0
{
    int field0 0 //single field
    array int field1 1 //array field
    float field2 2
    string field3 3
}

struct MessageB 1
{
    int field0 0
    MessageA field1 1
}

*/


namespace GameApp.DynResolver
{
    public static class DescriptorUtil
    {
        private static DynParser _parser;
        public static void Parse(string content)
        {
            if (_parser == null) _parser = new DynParser();
            var ds = _parser.Parse(content);
            _parser.Dispose();
            foreach (var d in ds)
            {
                Dyn.AddDescritor(d);
            }
        }
        
    }
}