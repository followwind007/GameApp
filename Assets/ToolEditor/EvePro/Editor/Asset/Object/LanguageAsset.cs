using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ToolEditor.EvePro.Editor.Asset.Object
{
    [System.Serializable]
    public class LanguageItem
    {
        public string key;
        public string value;
    }

    [System.Serializable]
    public class LanguageAsset : ScriptableObject
    {
        private static LanguageAsset _instance;
        public static LanguageAsset Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.LoadAssetAtPath<LanguageAsset>(EveConst.LANGUAGE_OBJECT_PATH);
                    if (_instance == null)
                    {
                        CreateNewAsset();
                    }
                    _instance.Init();
                }
                return _instance;
            }
        }

        public List<LanguageItem> languageList = new List<LanguageItem>();

        public Dictionary<string, LanguageItem> languageDict = new Dictionary<string, LanguageItem>();

        [MenuItem("Assets/Create/Eve Pro/Language Asset", false, 0)]
        public static void CreateNewAsset()
        {
            var createdAsset = AssetDatabase.LoadAssetAtPath<LanguageAsset>(EveConst.LANGUAGE_OBJECT_PATH);
            if (createdAsset != null)
            {
                EditorUtility.DisplayDialog("Warning", "Eve language asset already exist, don't create again", "OK");
                return;
            }
            _instance = CreateInstance<LanguageAsset>();
            AssetDatabase.CreateAsset(_instance, EveConst.LANGUAGE_OBJECT_PATH);
        }

        public void Init()
        {
            languageDict.Clear();
            foreach (var lu in languageList)
            {
                if (!string.IsNullOrEmpty(lu.key))
                {
                    languageDict[lu.key] = lu;
                }
            }
        }

        public static string GetValue(string key)
        {
            if (Instance.languageDict.ContainsKey(key))
            {
                return Instance.languageDict[key].value;
            }
            return key;
        }

        public void Print()
        {
            foreach (var lu in languageList)
            {
                Debug.Log(string.Format("{0}: {1}", lu.key, lu.value));
            }
        }

    }
}
