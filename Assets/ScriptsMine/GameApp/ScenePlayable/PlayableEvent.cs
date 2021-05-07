using UnityEngine;

namespace GameApp.ScenePlayable
{
    public class PlayableEvent
    {
        public enum TriggerType
        {
            Event = 0,
            Nearby = 1,
        }

        public string eventName;
        public string operation = "play";
        public TriggerType triggerType;
        public Vector3 triggerPosition;
        public float triggerRadius;

        public PlayableEvent(string name, TriggerType type)
        {
            eventName = name;
            triggerType = type;
        }

        public PlayableEvent(string name, TriggerType type, Vector3 triggerPos, float radius)
        {
            eventName = name;
            triggerType = type;
            triggerPosition = triggerPos;
            triggerRadius = radius;
        }

        public bool IsAcceptable(ScenePlayableObject playableObject)
        {
            if (playableObject == null) return false;
            if (playableObject.isTrigger)
                return IsAcceptable(playableObject.transform.position);
            else
                return IsAcceptable(playableObject.eventName);
        }

        public bool IsAcceptable(Vector3 receiverPosition)
        {
            if (triggerType == TriggerType.Nearby)
            {
                return Vector3.Distance(triggerPosition, receiverPosition) < triggerRadius;
            }
            return false;
        }

        public bool IsAcceptable(string name)
        {
            if (triggerType == TriggerType.Event)
            {
                return string.Equals(name, eventName);
            }
            return false;
        }


    }
}
