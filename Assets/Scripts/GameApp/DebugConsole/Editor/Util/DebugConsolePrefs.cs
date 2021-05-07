using UnityEngine;

namespace GameApp.DebugConsole
{
    [System.Serializable]
    public class DebugConsolePrefs
    {
        private const string KeyPref = "DebugConsole_Prefs";

        private static DebugConsolePrefs _instance;

        public static DebugConsolePrefs Instance
        {
            get
            {
                if (_instance == null)
                {
                    var json = PlayerPrefs.GetString(KeyPref);
                    if (string.IsNullOrEmpty(json))
                    {
                        _instance = new DebugConsolePrefs();
                    }
                    else
                    {
                        _instance = JsonUtility.FromJson<DebugConsolePrefs>(json) ?? new DebugConsolePrefs();
                    }
                }
            
                return _instance;
            }
        }

        public string selectedTab = DebugConsoleView.TabConsole;

        public bool autoUpload;
        public ConsoleView.ConsoleMode consoleMode = ConsoleView.ConsoleMode.Local;
        public string command;
        public string commandName;
        public string commandDesc;

        public AnalyzeView.SourceType sourceType = AnalyzeView.SourceType.Select;

        public static void SavePrefs()
        {
            if (_instance != null)
            {
                PlayerPrefs.SetString(KeyPref, JsonUtility.ToJson(_instance));
            }
        }
    }
}