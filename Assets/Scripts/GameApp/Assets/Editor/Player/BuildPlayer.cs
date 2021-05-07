using System.Diagnostics;
using UnityEditor;
using BS = UnityEditor.EditorUserBuildSettings;
using Debug = UnityEngine.Debug;

namespace GameApp.Assets
{
    public static class BuildPlayer
    {
        public static void Build(BuildTarget target)
        {
            var watch = new Stopwatch();
            watch.Start();
            
            var path = BuildConfig.GetProjectPath(target);

            var bo = BuildOptions.None;
            if (BS.development) bo = bo | BuildOptions.Development;
            if (BS.allowDebugging) bo = bo | BuildOptions.AllowDebugging;
            if (BS.connectProfiler) bo = bo | BuildOptions.ConnectWithProfiler;
            if (BS.buildWithDeepProfilingSupport) bo = bo | BuildOptions.EnableDeepProfilingSupport;
            if (BS.buildScriptsOnly) bo = bo | BuildOptions.BuildScriptsOnly;

            if (AssetSettings.instance.autoStartPlayer)
            {
                bo = bo | BuildOptions.AutoRunPlayer;
            }

            var options = new BuildPlayerOptions
            {
                options = bo,
                target = target,
                locationPathName = path
            };

            var report = BuildPipeline.BuildPlayer(options);
            var buildPath = report.summary.outputPath;
            
            Debug.Log($"build player at: {buildPath}, take {watch.ElapsedMilliseconds / 1000f} seconds");
        }
    }
}