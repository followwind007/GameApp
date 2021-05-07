using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.LogicGraph
{
    public class LogicGraphSettings : ScriptableObject
    {
        [Serializable]
        public class LogicGraphSetting
        {
            public string nodeDispose = "Dispose";
            public string formatDispose = "";
            public string nodeAddTimer = "AddTimer";
            public string nodeCallback = "Callback_";
            public string formatAddTimer = "";
            public string formatStartTimer = "";
        }

        [Serializable]
        public class LuaAnnotations
        {
            public string param = "param";
            public string Return = "return";
            public string vararg = "vararg";
            public string input = "input";
            public string output = "output";
        }
        
        [Serializable]
        public class LuaTypes
        {
            public string typeNumber = "number";
            public string typeBool = "boolean";
            public string typeString = "string";
            public string typeTable = "table";
            public string typeElua = "Elua";
            public string typeSchedule = "schedule";
            public string typeTransition = "transition";
            public string typeFunction = "fun";
        }
        
        [Serializable]
        public class NodeTypes
        {
            public string Event = "Event";
            public string logic = "Logic";
            public string math = "Math";
            public string variable = "Variable";
            public string state = "State";
            public string graph = "Graph";
            public string callback = "Callback";
            public string property = "Property";
        }

        [Serializable]
        public class StateGraphSetting
        {
            public string nodeEntry = "Entry";
            public string nodeAnyState = "AnyState";
            public string nodeState = "State";
            public string nodeExit = "Exit";
            public string nodeTransition = "Transition";

            public string formatCreate = "";
            public string formatAddParam = "";
            public string formatCreateState = "";
            public string formatAddState = "";
            public string formatAddCondition = "";
            public string formatAddLink = "";
            public string formatPlayState = "";
            public string formatDispose = "";
        }
        
        [Serializable]
        public class GraphDebug
        {
            public bool debugMode = true;
            public float notifyStayDura = 2f;
            public string formatNodeEnter = "";
            public string formatNodeExit = "";
        }
        
        public class LuaClass
        {
            public const string Event = "LogicGraph.Event";
            public const string Logic = "LogicGraph.Logic";
            public const string Math = "LogicGraph.Math";
            public const string Variable = "LogicGraph.Variable";
            public const string State = "LogicGraph.State";
            public const string Graph = "LogicGraph.Graph";
            public const string Callback = "LogicGraph.Callback";
            public const string Property = "LogicGraph.Property";
        }
        
        public enum NodeType
        {
            Event,
            Logic,
            Math,
            Variable,
            State,
            Graph,
            Callback,
            Property,
            Other,
        }
        
        private static readonly Dictionary<string, NodeType> NodeTypeMap = new Dictionary<string, NodeType>
        {
            {LuaClass.Event, NodeType.Event},
            {LuaClass.Logic, NodeType.Logic},
            {LuaClass.Math, NodeType.Math},
            {LuaClass.Variable, NodeType.Variable},
            {LuaClass.State, NodeType.State},
            {LuaClass.Graph, NodeType.Graph},
            {LuaClass.Callback, NodeType.Callback},
            {LuaClass.Property, NodeType.Property}
        };
        
        private static LogicGraphSettings _instance;
        public static LogicGraphSettings Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<LogicGraphSettings>("LogicGraphSettings");
                    if (_instance != null) _instance.Init(); 
                    else Debug.LogError("can not find LogicGraphSettings in Resources");
                }

                return _instance;
            }
        }

        public static string GetVariableNodePath()
        {
            return $"{Instance.nodes.variable}.lua";
        }

        public static string GetRequirePath(string path)
        {
            return path
                .Replace(Instance.luaRoot, "")
                .Replace(".lua", "")
                .Replace("/", ".");
        }

        public static NodeType GetNodeType(string clsName)
        {
            return NodeTypeMap.TryGetValue(clsName, out var type) ? type : NodeType.Other;
        }

        public static bool IsTypeCompatible(string typeA, string typeB)
        {
            if (typeA == typeB) return true;
            foreach (var typeSet in _instance._compatibleTypes) if (typeSet.Contains(typeA) && typeSet.Contains(typeB)) return true;
            return false;
        }
        
        private static readonly HashSet<NodeType> PlaceHolders = new HashSet<NodeType>
        {
            NodeType.Event, NodeType.Graph, NodeType.Variable, NodeType.Callback
        };
        public static bool IsPlaceHolderNode(NodeType type)
        {
            return PlaceHolders.Contains(type);
        }

        public string luaRoot = "Assets/Lua/";
        public string graphAssetPath = "Assets/Scripts/LogicGraph/Resources";
        public string graphCodePath = "Assets/Lua/LogicGraph/Gen";

        public List<string> requirePaths;

        public LogicGraphSetting logicGraph = new LogicGraphSetting();
        
        public StateGraphSetting stateGraph = new StateGraphSetting();
        
        public GraphDebug graphDebug = new GraphDebug();

        public NodeTypes nodes = new NodeTypes();

        public LuaAnnotations luaAnnotation;

        public LuaTypes luaTypes = new LuaTypes();

        private HashSet<HashSet<string>> _compatibleTypes = new HashSet<HashSet<string>>();

        public void Init()
        {
            _compatibleTypes = new HashSet<HashSet<string>>
            {
                new HashSet<string>{luaTypes.typeTransition, luaTypes.typeFunction}
            };
        }

    }
}