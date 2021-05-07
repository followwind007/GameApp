using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameApp.Util;
using UnityEditor;
using UnityEngine;

namespace GameApp.AssetProcessor
{
    [InitializeOnLoad]
    public static class CompileEntry
    {
        public readonly struct ComponentCheckerItem
        {
            public readonly MethodInfo method;
            public readonly ComponentCheckerAttribute attribute;

            public ComponentCheckerItem(MethodInfo method, ComponentCheckerAttribute attribute)
            {
                this.method = method;
                this.attribute = attribute;
            }
        }
        
        public readonly struct GameObjectCheckerItem
        {
            public readonly MethodInfo method;
            public readonly GameObjectCheckerAttribute attribute;

            public GameObjectCheckerItem(MethodInfo method, GameObjectCheckerAttribute attribute)
            {
                this.method = method;
                this.attribute = attribute;
            }
        }
        
        private static readonly Dictionary<Type, List<ComponentCheckerItem>> ComponentCheckerItems = new Dictionary<Type, List<ComponentCheckerItem>>();
        public static Dictionary<Type, List<ComponentCheckerItem>> ComponentCheckers
        {
            get
            {
                TryInit();
                return ComponentCheckerItems;
            }
        }
        
        private static readonly Dictionary<string, GameObjectCheckerItem> GameObjectCheckerItems = new Dictionary<string, GameObjectCheckerItem>();

        public static Dictionary<string, GameObjectCheckerItem> GameObjectCheckers
        {
            get
            {
                TryInit();
                return GameObjectCheckerItems;
            }
        }
        
        private static bool _initDone;

        static CompileEntry()
        {
            PrefabUtility.prefabInstanceUpdated += OnPrefabInstanceUpdate;
        }

        public static void TryInit()
        {
            if (_initDone) return;
            _initDone = true;
            
            foreach (var t in TypeUtil.AllTypes)
            {
                var ms = t.GetMethods(BindingFlags.Static | BindingFlags.Public);
                foreach (var m in ms)
                {
                    var attrs = m.GetCustomAttributes();
                    foreach (var attr in attrs)
                    {
                        if (attr is ComponentCheckerAttribute compAttr)
                        {
                            ComponentCheckerItems.TryGetValue(compAttr.checkType, out var list);
                            if (list == null)
                            {
                                list = new List<ComponentCheckerItem>();
                                ComponentCheckerItems[compAttr.checkType] = list;
                            }
                    
                            list.Add(new ComponentCheckerItem(m, compAttr));
                        }
                        else if (attr is GameObjectCheckerAttribute goAttr)
                        {
                            GameObjectCheckerItems[goAttr.id] = new GameObjectCheckerItem(m, goAttr);
                        }
                    }
                }
            }
        }

        public static void OnPrefabInstanceUpdate(GameObject go)
        {
            
        }

        public static void CallGameObjectCheckers(List<string> ids, GameObjectCheckerContext ctx)
        {
            foreach (var id in ids)
            {
                CallGameObjectCheckers(id, ctx);
            }
        }
        
        public static void CallGameObjectCheckers(string id, GameObjectCheckerContext ctx)
        {
            if (GameObjectCheckers.TryGetValue(id, out var checker))
            {
                checker.method.Invoke(null, new object[] {ctx});
            }
        }

        public static void CallComponentCheckers(Type type, List<string> ids, ComponentCheckerContext ctx)
        {
            foreach (var id in ids)
            {
                CallComponentCheckers(type, id, ctx);
            }
        }
        
        public static void CallComponentCheckers(Type type, string id, ComponentCheckerContext ctx)
        {
            if (ComponentCheckers.TryGetValue(type, out var checkers))
            {
                var validCheckers = checkers.Where(c => c.attribute.id == id);
                foreach (var c in validCheckers)
                {
                    c.method.Invoke(null, new object[]{ctx});
                }
            }
        }
        
    }
}