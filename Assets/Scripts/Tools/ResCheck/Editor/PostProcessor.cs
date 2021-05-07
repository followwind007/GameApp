using System;
using System.Collections.Generic;
using System.Linq;
using GameApp.Util;

namespace Tools.ResCheck
{
    public interface IModelPostProcessor
    {
        string Catagory { get; }
        int Priority { get; }

        void Process(string path);
    }

    public static class ProcessorUtil
    {
        private static List<IModelPostProcessor> _modelPostProcessors;
        public static List<IModelPostProcessor> ModelPostProcessors {
            get
            {
                if (_modelPostProcessors == null)
                {
                    _modelPostProcessors = new List<IModelPostProcessor>();
                    foreach (var t in TypeUtil.AllTypes)
                    {
                        if (t.GetInterfaces().Any(i => i == typeof(IModelPostProcessor)))
                        {
                            if (Activator.CreateInstance(t) is IModelPostProcessor p) _modelPostProcessors.Add(p);
                        }
                    }
                    _modelPostProcessors.Sort((a, b) => a.Priority.CompareTo(b.Priority) );
                }

                return _modelPostProcessors;
            }
        }
    }
    
}