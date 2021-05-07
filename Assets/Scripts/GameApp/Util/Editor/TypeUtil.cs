using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameApp.Util
{
    public static class TypeUtil
    {
        public static IEnumerable<Assembly> Assemblies => AppDomain.CurrentDomain.GetAssemblies();
        private static IEnumerable<Type> _types;
        public static IEnumerable<Type> AllTypes {
            get
            {
                if (_types == null)
                {
                    var list = new List<Type>();
                    foreach (var a in Assemblies)
                    {
                        foreach (var t in a.GetTypes())
                        {
                            list.Add(t);
                        }
                    }

                    _types = list.ToArray();
                }

                return _types;
            }
        }

        public static Type GetType(string type)
        {
            return AllTypes.FirstOrDefault(t => t.ToString() == type);
        }
        
    }
}