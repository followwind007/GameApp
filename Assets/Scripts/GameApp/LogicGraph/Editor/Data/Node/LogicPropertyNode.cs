namespace GameApp.LogicGraph
{
    public class LogicPropertyNode : LogicNode
    {
        public const string NameTarget = "target";
        public const string NameField = "field";
        public const string NameValue = "value";
        public const string NameReturn = "return_0";

        public const string FieldNone = "none";

        //for deserialize from json 
        public LogicPropertyNode() { }

        public LogicPropertyNode(string classId, string functionId):base(classId, functionId) { }

        private string TypeElua => LogicGraphSettings.Instance.luaTypes.typeElua;
        
        public override void OnGraphResolved()
        {
            var target = GetPortSlot(NameTarget);
            var value = GetPortSlot(NameValue);
            var return0 = GetPortSlot(NameReturn);
            
            if (linkNodes.TryGetValue(target, out var ln) && ln.Count > 0)
            {
                var field = GetPortSlot(NameField);

                var lt = ln[0];
                if (field.Value is string fieldVal && fieldVal != FieldNone && lt.Cls.Properties.TryGetValue(fieldVal, out var p))
                {
                    
                    if (value != null)
                    {
                        value.ValType = p.typeName;
                    }
                    
                    if (return0 != null)
                    {
                        return0.ValType = p.typeName;
                    }
                }
            }
            else
            {
                if (value != null)
                {
                    value.ValType = TypeElua;
                }

                if (return0 != null)
                {
                    return0.ValType = TypeElua;
                }
            }
        }
        
    }
}