using System.Collections.Generic;
using Cinemachine;
using Lean.Touch;
using UnityEngine;

namespace GameApp.CameraControl
{
    public class FreeLookInputer : MonoBehaviour
    {
        public CinemachineFreeLook freeLook;
        public Vector2 axisThreshold = new Vector2(10f, 10f);

        [Range(0f, 60f)]
        public float damp;

        private Vector2 _input;

        public void InputAxis(float x, float y)
        {
            freeLook.m_XAxis.m_InputAxisValue = x;
            freeLook.m_YAxis.m_InputAxisValue = y;
        }

        private void OnEnable()
        {
            LeanTouch.OnGesture += OnGesture;
        }

        private void OnDisable()
        {
            LeanTouch.OnGesture -= OnGesture;
        }

        private void Update()
        {
            _input = Vector2.Lerp(_input, Vector2.zero, Time.deltaTime * damp);
            InputAxis(_input.x, _input.y);
        }

        private void OnGesture(List<LeanFinger> fingers)
        {
            if (!freeLook) return;
            
            var delta = LeanGesture.GetScreenDelta(fingers);
            var x = Mathf.Clamp(delta.x, -axisThreshold.x, axisThreshold.x);
            var y = Mathf.Clamp(delta.y, -axisThreshold.y, axisThreshold.y);

            _input.x = x / axisThreshold.x;
            _input.y = y / axisThreshold.y;
        }
        
    }

}