using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameApp.Assets
{
    public enum BuildType
    {
        Single,
        Aggregate,
        Direct
    }

    public enum PathType
    {
        Direct,
        SubPath
    }
    
    public enum BuildMode
    {
        Fresh,
        Progressive
    }

    [CreateAssetMenu(fileName = "BundleSettings", menuName = "Custom/Build/Bundle Settings", order = 201)]
    public class BundleSettings : ScriptableObject
    {
        public static BundleSettings Instance => BuildSettings.Instance.bundleSettings;
        
        public static BuildContext Context => Instance.context;
        
        [Serializable]
        public class BuildContext
        {
            public BuildMode mode;
            public bool useScenesInBuild = true;
        }

        [Serializable]
        public class SearchPath
        {
            public PathType type;
            public string path;
            public string pattern;

            public string[] Paths => GetPaths();
            
            private string[] GetPaths()
            {
                if (type == PathType.Direct)
                {
                    return new[] {path};
                }
                else if(type == PathType.SubPath)
                {
                    return GetSubPaths(path);
                }

                return null;
            }
            
            private static string[] GetSubPaths(string path)
            {
                var dir = new DirectoryInfo(path);
                var subDirs = dir.GetDirectories();
                var paths = new string[subDirs.Length];
                for (var i = 0; i < subDirs.Length; i++)
                {
                    paths[i] = $"{path}/{subDirs[i].Name}";
                }
                return paths;
            }
        }
        
        [Serializable]
        public class Build
        {
            public BuildType buildType;
            public List<SearchPath> paths;
        }
        
        [Serializable]
        public class BuildGroup
        {
            public List<BundleCommand> preCommands;
            public List<BundleCommand> postCommands;
            public List<Build> builds;
        }

        public BuildContext context;
        public List<BundleCommand> preCommands;
        public List<BundleCommand> postCommands;
        public List<BuildGroup> groups;
    }
}