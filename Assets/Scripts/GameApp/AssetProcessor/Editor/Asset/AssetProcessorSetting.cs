using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tools.Table;
using UnityEngine;

namespace GameApp.AssetProcessor
{
    [CreateAssetMenu(fileName = "AssetProcessorSetting", menuName = "Custom/AssetProcessor/AssetProcessorSetting", order = 0)]
    public class AssetProcessorSetting : TableScriptableObject
    {
        [Serializable][CustomDesc]
        public class GameObjectProcessor
        {
            [CustomName("描述")]
            public string desc;
            
            [ReorderableItem(displayHeader = false)][CustomName("路径")]
            public List<string> paths;

            [ReorderableItem(displayHeader = false)][CustomName("GameObject处理")]
            public List<GameObjectProcessorItem> gameObjectProcessors;
            
            [ReorderableItem(displayHeader = false)][CustomName("组件处理")]
            public List<ComponentProcessorItem> componentProcessors;

            public override string ToString()
            {
                return $"[{desc}]";
            }
        }
        
        [Serializable]
        public class GameObjectProcessorItem
        {
            public List<string> checkerIds;
        }
        
        [Serializable]
        public class ComponentProcessorItem
        {
            public string type;
            public string path;
            public List<string> checkerIds;
        }

        private static AssetProcessorSetting _instance;

        public static AssetProcessorSetting Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<AssetProcessorSetting>("AssetProcessorSetting");
                }

                return _instance;
            }
        }

        [CustomName("GameObject处理")][ListItem(useSearch = true)]
        public List<GameObjectProcessor> gameObjectProcessors;

        public static List<GameObjectProcessor> GetGameObjectProcessors(string path)
        {
            return Instance.gameObjectProcessors.Where(gp => 
                gp.paths.Any(p=> 
                    Regex.Match(path, p).Success)).ToList();
        }
    }
}