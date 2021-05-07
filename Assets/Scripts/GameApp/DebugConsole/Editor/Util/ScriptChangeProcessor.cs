using Tools;
using UnityEditor;
using UnityEngine;

namespace GameApp.DebugConsole
{
    public class ScriptChangeProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (Application.isPlaying && DebugConsoleWindow.Instance && DebugConsolePrefs.Instance.autoUpload)
            {
                foreach (var path in importedAssets)
                {
                    if (!path.EndsWith(".lua")) continue;
                    foreach (var op in DebugConsoleSettings.Instance.observeLuaPaths)
                    {
                        if (path.Contains(op))
                        {
                            //ServerDebugHandler.Instance.Upload(path, AssetUtil.GetRelativePath(path));
                        }
                    }
                }
            }
        }
    }
}