using UnityEngine;
using System.Collections.Generic;

namespace Pangu.SceneObject
{
    public class SceneAreaManager
    {
        private static SceneAreaManager _instance;
        public static SceneAreaManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SceneAreaManager();
                }
                return _instance;
            }
        }
        private SceneAreaManager() { }

        private Dictionary<int, bool> _areaLockDict = new Dictionary<int, bool>();
        

        public void SetAreaLockState(int areaId, bool state)
        {
            _areaLockDict[areaId] = state;
        }

        public bool GetAreaLockState(int areaId)
        {
            if (_areaLockDict.ContainsKey(areaId))
            {
                return _areaLockDict[areaId];
            }
            return false;
        }

        public void DumpAreaInfo()
        {
            foreach (var kvLock in _areaLockDict)
            {
                Debug.Log(string.Format("area {0}, lock state: {1}", kvLock.Key, kvLock.Value));
            }
        }


    }

}