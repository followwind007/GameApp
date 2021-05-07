using System.Collections.Generic;
using System.Linq;
using GameApp.LuaResolver;
using UnityEngine;

namespace GameApp.LogicGraph
{
    public class LuaNodeInfo
    {
        public class Port
        {
            public string name;
            public string type;
            public string desc;

            public LogicPortSlot.PortType portType;
        }

        public string summary;
        public readonly List<Port> input = new List<Port>();
        public readonly List<Port> output = new List<Port>();
    }
    
    public static class LuaNodeResolver
    {
        private static LogicGraphSettings Settings => LogicGraphSettings.Instance;
        
        private static readonly HashSet<LogicGraphSettings.NodeType> CustomScheduleTypes = new HashSet<LogicGraphSettings.NodeType>
        {
            LogicGraphSettings.NodeType.Event,
            LogicGraphSettings.NodeType.Logic,
            LogicGraphSettings.NodeType.Callback,
            LogicGraphSettings.NodeType.Variable
        };

        public static LuaNodeInfo GetLuaNodeInfo(LogicNode node)
        {
            if (!LuaParser.Classes.TryGetValue(node.classId, out var cls))
            {
                Debug.LogError($"no class [{node.classId}] found");
                return null;
            }
            
            var func = cls.functions.FirstOrDefault(f => f.Id == node.functionId);
            if (func == null)
            {
                Debug.LogError($"no function [{node.functionId} found in [{node.classId}]");
                return null;
            }
            
            var info = new LuaNodeInfo { summary = func.summary };
            
            var nt = LogicGraphSettings.GetNodeType(cls.name);
            
            if (CustomScheduleTypes.Contains(nt))
            {
                DealScheule(cls, func, info);
            }
            else
            {
                AddScheduleIn(info);
                AddScheduleOut(info);
            }

            if (func.useSelf)
            {
                info.input.Add(new LuaNodeInfo.Port
                {
                    name = "self", 
                    type = cls.name, 
                    desc = "object to do this function",
                    portType = LogicPortSlot.PortType.Param
                });
            }
            
            func.paramters.ForEach(p =>
            {
                var isCallback = nt == LogicGraphSettings.NodeType.Callback;
                var target = isCallback ? info.output : info.input;
                target.Add(new LuaNodeInfo.Port
                {
                    name = p.name, 
                    type = p.typeName, 
                    desc = p.desc,
                    portType = isCallback ? LogicPortSlot.PortType.Return : LogicPortSlot.PortType.Param
                });
            });

            var returnTypes = func.returnInfo.typeNames;
            for (var i = 0; i < returnTypes.Count; i++)
            {
                info.output.Add(new LuaNodeInfo.Port
                {
                    name = $"return_{i + 1}", 
                    type = returnTypes[i],
                    portType = LogicPortSlot.PortType.Return
                });
            }

            return info;
        }

        private static void DealScheule(LuaClass cls, LuaFunction func, LuaNodeInfo info)
        {
            var annoInput = $"---@{Settings.luaAnnotation.input}";
            var annoOutput = $"---@{Settings.luaAnnotation.output}";
            for (var i = func.defineLine - 1; i >= 0; i--)
            {
                var line = cls.Lines[i].str;
                    
                if (!line.Contains(Reserves.Annotation)) break;
                if (line.Contains(annoInput) || line.Contains(annoOutput))
                {
                    var ti = ParseUtil.GetTypeInfo(line);
                    var port = new LuaNodeInfo.Port {name = ti.name, type = ti.typeName, desc = ti.desc};
                    if (line.Contains(annoInput))
                    {
                        port.portType = IsTransition(port.type)
                            ? LogicPortSlot.PortType.TranstionIn
                            : LogicPortSlot.PortType.ScheduleIn;
                        info.input.Insert(0, port);
                    }
                    else
                    {
                        port.portType = IsTransition(port.type)
                            ? LogicPortSlot.PortType.TranstionOut
                            : LogicPortSlot.PortType.ScheduleOut;
                        info.output.Insert(0, port);
                    }
                }
            }
        }

        private static void AddScheduleIn(LuaNodeInfo info)
        {
            info.input.Insert(0, new LuaNodeInfo.Port
            {
                name = "in", 
                type = Settings.luaTypes.typeSchedule, 
                portType = LogicPortSlot.PortType.ScheduleIn
            });
        }

        private static void AddScheduleOut(LuaNodeInfo info)
        {
            info.output.Insert(0, new LuaNodeInfo.Port
            {
                name = "out", 
                type = Settings.luaTypes.typeSchedule,
                portType = LogicPortSlot.PortType.ScheduleOut
            });
        }

        private static bool IsTransition(string type)
        {
            return type == Settings.luaTypes.typeTransition;
        }

    }
}