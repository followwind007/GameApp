using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using GameApp.Pool;
using Random = UnityEngine.Random;

namespace GameApp.Emitter
{   
    public class CustomEmitter : MonoBehaviour
    {
        public enum EmitType
        {
            UI,
        }

        public enum Status
        {
            Run,
            Stop,
            Pause,
        }

        public EmitType emitType = EmitType.UI;
        
        public bool playOnAwake;

        public float duration = 1f;

        public float life = 5f;
        
        [Header("item generated in one second")]
        [Range(0.1f, 100)]
        public float generateSpeed = 1f;

        public float speed = 10f;

        public bool isReverse;

        public float innerRadius;
        public float outterRadius;
        
        public List<GameObject> emitObjects = new List<GameObject>();

        public Action<GameObject> onEmitGameObject;

        private float _simulateCount;
        private Status _status = Status.Stop;
        private readonly List<ObjectPool> _pools = new List<ObjectPool>();

        private float _gapCount;

        private WaitForSeconds _seconds;
        
        private float GenerateGap
        {
            get { return 1f / generateSpeed; }
        }

        private void Awake()
        {
            if (emitObjects.Count < 1)
            {
                Debug.LogWarning("emit list null");
                return;
            }
            _seconds = new WaitForSeconds(life);
            foreach (var obj in emitObjects)
            {
                _pools.Add(Pool.Pool.Create(obj.name, obj));
            }
            if (playOnAwake)
            {
                StartEmit();
            }
        }

        public void StartEmit()
        {
            _status = Status.Run;
        }

        public void PauseEmit()
        {
            _status = Status.Pause;
        }

        public void StopEmit()
        {
            _status = Status.Stop;
            _simulateCount = 0f;
        }

        private void Update()
        {
            if (_status != Status.Run)
            {
                return;
            }
            
            _gapCount += Time.deltaTime;
            if (_gapCount > GenerateGap)
            {
                _gapCount = 0f;
                ScheduleOneShot();
            }
            
            _simulateCount += Time.deltaTime;
            if (_simulateCount > duration)
            {
                StopEmit();
            }
        }
        
        private void ScheduleOneShot()
        {
            var poolIndex = Random.Range(0, _pools.Count - 1);
            var ins = _pools[poolIndex].Fetch();
            onEmitGameObject?.Invoke(ins);

            if (emitType == EmitType.UI)
            {
                var rectTrans = (RectTransform)ins.transform;
                var half = new Vector2(0.5f, 0.5f);
                rectTrans.pivot = half;
                rectTrans.anchorMin = half;
                rectTrans.anchorMax = half;
                rectTrans.SetParentCenter(transform);

                var path = GetPath();
                rectTrans.anchoredPosition = path[0];
                
                var tweenDura = Vector3.Distance(path[0], path[1]) / speed;
                rectTrans.DOAnchorPos(path[1], tweenDura);
            }

            StartCoroutine(RecycleObject(poolIndex, ins));
        }

        private IEnumerator RecycleObject(int index, GameObject obj)
        {
            yield return _seconds;
            _pools[index].Recycle(obj);
        }

        private Vector3[] GetPath()
        {
            var path = new Vector3[2];
            var rand = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector3.up;
            path[0] = rand * (isReverse ? outterRadius : innerRadius);
            path[1] = rand * (isReverse ? innerRadius : outterRadius);
            return path;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var trans = transform;
            var pos = trans.position;
            var up = trans.up;
            var forward = trans.forward;

            var inner = innerRadius;
            var outter = outterRadius;
            if (emitType == EmitType.UI)
            {
                var canvas = GetComponentInParent<Canvas>();
                if (canvas)
                {
                    if (!canvas.isRootCanvas) canvas = canvas.rootCanvas;
                    var scale = canvas.transform.localScale.x;
                    inner = innerRadius * scale;
                    outter = outterRadius * scale;
                }
            }
            
            Handles.color = Color.red;
            Handles.DrawWireArc(pos, forward, up, 360, inner);
            Handles.DrawWireArc(pos, forward, up, 360, outter);
        }
#endif
        
        
        
    }

}
