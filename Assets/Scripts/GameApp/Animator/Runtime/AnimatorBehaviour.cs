using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.AnimatorBehaviour
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorBehaviour : MonoBehaviour, IAnimationClipSource
    {
        public AnimatorData Data
        {
            get { return data; }
            set
            {
                data = value;
                Init();
            }
        }

        public AnimatorDataOverride Overrides
        {
            get { return overrides; }
            set
            {
                overrides = value;
                Init();
            }
        }

        public DirectorUpdateMode UpdateMode
        {
            get { return updateMode; }
            set
            {
                updateMode = value;
                if (_initDone)
                {
                    Runner.UpdateMode = updateMode;
                }
            }
        }

        [SerializeField]
        private AnimatorData data;
        
        [SerializeField]
        private AnimatorDataOverride overrides;
        
        [SerializeField]
        private DirectorUpdateMode updateMode = DirectorUpdateMode.GameTime;

        private bool _initDone;

        public AnimatorRunner Runner { get; private set; }
        public Animator Animator { get; private set; }

        public void Play(string stateName)
        {
            Runner?.Play(stateName);
        }

        public void SetTrigger(string paramName)
        {
            Runner?.SetTrigger(paramName);
        }

        public void SetInt(string paramName, int value)
        {
            Runner?.SetInt(paramName, value);
        }

        public void SetBool(string paramName, bool value)
        {
            Runner?.SetBool(paramName, value);
        }

        public void SetFloat(string paramName, float value)
        {
            Runner?.SetFloat(paramName, value);
        }

        public void PlayAdditive(string stateName)
        {
            Runner?.PlayAdditive(stateName);
        }

        public void StopAdditive(string stateName)
        {
            Runner?.StopAddtive(stateName);
        }

        public AnimatorState GetState(string stateName)
        {
            return Runner?.GetState(stateName);
        }

        public AnimatorStateBehaviour GetBehaviour(string stateName)
        {
            return Runner?.GetBehaviour(stateName);
        }
        
        public void GetAnimationClips(List<AnimationClip> results)
        {
            if (results == null || data == null && overrides == null) return;
            var d = data ? data : overrides.animator;
            if (!d) return;
            
            var clipDict = new Dictionary<string, AnimationClip>();
            
            d.states.ForEach(s => { if (s is IClipState cs) clipDict[s.StateName] = cs.Clip; });
            if (overrides) overrides.overrides.ForEach(o => { if (o is IClipState co) clipDict[o.stateName] = co.Clip; });
            
            results.AddRange(clipDict.Values);
        }

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            if (!data && overrides) data = overrides.animator;
            Init();
        }

        private void Init()
        {
            Runner?.Destroy();
            if (Data)
            {
                Runner = new AnimatorRunner(Data, Overrides, Animator);
                
                Runner.Init();
                _initDone = true;
            }
        }

        private void Update()
        {
            Runner?.OnUpdate();
        }

        private void OnEnable()
        {
            Runner?.Graph.Play();
        }

        private void OnDisable()
        {
            Runner?.Graph.Stop();
        }

        private void OnDestroy()
        {
            Runner?.Destroy();
        }
        
    }
    
}