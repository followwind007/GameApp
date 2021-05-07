using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameApp.Assets.Command
{
    [CreateAssetMenu(fileName = "ConcatBundle", menuName = "Custom/Build/Bundle Command/ConcatBundle", order = 0)]
    public class ConcatBundle : BundleCommand
    {
        //group size 100MB
        private const long GroupSize = 100 * 1024 * 1024;
        
        public override void Execute(BundleReport report)
        {
            if (BundleCache.Manifest == null)
            {
                throw new Exception("Can not find manifest file");
            }
            
            var platform = report.target;
            var groupPath = BuildConfig.GetGroupPath(platform);
            
            if (Directory.Exists(groupPath))
            {
                Directory.Delete(groupPath, true);
            }
            Directory.CreateDirectory(groupPath);

            try
            {
                var bundlePath = BuildConfig.GetBundlePath(platform);
                
                ushort groupIdx = 0;
                var groupFileName = $"{groupPath}/{++groupIdx}";
                var fs = File.OpenWrite(groupFileName);
                
                BundleCache.Manifest.bundles.ForEach(b =>
                {
                    var bytes = File.ReadAllBytes($"{bundlePath}/{b.bundle}");
                    if (fs.Length + bytes.Length > GroupSize)
                    {
                        fs.Close();
                        fs.Dispose();
                        groupFileName = $"{groupPath}/{++groupIdx}";
                        fs = File.OpenWrite(groupFileName);
                    }
                    b.gidx = groupIdx;
                    b.offset = (ulong) fs.Length;

                    fs.Write(bytes, 0, bytes.Length);
                });
                
                fs.Close();
                fs.Dispose();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
            EditorUtility.SetDirty(BundleCache.Manifest);
        }
    }
}