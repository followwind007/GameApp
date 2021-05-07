using System.Collections.Generic;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [System.Serializable]
    public class AnimatorPrefs
    {
        [System.Serializable]
        public class AnimatorDataPref
        {
            public AnimatorData data;
            public Vector3 position = Vector3.zero;
            public Vector3 scale = Vector3.one;
        }
        
        private const string KeyPref = "Animator_Prefs";

        private static AnimatorPrefs _instance;

        public static AnimatorPrefs Instance
        {
            get
            {
                if (_instance == null)
                {
                    var json = PlayerPrefs.GetString(KeyPref);
                    if (string.IsNullOrEmpty(json))
                    {
                        _instance = new AnimatorPrefs();
                    }
                    else
                    {
                        _instance = JsonUtility.FromJson<AnimatorPrefs>(json) ?? new AnimatorPrefs();
                    }
                    _instance.Init();
                }
            
                return _instance;
            }
        }

        public bool isParamOn = true;
        public Rect paramPos = new Rect(0, 0, 200, 500);
        
        public bool isInspectorOn = true;
        public Rect inspectorPos = new Rect(400, 0, 300, 800);
        
        public List<AnimatorDataPref> dataPrefs = new List<AnimatorDataPref>();

        public static void SavePrefs()
        {
            if (_instance != null)
            {
                PlayerPrefs.SetString(KeyPref, JsonUtility.ToJson(_instance));
            }
        }

        public void Init()
        {
            var redundant = new List<AnimatorDataPref>();
            dataPrefs.ForEach(p =>
            {
                if (!p.data)
                {
                    redundant.Add(p);
                }
            });
            redundant.ForEach(r => { dataPrefs.Remove(r); });
        }

        public AnimatorDataPref TryGetAnimatorPref(AnimatorData data)
        {
            AnimatorDataPref pref = null;
            foreach (var p in dataPrefs)
            {
                if (p.data == data)
                {
                    pref = p;
                }
            }

            if (pref == null)
            {
                pref = new AnimatorDataPref();
                dataPrefs.Add(pref);
            }

            return pref;
        }

    }
}