using System;
using System.Collections.Generic;
using GameApp.DataBinder;
using GameApp.LuaResolver;
using UnityEditor.Experimental.GraphView;

namespace GameApp.LogicGraph
{
    [Serializable]
    public class LogicVariableNode : LogicNode
    {
        public string propertyName;

        //for deserialize from json 
        public LogicVariableNode() { }

        public static string VariableClass => LogicGraphSettings.LuaClass.Variable;

        public LogicVariableNode(string path, string propType, string propName) : base(VariableClass, GetFuncId(propType))
        {
            this.path = path;
            propertyName = propName;
        } 
        
        public override bool Init()
        {
            portSlots = new List<LogicPortSlot>();
            var info = LuaNodeResolver.GetLuaNodeInfo(this);
            if (info == null) return false;
            
            info.input.Add(new LuaNodeInfo.Port
            {
                name = "name",
                type = "string"
            });

            foreach (var p in info.input)
            {
                var port = new LogicPortSlot(this, p, Direction.Input);
                portSlots.Add(port);
                if (p.name == "name" && GraphObject.binds.GetWrap(port.BindKey) == null)
                {
                    var wrap = new ValueWrap { name = port.BindKey, type = DataBinder.ValueType.String, value = propertyName};
                    GraphObject.binds.wraps.Add(wrap);
                }
                else if (p.name == "value")
                {
                    port.BindKey = propertyName;
                }
            }

            foreach (var r in info.output)
            {
                portSlots.Add(new LogicPortSlot(this, r, Direction.Output));
            }

            return true;
        }

        private static string GetFuncId(string funcName)
        {
            if (LuaParser.Classes.TryGetValue(VariableClass, out var cls))
            {
                var funcs = cls.GetFunction(funcName);
                if (funcs.Count > 0)
                {
                    return funcs[0].Id;
                }
            }
            return null;
        }

        protected override string GetNodeTitle()
        {
            return $"{propertyName}({Func?.name})";
        }
    }
}