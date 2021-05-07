using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorRunner
    {
        public AnimatorData Data { get; }
        public AnimatorDataOverride DataOverride { get; }

        private DirectorUpdateMode _updateMode = DirectorUpdateMode.GameTime;
        public DirectorUpdateMode UpdateMode {
            set
            {
                _updateMode = value;
                RefreshUpdateMode();
            }
        }

        public Action<string> onPlayState;
        public Action<string> onStopState;
        public Action<AnimatorTransfer> onTransfer;

        public string CurrentStateName { get; private set; }
        public AnimatorState CurrentState => _stateDict[CurrentStateName];
        public AnimatorStateBehaviour CurrentBehaviour => GetBehaviour<AnimatorStateBehaviour>(CurrentStateName);
        
        public AnimatorTransfer CurrentTransfer { get; private set; }
        public AnimatorTransferBehaviour CurrentTransferBehaviour => GetTransferBehaviour<AnimatorTransferBehaviour>(CurrentTransfer);
        
        public Animator Animator { get; }

        public PlayableGraph Graph { get; private set; }
        public AnimationHandler Handler { get; private set; }

        private readonly Dictionary<string, AnimatorState> _stateDict = new Dictionary<string, AnimatorState>();
        private readonly Dictionary<string, AnimatorStateBehaviour> _behaviourDict = new Dictionary<string, AnimatorStateBehaviour>();
        private readonly Dictionary<string, AnimatorStateOverride> _overrideDict = new Dictionary<string, AnimatorStateOverride>();

        private readonly Dictionary<AnimatorTransfer, AnimatorTransferBehaviour> _transferBehaviours = new Dictionary<AnimatorTransfer, AnimatorTransferBehaviour>();
        
        private readonly List<AnimatorStateBehaviour> _additives = new List<AnimatorStateBehaviour>();
        
        private readonly Dictionary<string, AnimatorParameter<float>> _floatDict = new Dictionary<string, AnimatorParameter<float>>();
        private readonly Dictionary<string, AnimatorParameter<int>> _intDict = new Dictionary<string, AnimatorParameter<int>>();
        private readonly Dictionary<string, AnimatorParameter<bool>> _boolDict = new Dictionary<string, AnimatorParameter<bool>>();

        private bool _isParamsDirty;
        

        public AnimatorRunner(AnimatorData data, AnimatorDataOverride dataOverride, Animator animator)
        {
            Data = data;
            Animator = animator;
            DataOverride = dataOverride;
        }

        public void Init()
        {
            Data.Init();
            Graph = PlayableGraph.Create();
            Handler = new AnimationHandler(Graph, Animator);

            RefreshUpdateMode();
            
            _stateDict.Clear();
            _behaviourDict.Clear();
            _transferBehaviours.Clear();
            
            Data.states.ForEach(s =>
            {
                _stateDict.Add(s.StateName, s);
            });

            if (DataOverride)
            {
                DataOverride.overrides.ForEach(o =>
                {
                    if (o.enabled && _stateDict.ContainsKey(o.stateName))
                    {
                        _overrideDict[o.stateName] = o;
                    }
                });
            }
            
            _floatDict.Clear();
            _intDict.Clear();
            _boolDict.Clear();
            foreach (var p in Data.parameters)
            {
                var t = p.obj.T;
                if (t == typeof(float)) _floatDict.Add(p.name, p.GetParameter<float>());
                else if (t == typeof(int)) _intDict.Add(p.name, p.GetParameter<int>());
                else if (t == typeof(bool)) _boolDict.Add(p.name, p.GetParameter<bool>());
            }

            Play(Data.enterState.StateName);
        }

        public void Destroy()
        {
            Graph.Stop();
            Graph.Destroy();
        }

        public AnimatorStateOverride GetOverride(string stateName)
        {
            _overrideDict.TryGetValue(stateName, out var o);
            return o;
        }

        public T GetOverride<T>(string stateName) where T : AnimatorStateOverride
        {
            return GetOverride(stateName) as T;
        }

        public AnimatorState GetState(string stateName)
        {
            _stateDict.TryGetValue(stateName, out var s);
            return s;
        }

        public T GetState<T>(string stateName) where T : AnimatorState
        {
            return GetState(stateName) as T;
        }

        public AnimatorStateBehaviour GetBehaviour(string name)
        {
            if (name == null || !_stateDict.ContainsKey(name)) return null;

            if (!_behaviourDict.ContainsKey(name))
            {
                _behaviourDict[name] = _stateDict[name].CreateBehaviour(this);
            }

            return _behaviourDict[name];
        }
        
        public T GetBehaviour<T>(string name) where T : AnimatorStateBehaviour
        {
            return GetBehaviour(name) as T;
        }

        public AnimatorTransferBehaviour GetTransferBehaviour(AnimatorTransfer transfer)
        {
            return GetTransferBehaviour<AnimatorTransferBehaviour>(transfer);
        }
        
        public T GetTransferBehaviour<T>(AnimatorTransfer transfer) where T : AnimatorTransferBehaviour
        {
            if (transfer == null) return null;

            if (!_transferBehaviours.ContainsKey(transfer))
            {
                _transferBehaviours[transfer] = transfer.CreateBehaviour(this);
            }

            return _transferBehaviours[transfer] as T;
        }

        public void Play(string name)
        {
            if (CurrentStateName != null && CurrentStateName == name || !_stateDict.ContainsKey(name))
            {
                return;
            }
            
            CurrentBehaviour?.OnExit();
            
            CurrentStateName = name;
            
            CurrentBehaviour?.OnEnter();
            
            onPlayState?.Invoke(name);
        }

        public void PlayAdditive(string name)
        {
            var ad = GetBehaviour(name);
            if (ad == null) return;
            if (!_additives.Contains(ad))
            {
                _additives.Add(ad);
            }
            ad.OnEnter();
            onPlayState?.Invoke(name);
        }

        public void StopAddtive(string name)
        {
            var ad = GetBehaviour(name);
            if (ad == null) return;
            if (_additives.Contains(ad))
            {
                _additives.Remove(ad);
            }
            ad.OnExit();
            onStopState?.Invoke(name);
        }

        public bool IsAdditive(string name)
        {
            var ad = GetBehaviour(name);
            return ad != null && _additives.Contains(ad);
        }

        public void OnUpdate()
        {
            if (_isParamsDirty)
            {
                TryTransfer();
            }
            CurrentTransferBehaviour?.OnUpdate();
            CurrentBehaviour?.OnUpdate();
            _additives.ForEach(a => { a.OnUpdate(); });
        }
        
        public void TryTransfer()
        {
            _isParamsDirty = false;
            foreach (var t in CurrentState.transfers)
            {
                if (IsTransferValid(t))
                {
                    Transfer(t);
                }
            }
        }

        public bool IsTransferValid(AnimatorTransfer t)
        {
            if (t.groups.Count == 0)
            {
                return true;
            }
            foreach (var g in t.groups)
            {
                if (!CheckCondition(g.conditionFloats, _floatDict) || 
                    !CheckCondition(g.conditionInts, _intDict) || 
                    !CheckCondition(g.conditionBools, _boolDict))
                {
                    continue;
                }
                return true;
            }

            return false;
        }
        
        public void Transfer(AnimatorTransfer t)
        {
            CurrentTransferBehaviour?.OnExit();
                
            CurrentStateName = t.to.StateName;
            CurrentTransfer = t;
            
            CurrentTransferBehaviour?.OnEnter();
            onTransfer?.Invoke(t);
        }
        
        public void SetInt(string name, int value)
        {
            SetValue(_intDict, name, value);
        }

        public void SetFloat(string name, float value)
        {
            SetValue(_floatDict, name, value);
        }

        public void SetBool(string name, bool value)
        {
            SetValue(_boolDict, name, value);
        }

        public void SetTrigger(string name)
        {
            SetValue(_boolDict, name, true);
            TryTransfer();
            SetValue(_boolDict, name, false);
        }
        
        private void SetValue<T>(Dictionary<string, AnimatorParameter<T>> dict, string name, T value) 
            where T : IComparable
        {
            if (!dict.ContainsKey(name)) return;
            var p = dict[name];
            if (!p.value.Equals(value))
            {
                p.value = value;
                dict[name] = p;
                _isParamsDirty = true;
            }
        }
        
        private void RefreshUpdateMode()
        {
            Graph.SetTimeUpdateMode(_updateMode);
        }

        private bool CheckCondition<T>(
            IEnumerable<AnimatorCondition<T>> conditions, 
            Dictionary<string, AnimatorParameter<T>> param) where T : IComparable
        {
            foreach (var c in conditions)
            {
                var res = param.TryGetValue(c.parameterName, out var p);
                if (!res || !c.CheckValid(p.value))
                {
                    return false;
                }
            }
            return true;
        }


    }
}