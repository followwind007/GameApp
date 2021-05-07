using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tools.ResCheck
{
    public class AtlasChecker : IChecker
    {
        public class AtlasItem
        {
            public string name;
            public string atlasName;
            public FileInfo file;
            public string FilePath => AssetUtil.GetRelativePath(file.FullName);

            public int ReferenceCount
            {
                get
                {
                    var count = 0;
                    foreach (var kv in references)
                    {
                        count += kv.Value;
                    }

                    return count;
                }
            }
            public readonly Dictionary<string, int> references = new Dictionary<string, int>();
            public override string ToString()
            {
                return $"{name}: {ReferenceCount}";
            }
        }
        
        public class AtlasInfo
        {
            public string name;
            public DirectoryInfo directory;
            public readonly Dictionary<string, AtlasItem> sprites = new Dictionary<string, AtlasItem>();
            public override string ToString()
            {
                var res = $"Atlas: {name}\n";
                foreach (var sp in sprites.Values)
                {
                    res += sp + "\n";
                }
                return res;
            }
        }

        public static readonly Dictionary<string, AtlasInfo> Atlases = new Dictionary<string, AtlasInfo>();
        public static readonly Dictionary<string, AtlasItem> Items = new Dictionary<string, AtlasItem>();

        private static AtlasChecker _instance;
        public static AtlasChecker Instance => _instance ?? (_instance = new AtlasChecker());
        

        public void StartCheck()
        {
            Atlases.Clear();
            Items.Clear();
            GenerateAtlasInfo();
            GenerateModuleInfo();
        }

        private void GenerateAtlasInfo()
        {
            var atlasRoot = new DirectoryInfo(ResCheckSettings.Instance.atlasPath);
            if (!atlasRoot.Exists)
            {
                Debug.LogError($"not exist atlas root: {ResCheckSettings.Instance.atlasPath}");
                return;
            }
            var files = new List<FileInfo>();
            AssetUtil.GetFiles(atlasRoot.FullName, files, "*.png");

            var count = 0;
            foreach (var dir in atlasRoot.GetDirectories())
            {
                var atlas = new AtlasInfo
                {
                    name = dir.Name,
                    directory = dir,
                };
                foreach (var file in dir.GetFiles())
                {
                    if (file.Extension == ".meta") continue;

                    var spriteName = GetSpriteName(file);
                    var sprite = new AtlasItem
                    {
                        name = spriteName,
                        atlasName = atlas.name,
                        file = file,
                    };
                    Items[spriteName] = sprite;
                    atlas.sprites[spriteName] = sprite;
                    
                    count++;
                    EditorUtility.DisplayProgressBar("BuildAtlas Dictionary", $"{spriteName}", (float)count / files.Count);
                }
                Atlases[dir.Name] = atlas;
            }
            EditorUtility.ClearProgressBar();
        }

        private void GenerateModuleInfo()
        {
            var i = 0;
            foreach (var module in DataSource.Modules.Values)
            {
                i++;
                var percent = (float) i / DataSource.Modules.Count;
                
                var atlasPath = ResCheckSettings.Instance.atlasPath;
                module.atlasDict.Clear();
                module.spriteDict.Clear();
                module.spriteRefs.Clear();
            
                foreach (var prefab in module.prefabs.Values)
                {
                    var fileDir = prefab.path;
                    var dependencies = AssetDatabase.GetDependencies(fileDir);
                    foreach (var filePath in dependencies)
                    {
                        if (!filePath.Contains(atlasPath)) continue;
                        var spriteName = GetSpriteName(new FileInfo(filePath));
                        Items.TryGetValue(spriteName, out var sprite);
                        if (sprite == null) continue;

                        if (!sprite.references.ContainsKey(fileDir)) sprite.references[fileDir] = 0;
                        sprite.references[fileDir]++;
                        
                        if (!module.atlasDict.ContainsKey(sprite.atlasName))
                            module.atlasDict[sprite.atlasName] = Atlases[sprite.atlasName];

                        module.spriteDict[spriteName] = sprite;
                        if (!module.spriteRefs.ContainsKey(spriteName)) 
                            module.spriteRefs[spriteName] = new List<string>();
                        var refs = module.spriteRefs[spriteName];
                        refs.Add(fileDir);
                    }
                
                    EditorUtility.DisplayProgressBar("Analyse Reference", $"{fileDir}", percent);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        public static string GetSpriteName(string path)
        {
            var file = new FileInfo(path);
            return GetSpriteName(file);
        }
        
        private static string GetSpriteName(FileInfo file)
        {
            return $"{AssetUtil.GetFileFolderName(file.FullName)}/{file.Name}";
        }

    }
}