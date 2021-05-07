using System;
using System.Collections;
using UnityEngine;

namespace GameApp.Util
{
    public class Updater : MonoBehaviour
    {
        private static Updater _instance;
        public static Updater Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = MonoHolder.Holder.AddComponent<Updater>();
                }

                return _instance;
            }
        }

        public static Action UpdateCallback
        {
            get => Instance._updateCallback;
            set => Instance._updateCallback = value;
        }

        public static Action LateUpdateCallback
        {
            get => Instance._lateUpdateCallback;
            set => Instance._lateUpdateCallback = value;
        }

        private Action _updateCallback;
        private Action _lateUpdateCallback;

        public static Coroutine StartCo(IEnumerator enumerator)
        {
            return Instance.StartCoroutine(enumerator);
        }

        public static void StopCo(Coroutine co)
        {
            Instance.StopCoroutine(co);
        }

        private void Update()
        {
            _updateCallback?.Invoke();
        }

        private void LateUpdate()
        {
            _lateUpdateCallback?.Invoke();
        }
    }
}