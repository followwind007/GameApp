using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GameApp.Assets
{
    public class BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public BuildPlayerSettings Settings => BuildSettings.Instance.playerSettings;
        public int callbackOrder => 10;
        public void OnPreprocessBuild(BuildReport report)
        {
            Settings.preCommands.ForEach(cmd => cmd.Execute(report));
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            Settings.postSettings.ForEach(cmd => cmd.Execute(report));
        }
    }
}