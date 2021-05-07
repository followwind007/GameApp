using UnityEngine;
using System.Collections.Generic;
using GameApp.RenderTarget;
using GameApp.Util;

public class RenderTargetManager
{
    private static RenderTargetManager _instance;
    public static RenderTargetManager instance 
    {
        get { return _instance ?? (_instance = new RenderTargetManager()); }
    }
    
    private const float DefaultDestroyDelay = 5f;

    private readonly Vector3 _startPosition = new Vector3(9192, 0, 0);
    private readonly Vector3 _positionGap = new Vector3(128, 0, 0);

    private readonly Dictionary<GameObject, List<GameObject>> _itemDict = new Dictionary<GameObject, List<GameObject>>();
    private readonly List<GameObject> _itemList = new List<GameObject>();

    private readonly Dictionary<GameObject, int> _referenceCount = new Dictionary<GameObject, int>();

    public RenderTarget GetRenderTextureTarget(GameObject prefab, bool isUnique)
    {
        if (prefab == null) return null;
        var go = GetItem(prefab, isUnique);
        if (go)
        {
            var target = go.GetComponent<RenderTarget>();
            return target;
        }
        return null;
    }

    public GameObject GetItem(GameObject prefab, bool isUnique)
    {
        GameObject go = null;
        if (!isUnique && _itemDict.ContainsKey(prefab))
        {
            var list = _itemDict[prefab];
            foreach (var item in list)
            {
                if (item != null)
                {
                    go = item;
                    RetainItem(prefab, go);
                    break;
                }
            }
        }
        if (go == null)
        {
            go = CreateNewItem(prefab);
        }
        return go;
    }

    public void RetainItem(GameObject prefab, GameObject go)
    {
        if (prefab == null || go == null) return;
        
        if (_itemDict.ContainsKey(prefab))
        {
            if (_referenceCount.ContainsKey(go))
            {
                _referenceCount[go]++;
            }
            else
            {
                _referenceCount[go] = 1;
            }
        }
    }

    public void ReleaseItem(GameObject prefab, GameObject go)
    {
        if (prefab == null || go == null) return;
        
        if (_itemDict.ContainsKey(prefab))
        {
            if (_referenceCount.ContainsKey(go))
            {
                if (--_referenceCount[go] < 1)
                {
                    var target = prefab.GetComponent<RenderTarget>();
                    var destroyDelay = target == null ? DefaultDestroyDelay : target.destroyDelay;
                    #pragma warning disable CShap0001
                    Timer.Add(destroyDelay, () => DelayDestroyItem(prefab, go) );
                }
            }
        }
    }
    
    private GameObject CreateNewItem(GameObject prefab)
    {
        var pos = GetItemPosition();
        var go = Object.Instantiate(prefab, pos, prefab.transform.rotation);

        if (!_itemDict.ContainsKey(prefab))
        {
            _itemDict[prefab] = new List<GameObject>();
        }
        _itemDict[prefab].Add(go);
        _itemList.Add(go);
        RetainItem(prefab, go);
        return go;
    }

    private void DelayDestroyItem(GameObject prefab, GameObject go)
    {
        if (_itemDict.ContainsKey(prefab))
        {
            if (_referenceCount.ContainsKey(go) && _referenceCount[go] < 1)
            {
                Object.Destroy(go);
                _itemDict[prefab].Remove(go);
                if (_itemDict[prefab].Count < 1)
                {
                    _itemDict.Remove(prefab);
                }
                _itemList.Remove(go);
            }
        }
    }

    private Vector3 GetItemPosition()
    {
        var pos = _startPosition;
        if (_itemList.Count > 0)
        {
            for (var i = _itemList.Count - 1; i >= 0 ; i--)
            {
                var go = _itemList[i];
                if (go)
                {
                    pos = go.transform.position + _positionGap;
                    break;
                }
            }
        }
        return pos;
    }

}