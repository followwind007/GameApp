using System;
using UnityEngine;

[ExecuteInEditMode, DisallowMultipleComponent]
public class GuidComponent : MonoBehaviour, ISerializationCallbackReceiver 
{
    public enum ObjectType
    {
        None = 0,
        Prefab = 1,
        Instance = 2,
    } 

    private Guid _guid = Guid.Empty;

    [SerializeField]
    private byte[] _serializedGuid;

    private ObjectType _objectType = ObjectType.None;

    public Guid Guid
    {
        get
        {
            if (_guid == Guid.Empty && _serializedGuid != null && _serializedGuid.Length == 16)
            {
                _guid = new Guid(_serializedGuid);
            }
            return _guid;
        }
    }
    
    public void OnBeforeSerialize()
    {
        RefreshObjectType();
        if (_objectType == ObjectType.Prefab)
        {
            _serializedGuid = new byte[0];
            _guid = Guid.Empty;
        }
        else if (_guid != Guid.Empty)
        {
            _serializedGuid = _guid.ToByteArray();
        }
    }

    public void OnAfterDeserialize()
    {
        if (_serializedGuid != null && _serializedGuid.Length == 16)
        {
            _guid = new Guid(_serializedGuid);
        }
    }

    private void Awake()
    {
        RefreshObjectType();
        CreateGuid();
    }

    private void OnDestroy()
    {
        GuidManager.Remove(_guid);
    }

    private void OnValidate()
    {
        if (_objectType == ObjectType.Prefab)
        {
            _serializedGuid = null;
            _guid = Guid.Empty;
        }
        else
        {
            CreateGuid();
        }
    }

    private void CreateGuid()
    {
        // if serialized data is invalid, it's a new object, create new GUID
        if (_serializedGuid == null || _serializedGuid.Length != 16)
        {
            _guid = Guid.NewGuid();
            _serializedGuid = _guid.ToByteArray();
#if UNITY_EDITOR
            // If lost prefab connection, force a save of the modified prefab instance properties
            if (_objectType == ObjectType.Instance)
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
        }
        else if (_guid == Guid.Empty)
        {
            _guid = new Guid(_serializedGuid);
        }

        if (_guid != Guid.Empty)
        {
            if (!GuidManager.Add(this))
            {
                // if registration fails, create new one.
                _serializedGuid = null;
                _guid = Guid.Empty;
                CreateGuid();
            }
        }
    }

    private void RefreshObjectType()
    {
#if UNITY_EDITOR
        var prefabType = UnityEditor.PrefabUtility.GetPrefabAssetType(this);
        if (prefabType == UnityEditor.PrefabAssetType.Regular 
            || prefabType == UnityEditor.PrefabAssetType.Model
            || prefabType == UnityEditor.PrefabAssetType.Variant)
            _objectType = ObjectType.Prefab;
        else if (prefabType == UnityEditor.PrefabAssetType.NotAPrefab)
            _objectType = ObjectType.Instance;
#endif
    }

}
