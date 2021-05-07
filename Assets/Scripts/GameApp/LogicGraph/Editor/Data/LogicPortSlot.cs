using System;
using System.Collections.Generic;
using System.Linq;
using GameApp.DataBinder;
using UnityEditor.Experimental.GraphView;

namespace GameApp.LogicGraph
{
    public class LogicPortSlot
    {
        public enum PortType
        {
            Param, Return, ScheduleIn, ScheduleOut, TranstionIn, TranstionOut
        }
        
        public readonly string valueName;
        public readonly string desc;
        public object Value => owner.GraphObject.binds.GetData(BindKey);

        private string _valType;
        public string ValType 
        {
            get
            {
                var typeFun = LogicGraphSettings.Instance.luaTypes.typeFunction;
                return _valType.StartsWith(typeFun) ? typeFun : _valType;
            }
            set
            {
                _valType = value;
                RemoveValue();
            }
        }

        public readonly Direction portDirection;
        public readonly LogicNode owner;

        public Action<LogicPortSlot> onValueChange;
        
        private static LogicGraphSettings Settings => LogicGraphSettings.Instance;
        
        private static readonly HashSet<string> TypeSet = new HashSet<string>
        {
            "boolean",
            "number",
            "string",
            "schedule",
            "transition",
            "fun",
            "Elua",
            "UnityEngine.Vector2",
            "UnityEngine.Vector3",
            "UnityEngine.Vector4",
            "UnityEngine.Rect",
            "UnityEngine.Color",
            "UnityEngine.AnimationCurve",
            "UnityEngine.Bounds",
            "UnityEngine.Object"
        };
        
        private static readonly HashSet<string> ScheduleTypes = new HashSet<string>
        {
            Settings.luaTypes.typeSchedule,
            Settings.luaTypes.typeTransition
        };
        
        public PortType LogicPortType { get; }

        public string DisplayName => $"{valueName}";

        public string ShortValueType
        {
            get
            {
                var parts = ValType.Split('.');
                return parts[parts.Length - 1];
            }
        }

        public string DisplayValueType => TypeSet.Any(t => t == ValType) ? ShortValueType : "Object";

        public string BindKey { get; set; }

        public string ToolTip => string.IsNullOrEmpty(desc) ? ValType : $"{ValType} {desc}";

        public bool IsInputSlot => portDirection == Direction.Input;
        public bool IsOutputSlot => portDirection == Direction.Output;
        public bool IsScheduleSlot => ScheduleTypes.Contains(ShortValueType);

        public bool IsCompatibleWith(LogicPortSlot other)
        {
            if (owner == other.owner || portDirection == other.portDirection)
            {
                return false;
            }

            var typeObject = LogicGraphSettings.Instance.luaTypes.typeElua;
            if (ValType == typeObject || other.ValType == typeObject)
            {
                return true;
            }

            return LogicGraphSettings.IsTypeCompatible(ValType, other.ValType);
        }

        public bool AllowMultiEdge
        {
            get
            {
                if (IsOutputSlot)
                {
                    //only return port allow multi eage
                    //return LogicPortType == PortType.Return;
                    return true;
                }
                
                return owner.NodeName == Settings.nodes.state;
            }
        }

        public LogicPortSlot(LogicNode owner, LuaNodeInfo.Port p, Direction direction)
        {
            this.owner = owner;
            valueName = p.name;
            _valType = p.type;
            desc = p.desc;
            portDirection = direction;
            
            BindKey = $"{owner.BindNodeGuid}_{valueName}";

            LogicPortType = p.portType;
        }

        public void RemoveValue()
        {
            if (owner.ShortType == LogicGraphSettings.NodeType.Variable && valueName == "value")
            {
                return;
            }
            owner.GraphObject.binds.RemoveWrap(BindKey);
        }

    }
}