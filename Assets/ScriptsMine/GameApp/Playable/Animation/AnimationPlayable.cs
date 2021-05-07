using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using GameApp.ScenePlayable;

namespace Pangu.Playble
{
    public enum AnimationPlayType
    {
        First, AllMix,
    }

    public enum AnimationTriggerType
    {
        OnEnable,
        OnVisible,
    }

    [Serializable]
    public class AnimationClipElement
    {
        [PathRef(typeof(AnimationClip))]
        public string animationPath;

        [NonSerialized]
        private AnimationClip _clip;

        public AnimationClip clip
        {
            get
            {
                if (_clip == null)
                {
                    _clip = PlayableLoader.LoadAssetAtPath<AnimationClip>(animationPath);
                }
                return _clip;
            }
        }
    }

    [Serializable]
    public class AnimationPlayableEvent
    {
        public AnimationTriggerType triggerType;
        public AnimationPlayType playType;
        public List<int> indexList;
        public List<string> nameList;
    }

    [RequireComponent(typeof(Animator))]
    public class AnimationPlayable : MonoBehaviour, IVisibleObject
    {
        public bool playOnAwake;

        [Tooltip("播放类型: \nFirst (播放第一个) \nAllMix (播放全部并使用混合)")]
        public AnimationPlayType playType;

        [Tooltip("播放的Animation Clip列表, 默认不会加载, 在播放时加载")]
        public List<AnimationClipElement> clips = new List<AnimationClipElement>();

        [Tooltip("播放AnimationClip的触发事件")]
        public List<AnimationPlayableEvent> events = new List<AnimationPlayableEvent>();

        private Animator _animator;

        private readonly List<AnimationClipElement> _targetClips = new List<AnimationClipElement>();

        private PlayableGraph _playableGraph;
        private bool _playableGraphCreated;

        public void PlaySpecific(List<int> indexList)
        {
            MarkClipUsage(indexList);
            Play();
        }

        public void PlaySpecific(List<string> nameList)
        {
            MarkClipUsage(nameList);
            Play();
        }

        public void Play()
        {
            if (_targetClips.Count < 1) return;
            if (_playableGraphCreated && _playableGraph.IsPlaying())
            {
                ClearPlayableGraph();
            }

            _playableGraphCreated = true;
            _playableGraph = PlayableGraph.Create();

            var playableSize = _targetClips.Count;
            if (playType == AnimationPlayType.First)
                playableSize = 1;

            var clipPlayableList = new List<AnimationClipPlayable>();
            for (var i = 0; i < playableSize; i++)
            {
                var clipPlayable = AnimationClipPlayable.Create(_playableGraph, _targetClips[i].clip);
                clipPlayableList.Add(clipPlayable);
            }

            var mixerPlayable = AnimationMixerPlayable.Create(_playableGraph, _targetClips.Count);
            for (var i = 0; i < playableSize; i++)
            {
                _playableGraph.Connect(clipPlayableList[i], 0, mixerPlayable, i);
                mixerPlayable.SetInputWeight(i, 1);
            }
            var playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
            playableOutput.SetSourcePlayable(mixerPlayable);

            _playableGraph.Play();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (playOnAwake)
            {
                _targetClips.AddRange(clips);
                Play();
            }
        }

        private void OnEnable()
        {
            TriggerEvent(AnimationTriggerType.OnEnable);
        }

        private void TriggerEvent(AnimationTriggerType triggerType)
        {
            foreach (var evt in events)
            {
                if (evt.triggerType == triggerType)
                {
                    playType = evt.playType;
                    if (evt.nameList.Count > 0)
                    {
                        MarkClipUsage(evt.nameList);
                    }
                    else if (evt.indexList.Count > 0)
                    {
                        MarkClipUsage(evt.indexList);
                    }
                    else
                    {
                        _targetClips.Clear();
                        _targetClips.AddRange(clips);
                    }
                    Play();
                    break;
                }
            }
        }

        private void MarkClipUsage(List<int> indexList)
        {
            _targetClips.Clear();
            for (var i = 0; i < clips.Count; i++)
            {
                if (indexList.Contains(i)) _targetClips.Add(clips[i]);
            }
        }

        private void MarkClipUsage(List<string> nameList)
        {
            foreach (var clip in clips)
            {
                foreach (var clipName in nameList)
                {
                    if (clip.animationPath.Contains(clipName))
                    {
                        _targetClips.Add(clip);
                        break;
                    }
                }
            }
        }

        private void OnDisable()
        {
            ClearPlayableGraph();
        }

        private void ClearPlayableGraph()
        {
            if (_playableGraphCreated)
            {
                _playableGraphCreated = false;
                _playableGraph.Stop();
                _playableGraph.Destroy();
            }
        }

        public void OnVisible(bool isVisible)
        {
            if (isVisible)
            {
                TriggerEvent(AnimationTriggerType.OnVisible);
            }
            else
            {
                ClearPlayableGraph();
            }
        }
    }
}
