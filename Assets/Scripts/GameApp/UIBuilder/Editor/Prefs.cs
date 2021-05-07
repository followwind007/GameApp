using UnityEngine;

namespace GameApp.UIBuilder
{
    [System.Serializable]
    public class Prefs
    {
        private const string KeyPref = "UIBuilder_Prefs";

        private static Prefs _instance;

        public static Prefs instance
        {
            get
            {
                if (_instance == null)
                {
                    var json = PlayerPrefs.GetString(KeyPref);
                    if (string.IsNullOrEmpty(json))
                    {
                        _instance = new Prefs();
                    }
                    else
                    {
                        _instance = JsonUtility.FromJson<Prefs>(json) ?? new Prefs();
                    }
                }
            
                return _instance;
            }
        }
        
        public Color bgColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        public float scale = 1f;

        public static void SavePrefs()
        {
            if (_instance != null)
            {
                PlayerPrefs.SetString(KeyPref, JsonUtility.ToJson(_instance));
            }
        }
    }
}