using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace GameApp.ScenePlayable
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayableDirector))]
    public class PlayableTimeline : ScenePlayableObject
    {
        [PathRef(typeof(TimelineAsset))]
        public string timelinePath;

        public PlayableDirector Director { get; private set; }

        private Dictionary<string, Object> _trackDict;
        private List<Object> _trackList;
        private Dictionary<string, Dictionary<string, PlayableAsset>> _clips;

        private void Awake()
        {
            Director = GetComponent<PlayableDirector>();
            Director.playOnAwake = false;
            var asset = Director.playableAsset;
            if (asset == null)
            {
                if (!string.IsNullOrEmpty(timelinePath))
                    asset = PlayableLoader.LoadAssetAtPath<PlayableAsset>(timelinePath);
                if (asset != null)
                    Director.playableAsset = asset;
            }
            Init();
        }

        public void Init()
        {
            var asset = Director.playableAsset;
            if (asset == null || asset.outputs == null) return;

            _trackDict = new Dictionary<string, Object>();
            _trackList = new List<Object>();
            _clips = new Dictionary<string, Dictionary<string, PlayableAsset>>();

            foreach (PlayableBinding o in asset.outputs)
            {
                string trackName = o.streamName;
                _trackDict.Add(trackName, o.sourceObject);
                _trackList.Add(o.sourceObject);

                TrackAsset track = o.sourceObject as TrackAsset;
                IEnumerable<TimelineClip> clipList = track.GetClips();
                if (clipList == null) continue;

                foreach (TimelineClip c in clipList)
                {
                    if (!_clips.ContainsKey(trackName))
                    {
                        _clips[trackName] = new Dictionary<string, PlayableAsset>();
                    }
                    Dictionary<string, PlayableAsset> name2Clips = _clips[trackName];
                    if (!name2Clips.ContainsKey(c.displayName))
                    {
                        name2Clips.Add(c.displayName, c.asset as PlayableAsset);
                    }
                }
            }
        }

        public void SetBinding(int trackIndex, Object o)
        {
            if (_trackList != null && _trackList.Count > trackIndex)
            {
                TrackAsset track = _trackList[trackIndex] as TrackAsset;
                SetBinding(track, o);
            }
        }

        public void SetBinding(string trackName, Object o)
        {
            TrackAsset track = GetTrack(trackName);
            SetBinding(track, o);
        }

        public void SetBinding<T>(Object o) where T : TrackAsset
        {
            SetBinding(typeof(T), o);
        }

        public void SetBinding(System.Type trackType, Object o)
        {
            foreach (var track in _trackList)
            {
                if (track.GetType() == trackType)
                {
                    SetBinding(track as TrackAsset, o);
                }
            }
        }

        public T GetTrack<T>(string trackName) where T : TrackAsset
        {
            TrackAsset trackAsset = GetTrack(trackName);
            if (trackAsset != null)
            {
                return trackAsset as T;
            }
            return null;
        }

        public TrackAsset GetTrack(string trackName)
        {
            if (_trackDict.ContainsKey(trackName))
            {
                return _trackDict[trackName] as TrackAsset;
            }
            Debug.LogWarning("Timeline doesn't contain track: " + trackName);
            return null;
        }

        public T GetClip<T>(string trackName, string clipName) where T : PlayableAsset
        {
            PlayableAsset clipAsset = GetClip(trackName, clipName);
            if (clipAsset != null)
            {
                return clipAsset as T;
            }
            return null;
        }

        public PlayableAsset GetClip(string trackName, string clipName)
        {
            if (_clips.ContainsKey(trackName))
            {
                var track = _clips[trackName];
                if (track.ContainsKey(clipName))
                {
                    return track[clipName];
                }
            }
            Debug.LogWarning("Track doesn't contain clip, track: " + trackName + ", clip: " + clipName);
            return null;
        }

        public void RecycleAsset()
        {
            Director.playableAsset = null;
        }

        public override void Play()
        {
            Director.Play();
        }

        public override void Pause()
        {
            Director.Pause();
        }

        public override void Stop()
        {
            Director.Stop();
        }

        private void SetBinding(TrackAsset track, Object o)
        {
            if (track != null)
            {
                Director.SetGenericBinding(track, o);
            }
        }

    }
}