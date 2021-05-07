using System;
using System.Collections.Generic;
using System.Linq;
using GameApp.LuaResolver;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GameApp.LogicGraph
{
    [Serializable]
    public class LogicNode
    {
        [NonSerialized] 
        public List<LogicPortSlot> portSlots = new List<LogicPortSlot>();

        public string path;

        public string classId;
        public string functionId;
        
        public string nodeGuid;
        public string BindNodeGuid => nodeGuid.Replace(".", "_");
        
        public Vector2Int position;
        
        public bool expanded = true;

        public bool hideLeaf;

        public Guid groupGuid;

        public LuaClass Cls 
        {
            get
            {
                LuaParser.Classes.TryGetValue(classId, out var cls);
                return cls;
            }
        }
        public LuaFunction Func 
        {
            get
            {
                if (Cls != null)
                {
                    Cls.FunctionDict.TryGetValue(functionId, out var func);
                    return func;
                }

                return null;
            }
        }

        public SerializedNode SerializedNode { get; set; }
        
        public string Summary { get; private set; }

        public LogicGraphObject GraphObject { get; set; }
        
        [NonSerialized]
        public readonly Dictionary<LogicPortSlot, List<LogicNode>> linkNodes = new Dictionary<LogicPortSlot, List<LogicNode>>();

        public string NodeName => Func?.name;
        public string NodeTitle => GetNodeTitle();
        public string RequireName => Cls?.name.Replace(".", "_");

        public string CallName => Cls != null && Cls.IsCsType ? $"{Cls?.name}.{Func?.name}" : $"{RequireName}.{Func?.name}";
        public string NodeType => $"{classId}.{Func?.name}";

        public string ShortGuid
        {
            get
            {
                var parts = nodeGuid.Split('.');
                return parts[parts.Length - 1];
            }
        }

        public LogicGraphSettings.NodeType ShortType => LogicGraphSettings.GetNodeType(Cls?.name);

        public LogicNode(string classId, string functionId)
        {
            this.classId = classId;
            this.functionId = functionId;
            path = Cls?.RelativePath;
        }
        
        //for deserialize from json 
        public LogicNode() { }

        public virtual bool Init()
        {
            var info = LuaNodeResolver.GetLuaNodeInfo(this);

            if (info == null)
            {
                return false;
            }

            Summary = info.summary;
            foreach (var p in info.input)
            {
                portSlots.Add(new LogicPortSlot(this, p, Direction.Input));
            }

            foreach (var r in info.output)
            {
                portSlots.Add(new LogicPortSlot(this, r, Direction.Output));
            }

            return true;
        }

        public virtual void OnGraphResolved() { }

        public void Serialize()
        {
            SerializedNode.json = JsonUtility.ToJson(this);
        }
        
        public void OnRemove()
        {
            foreach (var slot in portSlots.Where(s => s.IsInputSlot))
            {
                slot.RemoveValue();
            }
        }

        public LogicPortSlot GetPortSlot(string name)
        {
            return portSlots.FirstOrDefault(p => p.valueName == name);
        }

        protected virtual string GetNodeTitle()
        {
            return $"{Cls?.ShortName}.{Func?.name}";
        }

    }
}