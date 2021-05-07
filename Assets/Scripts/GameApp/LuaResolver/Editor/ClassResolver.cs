using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tools;
using UnityEngine;

namespace GameApp.LuaResolver
{
    public class ClassResolver
    {
        private static ClassResolver _instance;
        public static ClassResolver Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClassResolver();
                }

                return _instance;
            }
        }

        private FileInfo _file;
        private List<LineInfo> _lines = new List<LineInfo>();

        public readonly List<LuaClass> classes = new List<LuaClass>();

        public LuaClass Class => classes.Count > 0 ? classes[0] : null;

        public void Resolve(FileInfo file)
        {
            _file = file;
            classes.Clear();
            Generate();
        }

        public void Generate()
        {
            var ls = File.ReadAllLines(_file.FullName);
            _lines = ParseUtil.GetLineInfo(ls);
            
            CreateClasses();

            if (classes.Count <= 0) return;

            CreateFunctions();
        }

        private void CreateClasses()
        {
            classes.Clear();
            var classLines = new List<int>();
            var tagLine = -1;
            foreach (var l in _lines)
            {
                if (l.type == LineType.Class)
                    classLines.Add(l.line);
                else if (l.type == LineType.Tag)
                    tagLine = l.line;
            }

            classLines.ForEach(cl =>
            {
                var name = ParseUtil.GetClassName(_lines[cl].str, out var ps);
                var cls = new LuaClass
                {
                    name = name,
                    parentNames = ps,
                    file = AssetUtil.GetRelativePath(_file.FullName)
                };

                if (tagLine >= 0)
                {
                    cls.tags = ParseUtil.GetTags(_lines[tagLine].str);
                }
                
                var isReverse = true;
                if (cl + 1 < _lines.Count)
                {
                    var clNext = _lines[cl + 1];
                    if (clNext.type == LineType.Property || clNext.type == LineType.PublicProperty) isReverse = false;
                }

                var gap = isReverse ? -1 : 1;
                var endIdx = FetchProperty(cls, cl, gap);
                
                var defineLine = isReverse ? cl + 1 : endIdx;
                if (defineLine < _lines.Count) cls.defineName = ParseUtil.GetClassDefineName(_lines[defineLine].str);

                cls.propertyList.Reverse();
                classes.Add(cls);
            });
        }

        private int FetchProperty(LuaClass cls, int start, int gap)
        {
            var i = start;
            while (true)
            {
                i = i + gap;
                if (i < 0 || i > _lines.Count) break;
                    
                var pl = _lines[i];
                    
                if (pl.type == LineType.Property || pl.type == LineType.PublicProperty)
                {
                    var p = ParseUtil.GetPropertyInfo(pl.str);
                    if (p != null)
                    {
                        p.index = cls.Properties.Count;
                        if (cls.Properties.ContainsKey(p.name)) 
                            LogError($"property [{p.name}] already exist!");
                        cls.Properties[p.name] = p;
                        cls.propertyList.Add(p);
                    }
                }
                else if (pl.str.Contains(Reserves.Summary))
                {
                    cls.summary = ParseUtil.GetSummary(pl.str);
                }
                else
                {
                    break;
                }
            }

            return i;
        }

        private void CreateFunctions()
        {
            foreach (var fl in _lines.Where(l => l.type == LineType.Function && !ParseUtil.IsAnnotated(l.str)))
            {
                var f = GetFunction(fl.line);
                var cls = classes.FirstOrDefault(c => c.defineName == f.selfName);
                if (cls != null)
                {
                    var funcId = f.Id;
                    if (cls.FunctionDict.ContainsKey(funcId))
                    {
                        Debug.LogError($"{cls.file}:{fl.line + 1}[{funcId}] is already exist!");
                        continue;
                    }
                    cls.functions.Add(f);
                    cls.FunctionDict[f.Id] = f;
                }
            }
        }
        
        private LuaFunction GetFunction(int index)
        {
            var def = _lines[index].str;
            var fun = new LuaFunction
            {
                name = ParseUtil.GetFunctionName(def),
                selfName = ParseUtil.GetFunctionSelfName(def),
                useSelf = def.Contains(":"),
                defineLine = index
            };
            
            for (var i = index - 1; i >= 0; i--)
            {
                var line = _lines[i].str;
                if (line.Contains(Reserves.Param))
                {
                    var ti = ParseUtil.GetTypeInfo(line);
                    if (ti != null)
                    {
                        var p = new ParameterInfo
                        {
                            name = ti.name,
                            typeName = ti.typeName,
                            literalDescs = ti.literalDescs,
                            desc = ti.desc
                        };
                        fun.paramters.Insert(0, p);
                    }
                }
                else if (line.Contains(Reserves.Return))
                {
                    var ri = ParseUtil.GetReturnInfo(line);
                    if (ri != null) fun.returnInfo = ri;
                }
                else if (line.Contains(Reserves.Summary))
                {
                    fun.summary = ParseUtil.GetSummary(line);
                }
                else
                {
                    break;
                }
            }

            return fun;
        }

        private void LogError(string log)
        {
            if (!LuaParser.Instance.showLog) return;
            Debug.LogError($"[{AssetUtil.GetRelativePath(_file.FullName)}] {log}");
        }

    }
}