using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace GameApp.Assets
{
    public abstract class BuildRule
    {
        protected readonly string pattern;
        protected readonly IList<string> paths;

        public readonly List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        public readonly List<AssetBundleBuild> valids = new List<AssetBundleBuild>();

        protected BuildRule(string pattern, IList<string> paths)
        {
            this.pattern = pattern;
            this.paths = paths;
            
            GetBuilds();
        }
        
        protected abstract void GetBuilds();
        
        private static readonly Regex ExtReg = new Regex(@"\.(.)+");
        protected string PathToBundleName(string path)
        {
            return ExtReg.Replace(path, "").ToLower();
        }

        protected void Add(AssetBundleBuild build)
        {
            var isValid = BundleCache.IsBuildValid(build);
            if (isValid) BundleCache.SaveCache(build);
            
            build.assetBundleName = BundleCache.BundleNameWithVersion(build.assetBundleName);

            if (isValid) valids.Add(build);
            builds.Add(build);
        }
    }

    public class SingleRule : BuildRule
    {
        public SingleRule(string pattern, IList<string> paths) : base(pattern, paths) { }
        
        protected override void GetBuilds()
        {
            var guids = AssetDatabase.FindAssets(pattern, paths.ToArray());
            foreach (var guid in guids)
            {
                var p = AssetDatabase.GUIDToAssetPath(guid);
                var b = new AssetBundleBuild
                {
                    assetBundleName = PathToBundleName(p),
                    assetNames = new []{p}
                };
                Add(b);
            }
        }
    }

    public class AggregateRule : BuildRule
    {
        public AggregateRule(string pattern, IList<string> paths) : base(pattern, paths) { }

        protected override void GetBuilds()
        {
            foreach (var path in paths)
            {
                var guids = AssetDatabase.FindAssets(pattern, new[] {path});
                var assetPaths = new string[guids.Length];
                for (var i = 0; i < guids.Length; i++)
                {
                    assetPaths[i] = AssetDatabase.GUIDToAssetPath(guids[i]);
                }
                
                var b = new AssetBundleBuild
                {
                    assetBundleName = PathToBundleName(path),
                    assetNames = assetPaths
                };
                Add(b);
            }
        }
    }

    public class DirectSingleRule : BuildRule
    {
        public DirectSingleRule(IList<string> paths) : base(null, paths) { }

        protected override void GetBuilds()
        {
            foreach (var path in paths)
            {
                var b = new AssetBundleBuild
                {
                    assetBundleName = PathToBundleName(path),
                    assetNames = new []{path}
                };
                Add(b);
            }
        }
    }

}