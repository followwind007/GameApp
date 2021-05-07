using System.Collections.Generic;
using UnityEngine;
using GameApp.ScenePlayable;

namespace Framework.Manager
{
    public class ScenePlayableManager
    {
        private static ScenePlayableManager _instance;
        public static ScenePlayableManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScenePlayableManager();
                    _instance.Init();
                }
                return _instance;
            }
        }

        public Dictionary<string, List<ScenePlayableObject>> PlayableDict { get; private set; }

        private Dictionary<string, List<PlayableEvent>> _unReceivedDict;

        private Vector3 _senderPosition;
        
        /// <summary>
        /// register a playble object
        /// </summary>
        /// <typeparam name="T">with super class ScenePlayableObject</typeparam>
        /// <param name="playable">playbale object</param>
        public void RegisterPlayable<T>(T playable) where T : ScenePlayableObject
        {
            if (playable == null || string.IsNullOrEmpty(playable.eventName)) return;
            List<ScenePlayableObject> playableList = null;

            if (PlayableDict.ContainsKey(playable.eventName))
            {
                playableList = PlayableDict[playable.eventName];
                if (playableList.Contains(playable))
                {
                    Debug.LogWarning(string.Format("event name:{0}, already exsit!", playable.eventName));
                    return;
                }
            }
            else
            {
                playableList = new List<ScenePlayableObject>();
                PlayableDict[playable.eventName] = playableList;
            }
            playableList.Add(playable);
            TriggerUnReceived(playable);
        }

        /// <summary>
        /// deregister a playable object
        /// </summary>
        /// <typeparam name="T">with super class ScenePlayableObject</typeparam>
        /// <param name="playable">playable object</param>
        public void DeRegisterPlayable<T>(T playable) where T : ScenePlayableObject
        {
            if (playable == null || string.IsNullOrEmpty(playable.eventName)) return;
            if (!PlayableDict.ContainsKey(playable.eventName))
            {
                Debug.LogWarning(string.Format("event name:{0}, not registered!", playable.eventName));
                return;
            }
            var playableList = PlayableDict[playable.eventName];
            playableList.Remove(playable);
            if (playableList.Count < 1)
            {
                PlayableDict.Remove(playable.eventName);
            }
        }

        public ScenePlayableObject GetPlayableObject(string eventName)
        {
            var list = GetPlayableObjectList(eventName);
            if (list != null && list.Count > 0) return list[0];
            return null;
        }

        public List<ScenePlayableObject> GetPlayableObjectList(string eventName)
        {
            if (PlayableDict.ContainsKey(eventName))
                return PlayableDict[eventName];
            return null;
        }

        /// <summary>
        /// default accept event with "Play" operation
        /// </summary>
        /// <param name="eventName">event name</param>
        public void OnReceiveEventPlayable(string eventName)
        {
            OnReceiveEventPlayable(eventName, ScenePlayableUtil.EVENT_OP_PLAY);
        }

        /// <summary>
        /// receive event with event name and operation
        /// </summary>
        /// <param name="eventName">event name</param>
        /// <param name="operation">operation, ref ScenePlayableUtil.EVENT_OP_****</param>
        public void OnReceiveEventPlayable(string eventName, string operation)
        {
            if (!PlayableDict.ContainsKey(eventName))
            {
                var playableEvent = new PlayableEvent(eventName, PlayableEvent.TriggerType.Event);
                RegisterUnReceived(playableEvent);
                return;
            }
            List<ScenePlayableObject> playableList = PlayableDict[eventName];
            foreach (var playable in playableList)
            {
                switch (operation)
                {
                    case ScenePlayableUtil.EVENT_OP_PLAY:
                        playable.Play();
                        break;
                    case ScenePlayableUtil.EVENT_OP_PAUSE:
                        playable.Pause();
                        break;
                    case ScenePlayableUtil.EVENT_OP_STOP:
                        playable.Stop();
                        break;
                    default:
                        break;
                }
            }
        }

        public void TriggerEventNearby(GameObject sender, string eventName, bool triggerAllFlag = false)
        {
            float radius = ScenePlayableUtil.DEFALUT_OVERLAP_RADIUS;
            TriggerEventNearby(sender, eventName, radius, triggerAllFlag);
        }

        public void TriggerEventNearby(GameObject sender, string eventName, float radius, bool triggerAllFlag = false)
        {
            if (sender == null) return;
            TriggerEventNearby(sender.transform.position, eventName, radius, triggerAllFlag);
        }

        /// <summary>
        /// trigger playable event on nearby receiver
        /// </summary>
        /// <param name="senderPos">sender position</param>
        /// <param name="eventName">event name</param>
        /// <param name="radius">radius between sender and receiver</param>
        /// <param name="triggerAllFlag">will event be sent to all receiver inside radius, false default</param>
        public void TriggerEventNearby(Vector3 senderPos, string eventName, float radius, bool triggerAllFlag = false)
        {
            Collider[] cols = Physics.OverlapSphere(senderPos, radius, ScenePlayableUtil.ReceiverLayer);
            List<Collider> colList = new List<Collider>(cols);

            _senderPosition = senderPos;
            colList.Sort(CompareColliderWithDist);

            bool received = false;
            foreach (var col in colList)
            {
                var ps = col.gameObject.GetComponent<ScenePlayableObject>();
                if (ps != null && ps.eventName.Equals(eventName))
                {
                    received = true;
                    ps.Play();
                    if (!triggerAllFlag) break;
                }
            }
            if (!received)
            {
                var playbleEvent = new PlayableEvent(eventName, PlayableEvent.TriggerType.Nearby, senderPos, radius);
                RegisterUnReceived(playbleEvent);
            }
        }

        private void Init()
        {
            PlayableDict = new Dictionary<string, List<ScenePlayableObject>>();
            _unReceivedDict = new Dictionary<string, List<PlayableEvent>>();
            _instance = this;
        }

        private void TriggerUnReceived(ScenePlayableObject playable)
        {
            if (!_unReceivedDict.ContainsKey(playable.eventName)) return;
            foreach (var ue in _unReceivedDict[playable.eventName])
            {
                if (ue.IsAcceptable(playable))
                    playable.Play();
            }
        }

        private void RegisterUnReceived(PlayableEvent playableEvent)
        {
            if (!_unReceivedDict.ContainsKey(playableEvent.eventName))
            {
                _unReceivedDict[playableEvent.eventName] = new List<PlayableEvent>();
            }
            var list = _unReceivedDict[playableEvent.eventName];
            //make sure no duplicated add, but playable with same name and position also be redundant
            foreach (var ue in list)
            {
                if (ue.triggerPosition.Equals(playableEvent.triggerPosition))
                {
                    return;
                }
            }
            list.Add(playableEvent);
        }

        private void DeregisterUnReceived(PlayableEvent playableEvent)
        {
            if (_unReceivedDict.ContainsKey(playableEvent.eventName))
            {
                var list = _unReceivedDict[playableEvent.eventName];
                list.Remove(playableEvent);
                if (list.Count == 0)
                {
                    _unReceivedDict.Remove(playableEvent.eventName);
                }
            }
        }

        private int CompareColliderWithDist(Collider a, Collider b)
        {
            float distA = Vector3.Distance(a.transform.position, _senderPosition);
            float distB = Vector3.Distance(b.transform.position, _senderPosition);
            return distA.CompareTo(distB);
        }
            


    }
}
