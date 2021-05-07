
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tools.Table
{
    public class TableAttrs
    {
        public struct ExposedFunc
        {
            public ExposedFuncAttribute attr;
            public MethodInfo method;
            public string DisplayName => string.IsNullOrEmpty(attr.displayName) ? method.Name : attr.displayName;
        }
        
        public readonly Type type;
        
        public readonly List<string> tabs = new List<string>();
        public readonly Dictionary<string, HashSet<string>> tabFields = new Dictionary<string, HashSet<string>>();
        public readonly HashSet<string> nonTabFields = new HashSet<string>();
        
        public readonly List<string> lines = new List<string>();
        public readonly Dictionary<string, List<string>> lineFields = new Dictionary<string, List<string>>();
        public readonly Dictionary<string, string> allLineFields = new Dictionary<string, string>();
        
        public readonly Dictionary<string, ListItemAttribute> listItemAttrs = new Dictionary<string, ListItemAttribute>();
        
        public readonly Dictionary<string, ReorderableItemAttribute> reorderableItemAttrs = new Dictionary<string, ReorderableItemAttribute>();

        public readonly CustomDescAttribute customDesc;

        public readonly List<ExposedFunc> exposedFuncs = new List<ExposedFunc>();
        public readonly List<List<ExposedFunc>> exposedLineFuncs = new List<List<ExposedFunc>>();
        
        public readonly Dictionary<string, CustomNameAttribute> customNames = new Dictionary<string, CustomNameAttribute>();

        public TableAttrs(Type type)
        {
            this.type = type;
            
            //type attributes
            var tAttrs = type.GetCustomAttributes();
            foreach (var tAttr in tAttrs)
            {
                if (tAttr is CustomDescAttribute customDescAttr)
                {
                    customDesc = customDescAttr;
                }
            }
            
            //fields attributes
            var fields = type.GetFields();
            foreach (var f in fields)
            {
                //UnityEngine.Debug.Log($"{type.Name}, {fields.Length}, {f.Name}");
                var attrs = f.GetCustomAttributes();
                var hasTab = false;
                foreach (var attr in attrs)
                {
                    if (attr is TabItemAttribute tabAttr)
                    {
                        hasTab = true;
                        if (!tabFields.ContainsKey(tabAttr.tab))
                        {
                            tabs.Add(tabAttr.tab);
                            tabFields[tabAttr.tab] = new HashSet<string>();
                        }
                        tabFields[tabAttr.tab].Add(f.Name);
                    }
                    else if (attr is LineItemAttribute lineAttr)
                    {
                        if (!lineFields.ContainsKey(lineAttr.line))
                        {
                            lines.Add(lineAttr.line);
                            lineFields[lineAttr.line] = new List<string>();
                        }

                        allLineFields[f.Name] = lineAttr.line;
                        lineFields[lineAttr.line].Add(f.Name);
                        
                    }
                    else if (attr is ListItemAttribute listItemAttr)
                    {
                        listItemAttrs[f.Name] = listItemAttr;
                    }
                    else if (attr is ReorderableItemAttribute reorderableItemAttr)
                    {
                        reorderableItemAttrs[f.Name] = reorderableItemAttr;
                    }
                    else if (attr is CustomNameAttribute customNameAttr)
                    {
                        customNames[f.Name] = customNameAttr;
                    }
                }

                if (!hasTab) nonTabFields.Add(f.Name);
            }

            //method attributes
            var methods = type.GetMethods();
            foreach (var m in methods)
            {
                var mAttrs = m.GetCustomAttributes();
                foreach (var mAttr in mAttrs)
                {
                    if (mAttr is ExposedFuncAttribute exposedFuncAttr)
                    {
                        exposedFuncs.Add(new ExposedFunc { attr = exposedFuncAttr, method = m});
                    }
                }
            }
            
            //sort exposed funcs
            SortExposedFuncs();
        }

        private void SortExposedFuncs()
        {
            exposedLineFuncs.Clear();
            exposedFuncs.Sort((a, b) => a.attr.priority.CompareTo(b.attr.priority));
            var groups = exposedFuncs.GroupBy(f => f.attr.priority > 0 ? f.attr.priority / 100 * 100 : 0).ToList();
            groups.Sort((a, b) => a.Key.CompareTo(b.Key));

            foreach (var g in groups)
            {
                var fs = new List<ExposedFunc>();
                foreach (var exposedFunc in g) fs.Add(exposedFunc);
                exposedLineFuncs.Add(fs);
            }
        }
        
    }
    
    public static class TableAttrUtil
    {
        private static Dictionary<Type, TableAttrs> _tableAttrs = new Dictionary<Type, TableAttrs>();

        public static TableAttrs GetTableAttrs(Type type)
        {
            if (!_tableAttrs.ContainsKey(type))
            {
                _tableAttrs[type] = new TableAttrs(type);
            }

            return _tableAttrs[type];
        }
    }
}