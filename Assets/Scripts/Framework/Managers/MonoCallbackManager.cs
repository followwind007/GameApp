using UnityEngine;
using System;
using System.Collections.Generic;

namespace Pangu.Manager
{
    public class MonoCallbackManager : MonoBehaviour
    {
        public static MonoCallbackManager Instance { get; private set; }

        public enum CallbackType
        {
            Update = 0,
            FixedUpdate = 1,
            LateUpdate = 2,
            OnDestroy = 3,
        }

        public delegate void CallbackDelegate();

        private Dictionary<CallbackType, CallbackDelegate> _callbackDict = new Dictionary<CallbackType, CallbackDelegate>();

        public void AddCallback(CallbackType type, CallbackDelegate callback)
        {
            if (!_callbackDict.ContainsKey(type))
            {
                _callbackDict[type] = callback;
            }
            else
            {
                _callbackDict[type] += callback;
            }
        }

        public void RemoveCallback(CallbackType type, CallbackDelegate callback)
        {
            if (_callbackDict.ContainsKey(type))
            {
                _callbackDict[type] -= callback;
                if (_callbackDict[type] == null)
                {
                    _callbackDict.Remove(type);
                }
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (_callbackDict.ContainsKey(CallbackType.Update))
            {
                _callbackDict[CallbackType.Update]();
            }
        }

        private void FixedUpdate()
        {
            if (_callbackDict.ContainsKey(CallbackType.FixedUpdate))
            {
                _callbackDict[CallbackType.FixedUpdate]();
            }
        }

        private void LateUpdate()
        {
            if (_callbackDict.ContainsKey(CallbackType.LateUpdate))
            {
                _callbackDict[CallbackType.LateUpdate]();
            }
        }

        private void OnDestroy()
        {
            if (_callbackDict.ContainsKey(CallbackType.OnDestroy))
            {
                _callbackDict[CallbackType.OnDestroy]();
            }
        }

    }

}