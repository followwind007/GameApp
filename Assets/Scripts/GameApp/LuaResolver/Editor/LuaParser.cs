using System;
using System.Collections.Generic;
using System.IO;
using Tools;
using UnityEditor;

namespace GameApp.LuaResolver
{
    public class LuaParser
    {
        private static LuaParser _instance;
        public static LuaParser Instance
        {
            get
            {
                if (_instance == null) _instance = new LuaParser();
                return _instance;
            }
        }

        public static Action onParse;

        public bool showLog;

        private LuaParser()
        {
            Init();
        }

        public LuaResolverSettings Settings => LuaResolverSettings.Instance;
        public static Dictionary<string, LuaClass> Classes
        {
            get
            {
                if (Instance._classDict.Count == 0)
                {
                    Instance.Init();
                }

                return Instance._classDict;
            }
        }

        private readonly Dictionary<string, LuaClass> _classDict = new Dictionary<string, LuaClass>();

        [MenuItem("Tools/LuaResolver", false, 901)]
        public static void StartParse()
        {
            Instance.Parse();
        }

        public void Init()
        {
            var classesList = LuaResolverSettings.Instance.luaClasses;
            if (classesList.Count > 0)
            {
                classesList.ForEach(cls => { _classDict[cls.name] = cls; });
            }
            else
            {
                Parse();
            }
        }

        public void Parse(bool showLogFlag = true)
        {
            showLog = showLogFlag;
            _classDict.Clear();
            
            var files = new List<FileInfo>();

            try
            {
                Settings.luaPaths.ForEach(lp =>
                {
                    var tmp = new List<FileInfo>();
                    AssetUtil.GetFiles(lp, tmp, "*.lua");
                    files.AddRange(tmp);
                });
                
                Settings.luaClasses.Clear();

                var count = files.Count;
                for (var i = 0; i < count; i++)
                {
                    var info = $"Dealing: {AssetUtil.GetRelativePath(files[i].FullName)}";
                    var cls = ParseFile(files[i]);
                    if (cls != null && !string.IsNullOrEmpty(cls.name)) Settings.luaClasses.Add(cls); 
                    EditorUtility.DisplayProgressBar("Parsing", info, (float) i / count);
                }
                EditorUtility.SetDirty(Settings);
                AssetDatabase.SaveAssets();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            
            GenerateInheritTree();
            
            onParse?.Invoke();
        }

        public LuaClass GetClass(string path)
        {
            return ParseFile(new FileInfo(path));
        }

        private LuaClass ParseFile(FileInfo f)
        {
            var resolver = ClassResolver.Instance;
            resolver.Resolve(f);
            foreach (var c in resolver.classes)
            {
                _classDict[c.name] = c;
            }
            
            return resolver.Class;
        }

        private void GenerateInheritTree()
        {
            
        }
        
    }
}