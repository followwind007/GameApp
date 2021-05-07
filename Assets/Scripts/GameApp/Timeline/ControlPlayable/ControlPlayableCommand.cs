using UnityEngine;
using System.Collections;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class ControlPlayableCommand
    {
        public enum CommandType
        {
            SetActive = 0,
            SetInActive = 1,
            SetVisible = 2,
            SetInVisible = 3,
        }

        public CommandType type;

        public void DoCommand(GameObject target)
        {
            switch (type)
            {
                case CommandType.SetActive:
                    target.SetActive(true);
                    break;
                case CommandType.SetInActive:
                    target.SetActive(false);
                    break;
                case CommandType.SetVisible:
                    target.SetVisible(true);
                    break;
                case CommandType.SetInVisible:
                    target.SetVisible(false);
                    break;
                default:
                    break;
            }
        }

    }
}
