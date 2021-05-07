using UnityEngine;
using System;

namespace GameApp.Util
{
    public class ValueTweener : MonoBehaviour {
        private Vector3 _from,_to;
        private float _duration;
        private Action<Vector3> _update;
        private Action _finish;
        private float _elapsedtime;
        private bool _working;
        
        public static Vector3 ValueTo(GameObject obj, Vector3 from, Vector3 to,float duration, Action<Vector3> update = null, Action finish = null)
        {
            var tween = obj.GetComponent<ValueTweener>();
            if (!tween) tween = obj.AddComponent<ValueTweener>();
            
            tween._elapsedtime = 0;
            tween._working = true;
            tween.enabled = true;
            tween._from = from;
            tween._to = to;
            tween._duration = duration;
            tween._update = update;
            tween._finish = finish;
            return Vector3.zero;
        }

        private static Vector3 QuadOut(Vector3 start,Vector3 end,float duration,float elapsedTime)
        {
            if (elapsedTime >= duration) return end;
            return (elapsedTime / duration) * (elapsedTime / duration - 2) * -(end - start) + start;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_working)
            {
                _elapsedtime += Time.deltaTime;
                var value = QuadOut(_from, _to, _duration, _elapsedtime);
                _update?.Invoke(value);
                if (_elapsedtime>=_duration)
                {
                    _working = false;
                    enabled = false;
                    _finish?.Invoke();
                }
            }
        }
        
    }
}