using System.Collections.Generic;
using UnityEngine;

namespace GameApp.Pool
{
    public class ObjectPool
    {
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly string _name;
        
        public ObjectPool(GameObject prefab, Transform parent, string name)
        {
            _prefab = prefab;
            _parent = parent;
            _name = name;
        }

        public GameObject Fetch()
        {
            return _parent.childCount > 0 ? _parent.GetChild(0).gameObject : Object.Instantiate(_prefab);
        }

        public void Recycle(GameObject go, bool worldPositionStays = false)
        {
            if (go == null) return;
            var poolItem = go.GetComponent<IPoolItem>();
            poolItem?.OnRecycle();
            go.transform.SetParent(_parent, worldPositionStays);
        }

        public void RecycleChilds(Transform trans)
        {
            if (trans == null) return;
            var count = trans.childCount;
            for (var i = 0; i < count; i++)
            {
                Recycle(trans.GetChild(0).gameObject);
            }
        }

        public void Recycle(IEnumerable<GameObject> list)
        {
            if (list == null) return;
            foreach (var go in list)
            {
                Recycle(go);
            }
        }

        public void Destroy()
        {
            if (_parent)
            {
                Pool.Remove(_name);
                Object.Destroy(_parent.gameObject);
            }
        }

        public void Instantiate(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var go = Object.Instantiate(_prefab);
                Recycle(go);
            }
        }
    }

    public class Pool : MonoBehaviour
    {
        public Transform host { get; set; }
        private static Pool _instance;
        private static Pool Instance
        {
            get
            {
                if (_instance == null)
                {
                    var root = GameObject.Find("_Pool");
                    if (root == null)
                    {
                        root = new GameObject("_Pool");
                        root.transform.position = new Vector3(-89757, -89757, -89757);
                        DontDestroyOnLoad(root);
                        
                        var hostGo = new GameObject("Host");
                        hostGo.SetActive(false);
                        hostGo.transform.SetParent(root.transform);
                        hostGo.transform.localPosition = Vector3.zero;
                        
                        _instance = root.AddComponent<Pool>();
                        _instance.host = hostGo.transform;
                        _instance.Init();
                    }
                }

                return _instance;
            }
        }

        private Transform _root;
        
        private readonly Dictionary<string, ObjectPool> _poolDict = new Dictionary<string, ObjectPool>();
        

        public static ObjectPool Create(string poolName, GameObject prefab, int count = 0, bool activePool = false)
        {
            return Instance.InternalCreate(poolName, prefab, count, activePool);
        }

        public static GameObject Fetch(string poolName)
        {
            return Instance.InternalFetch(poolName);
        }

        public static void Recycle(string poolName, GameObject go)
        {
            Instance.InternalRecycle(poolName, go);
        }

        public static void Destroy(string poolName)
        {
            Instance.InternalDestroy(poolName);
        }

        public static void Remove(string poolName)
        {
            if (Instance._poolDict.ContainsKey(poolName))
            {
                Instance._poolDict.Remove(poolName);
            }
        }

        private void Init()
        {
            _root = transform;
        }

        private ObjectPool InternalCreate(string poolName, GameObject prefab, int count, bool activePool = false)
        {
            _poolDict.TryGetValue(poolName, out var pool);
            if (pool == null)
            {
                var parent = new GameObject(poolName);
                parent.transform.SetParent(_root);
                parent.transform.localPosition = Vector3.zero;
                parent.SetActive(activePool);
                
                //if prefab is a instance gameobject
                if (prefab.scene.rootCount != 0)
                {
                    prefab.transform.SetParent(host);
                }

                pool = new ObjectPool(prefab, parent.transform, poolName);
                pool.Instantiate(count);
                _poolDict[poolName] = pool;
            }

            return pool;
        }
        
        private GameObject InternalFetch(string poolName)
        {
            return _poolDict.ContainsKey(poolName) ? _poolDict[poolName].Fetch() : null;
        }
        
        private void InternalRecycle(string poolName, GameObject go)
        {
            if (!_poolDict.ContainsKey(poolName)) return;
            _poolDict[poolName].Recycle(go);
        }

        private void InternalDestroy(string poolName)
        {
            if (!_poolDict.ContainsKey(poolName)) return;
            _poolDict[poolName].Destroy();
            _poolDict.Remove(poolName);
        }

        
    }

}