using UnityEngine;
using Lean.Touch;
using System.Collections.Generic;
using System;

namespace GameApp.Util
{
    public class GestureHandler : MonoBehaviour
    {
        public Action<List<LeanFinger>> onGesture;
        public Action<LeanFinger> onFingerDown;
        public Action<LeanFinger> onFingerUp;
        public Action<LeanFinger> onFingerTap;

        public void AddCallback()
        {
            if (onGesture != null)
            {
                LeanTouch.OnGesture += onGesture;
            }

            if (onFingerDown != null)
            {
                LeanTouch.OnFingerDown += onFingerDown;
            }

            if (onFingerUp != null)
            {
                LeanTouch.OnFingerUp += onFingerUp;
            }

            if (onFingerTap != null)
            {
                LeanTouch.OnFingerTap += onFingerTap;
            }
        }

        public void RemoveCallback()
        {
            if (onGesture != null)
            {
                LeanTouch.OnGesture -= onGesture;
            }

            if (onFingerDown != null)
            {
                LeanTouch.OnFingerDown -= onFingerDown;
            }

            if (onFingerUp != null)
            {
                LeanTouch.OnFingerUp -= onFingerUp;
            }

            if (onFingerTap != null)
            {
                LeanTouch.OnFingerTap -= onFingerTap;
            }
        }

        private void OnDisable()
        {
            RemoveCallback();
        }

        private void OnEnable()
        {
            AddCallback();
        }

        private void OnDestroy()
        {
            RemoveCallback();
        }


    }
}