using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameApp.LuaResolver;

namespace GameApp.LogicGraph
{
    public class LuaNodeTree
    {
        private static LuaNodeTree _instance;
        public static LuaNodeTree Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LuaNodeTree();
                }
                return _instance;
            }
        }
        public class NodeItem
        {
            public readonly string[] parts;
            public readonly FileInfo file;

            public readonly string classId;
            public readonly string functionId;

            public string FilePath
            {
                get { return AssetUtil.GetRelativePath(file.FullName); }
            }
            
            public NodeItem(LuaClass cls, LuaFunction func)
            {
                var pl = new List<string>();
                pl.AddRange(cls.NameParts);
                pl.Add(func.Id);

                /*
                for (var i = 0; i < pl.Count; i++)
                {
                    var tag = i != pl.Count - 1 ? "n:" : "f:";
                    pl[i] = $"{tag}{pl[i]}";
                }
                */
                
                parts = pl.ToArray();
                file = new FileInfo(cls.file);

                classId = cls.name;
                functionId = func.Id;
            }
            
        }

        public readonly List<NodeItem> tree = new List<NodeItem>();

        private LuaNodeTree()
        {
            GenerateTree();
            LuaParser.onParse += GenerateTree;
        }

        private void GenerateTree()
        {
            tree.Clear();
            var requirePaths = LogicGraphSettings.Instance.requirePaths;

            foreach (var c in LuaParser.Classes.Values)
            {
                var valid = c.tags.Contains("expose") || requirePaths.Any(p => c.file.Contains(p));
                if (!valid) continue;
                c.functions.ForEach(f =>
                {
                    tree.Add(new NodeItem(c, f));
                });
            }
        }

    }
}