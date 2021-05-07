using System;
using System.Collections.Generic;
using GameApp.DataBinder;
using UnityEngine;
using UnityEngine.Playables;

namespace Tools.ResCheck
{

    [Serializable]
    public class CopySetting
    {
        public enum ComponentType
        {
            Animator,
            PlayableDirector,
            BehaviourBinder,
        }
        
        private static readonly Dictionary<Type, ComponentType> ComponentMap = new Dictionary<Type, ComponentType>
        {
            {typeof(Animator), ComponentType.Animator},
            {typeof(PlayableDirector), ComponentType.PlayableDirector},
            {typeof(BehaviourBinder), ComponentType.BehaviourBinder}
        };

        public List<ComponentType> excludeTypes = new List<ComponentType>();

        public bool IsExcludeType(Type t)
        {
            if (ComponentMap.TryGetValue(t, out var p))
            {
                return excludeTypes.Contains(p);
            }

            return false;
        }
    }
    
    [Serializable]
    public class CopyItem
    {
        public GameObject source;
        public string savePath;
        public CopySetting copySetting;
    }

    [CreateAssetMenu(fileName = "PrefabImportSetting", menuName = "Custom/Res/PrefabImportSetting", order = 100)]
    public class PrefabImportSetting : ScriptableObject
    {
        public const string Name = "PrefabImportSetting";
        
        private static PrefabImportSetting _instance;
        public static PrefabImportSetting Instance => _instance ? _instance : _instance = Resources.Load<PrefabImportSetting>(Name);
        

        public List<CopyItem> copyItems = new List<CopyItem>();
    }
}