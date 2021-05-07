using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameApp.Util;

namespace GameApp.Serialize
{
    public static class DrawerTypesUtil
    {
        private static Dictionary<string, Type> _typeDrawers;
        
        public static Dictionary<string, Type> TypeDrawers
        {
            get
            {
                if (_typeDrawers == null)
                {
                    _typeDrawers = new Dictionary<string, Type>();
                    foreach (var t in TypeUtil.AllTypes)
                    {
                        var attrs = t.GetCustomAttributes(typeof(CustomSerizlizedJsonDrawer)).ToList();
                        if (attrs.Count > 0)
                        {
                            if (attrs[0] is CustomSerizlizedJsonDrawer attr && attr.type.FullName != null)
                            {
                                _typeDrawers[attr.type.FullName] = t;
                            }
                        }
                    }
                    
                }

                return _typeDrawers;
            }
        }

        private static List<Type> _supportedTypes;

        public static List<Type> SupportedTypes
        {
            get
            {
                if (_supportedTypes == null)
                {
                    _supportedTypes = new List<Type>();
                    foreach (var t in TypeUtil.AllTypes)
                    {
                        var attrs = t.GetCustomAttributes(typeof(CustomSerizlizedJsonDrawer)).ToList();
                        if (attrs.Count > 0)
                        {
                            if (attrs[0] is CustomSerizlizedJsonDrawer attr)
                            {
                                _supportedTypes.Add(attr.type);
                            }
                        }
                    }
                }

                return _supportedTypes;
            }
        }
        
        
    }
}