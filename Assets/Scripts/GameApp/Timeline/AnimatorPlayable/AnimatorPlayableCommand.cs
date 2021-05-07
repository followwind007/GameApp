using System;
using UnityEngine;

namespace GameApp.Timeline
{
    [Serializable]
    public class AnimatorPlayableCommand
    {
        public enum CommandType
        {
            SetInteger = 0,
            SetFloat = 1,
            SetBool = 2,
            SetTrigger = 3,
            PlayState = 10,
        }

        public string paramName;
        public CommandType type;

        public int valueInt1;
        public float valueFloat1;
        public bool valueBool1;

        public void DoCommand(Animator animator)
        {
            if (string.IsNullOrEmpty(paramName)) return;
            switch (type)
            {
                case CommandType.SetInteger:
                    animator.SetInteger(paramName, valueInt1);
                    break;
                case CommandType.SetFloat:
                    animator.SetFloat(paramName, valueFloat1);
                    break;
                case CommandType.SetBool:
                    animator.SetBool(paramName, valueBool1);
                    break;
                case CommandType.SetTrigger:
                    animator.SetTrigger(paramName);
                    break;
                case CommandType.PlayState:
                    animator.Play(paramName);
                    break;
                default:
                    break;
            }
        }


    }

}