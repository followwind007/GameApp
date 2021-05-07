using System.Reflection;
using TMPro;

namespace Framework.Managers
{
    public class TMPManager
    {
        public static void SetSettings(TMP_Settings settings)
        {
            var settingsType = settings.GetType();
            var instanceField = settingsType.GetField("s_Instance", BindingFlags.Static | BindingFlags.NonPublic);
            instanceField?.SetValue(null, settings);
        }
    }
}