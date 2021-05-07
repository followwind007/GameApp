using System.Collections.Generic;
using UnityEngine;
using System;

// Class to handle registering and accessing objects by GUID
public class GuidManager
{
    // for each GUID we need to know the Game Object it references
    // and an event to store all the callbacks that need to know when it is destroyed
    private struct GuidInfo
    {
        public GameObject go;

        public event Action<GameObject> OnAdd;
        public event Action OnRemove;

        public GuidInfo(GuidComponent comp)
        {
            go = comp.gameObject;
            OnRemove = null;
            OnAdd = null;
        }

        public void HandleAddCallback()
        {
            if (OnAdd != null)
            {
                OnAdd(go);
            }
        }

        public void HandleRemoveCallback()
        {
            if (OnRemove != null)
            {
                OnRemove();
            }
        }
    }

    private static GuidManager _instance;
    
    static GuidManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GuidManager();
            }
            return _instance;
        }
    }

    private GuidManager()
    {
        _guidToObjectMap = new Dictionary<Guid, GuidInfo>();
    }

    // instance data
    private Dictionary<Guid, GuidInfo> _guidToObjectMap;

    // All the public API is static so you need not worry about creating an instance
    public static bool Add(GuidComponent guidComponent )
    {
        return Instance.InternalAdd(guidComponent);
    }

    public static void Remove(Guid guid)
    {
        Instance.InternalRemove(guid);
    }
    public static GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback = null, Action onRemoveCallback = null)
    {
        return Instance.ResolveGuidInternal(guid, onAddCallback, onRemoveCallback);
    }

    private bool InternalAdd(GuidComponent guidComponent)
    {
        Guid guid = guidComponent.Guid;

        GuidInfo info = new GuidInfo(guidComponent);

        if (!_guidToObjectMap.ContainsKey(guid))
        {
            _guidToObjectMap.Add(guid, info);
            return true;
        }

        GuidInfo existingInfo = _guidToObjectMap[guid];
        if ( existingInfo.go != null && existingInfo.go != guidComponent.gameObject )
        {
            return false;
        }

        // if we already tried to find this GUID, but haven't set the game object to anything specific, copy any OnAdd callbacks then call them
        existingInfo.go = info.go;
        existingInfo.HandleAddCallback();
        _guidToObjectMap[guid] = existingInfo;
        return true;
    }

    private void InternalRemove(Guid guid)
    {
        GuidInfo info;
        if (_guidToObjectMap.TryGetValue(guid, out info))
        {
            // trigger all the destroy delegates that have registered
            info.HandleRemoveCallback();
        }

        _guidToObjectMap.Remove(guid);
    }

    // nice easy api to find a GUID, and if it works, register an on destroy callback
    // this should be used to register functions to cleanup any data you cache on finding
    // your target. Otherwise, you might keep components in memory by referencing them
    private GameObject ResolveGuidInternal(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
    {
        GuidInfo info;
        if (_guidToObjectMap.TryGetValue(guid, out info))
        {
            if (onAddCallback != null)
            {
                info.OnAdd += onAddCallback;
            }

            if (onRemoveCallback != null)
            {
                info.OnRemove += onRemoveCallback;
            }
            _guidToObjectMap[guid] = info;
            return info.go;
        }

        if (onAddCallback != null)
        {
            info.OnAdd += onAddCallback;
        }

        if (onRemoveCallback != null)
        {
            info.OnRemove += onRemoveCallback;
        }

        _guidToObjectMap.Add(guid, info);
        
        return null;
    }
}
