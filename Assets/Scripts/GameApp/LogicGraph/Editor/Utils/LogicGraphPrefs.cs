using UnityEngine;

namespace GameApp.LogicGraph
{
    [System.Serializable]
    public class LogicGraphPrefs
    {
        private const string KeyPref = "LogicGraph_Prefs";

        private static LogicGraphPrefs _instance;

        public static LogicGraphPrefs Instance
        {
            get
            {
                if (_instance == null)
                {
                    var json = PlayerPrefs.GetString(KeyPref);
                    if (string.IsNullOrEmpty(json))
                    {
                        _instance = new LogicGraphPrefs();
                    }
                    else
                    {
                        _instance = JsonUtility.FromJson<LogicGraphPrefs>(json) ?? new LogicGraphPrefs();
                    }
                }
            
                return _instance;
            }
        }

        public bool isDebug;
        public bool autoActive;

        public static void SavePrefs()
        {
            if (_instance != null)
            {
                PlayerPrefs.SetString(KeyPref, JsonUtility.ToJson(_instance));
            }
        }
    }
}