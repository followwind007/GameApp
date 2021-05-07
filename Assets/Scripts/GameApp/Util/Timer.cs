using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace GameApp.Util
{
    public class Timer : MonoBehaviour
    {
        public const int Capacity = 200;
        public struct TimerObj
        {
            public readonly float delay;
            public readonly int limit;
            public readonly bool unscaledTime;
            private Action _callback;
            private Action<int> _countCallback;

            public int count;
            public float last;

            public bool enabled;

            public TimerObj(float delay, Action callback, Action<int> countCallback, int limit, bool unscaledTime)
            {
                this.delay = delay;
                _callback = callback;
                this.limit = limit;
                _countCallback = countCallback;
                this.unscaledTime = unscaledTime;
                
                count = 0;
                last = Time.time;
                enabled = true;
            }

            public void Disable()
            {
                enabled = false;
                _callback = null;
                _countCallback = null;
            }

            public void Invoke()
            {
                _callback?.Invoke();
                _countCallback?.Invoke(count - 1);
            }
            
        }
        public static Action OnUpdate { get; } = null;
        public static Action OnFixedUpdate { get; } = null;
        public static Action OnLateUpdate { get; } = null;

        private static Timer _instance;
        
        private readonly List<TimerObj> _timers = new List<TimerObj>(Capacity);
        
        private static bool _applicationIsQuitting;

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            Application.quitting += () => _applicationIsQuitting = true;
        }
        
        private static Timer Instance
        {
            get
            {
                if (_instance == null && !_applicationIsQuitting && Application.isPlaying)
                {
                    var go = new GameObject("_Timer");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<Timer>();
                }

                return _instance;
            }
        }
        
        public static TimerObj Add(float delay, Action callback, bool unscaledTime = false)
        {
            var t = new TimerObj(delay, callback, null, 1, unscaledTime);
            if (Instance) Instance.Add(t);
            return t;
        }

        public static TimerObj Add(float delay, Action callback, int limit, bool unscaledTime = false)
        {
            var t = new TimerObj(delay, callback, null, limit, unscaledTime);
            if (Instance) Instance.Add(t);
            return t;
        }
        
        public static TimerObj Add(float delay, Action<int> callback, int limit, bool unscaledTime = false)
        {
            var t = new TimerObj(delay, null, callback, limit, unscaledTime);
            if (Instance) Instance.Add(t);
            return t;
        }

        public static void DoStartCoroutine(IEnumerator enumerator)
        {
            if (Instance) Instance.StartCoroutine(enumerator);
        }

        public void Expand(int range)
        {
            for (var i = 0; i < range; i++)
            {
                var t = new TimerObj {enabled = false};
                _timers.Add(t);
            }
        }

        private void Add(TimerObj obj)
        {
            for (var i = 0; i < _timers.Count; i++)
            {
                if (!_timers[i].enabled)
                {
                    _timers[i] = obj;
                    return;
                }
            }
            Debug.LogWarning($"timer count overflow! capacity: {Capacity}");
        }

        private void Awake()
        {
            _instance = this;
            Expand(Capacity);
        }

        private void OnDestroy()
        {
            _instance = null;
            _timers.Clear();
        }

        private const string SampleTag = "C# Timer";
        private void Update()
        {
            OnUpdate?.Invoke();
            Profiler.BeginSample(SampleTag);
            for (var i = 0; i < _timers.Count; i++)
            {
                var t = _timers[i];
                if (!t.enabled) continue;
                var time = t.unscaledTime ? Time.unscaledTime : Time.time;
                if (time - t.last > t.delay)
                {
                    t.last = time;
                    t.count++;
                    if (t.limit > 0 && t.count > t.limit) t.Disable();
                    else t.Invoke();
                    _timers[i] = t;
                }
            }
            Profiler.EndSample();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
        
    }
}