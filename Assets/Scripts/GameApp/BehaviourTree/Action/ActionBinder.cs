using System.Collections.Generic;
using GameApp.DataBinder;
using LuaInterface;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskDescription("ActionBinder support lua action")]
    [TaskIcon("{SkinColor}LogIcon.png")]
    public class ActionBinder : Action
    {
        public enum MethodType
        {
            OnAwake = 0,
            OnStart = 1,
            OnUpdate = 2,
            OnPause = 3,
            OnBehaviorComplete = 4,
            OnEnd = 5,
            OnBehaviorRestart = 6,
            OnReset = 7,
        }

        public const string PathKey = "_path";
        
        public readonly BindableValues vals = new BindableValues();
        
        public HashSet<int> Methods { get; set; }
        
        public BindableValues Vals => vals;
        public string LuaPath => vals[PathKey] as string;
        public List<LuaPath> Interfaces { get; } = new List<LuaPath>();

        private string _uniqId;

        private static LuaFunction _callFunc;
        private static LuaFunction CallFunc
        {
            get
            {
                if (_callFunc == null)
                {
                    _callFunc = BindableTargetImpl.State?.GetFunction("BTAdapter.Call");
                }

                return _callFunc;
            }
        }

        private static LuaFunction _initFunc;

        private static LuaFunction InitFunc
        {
            get
            {
                if (_initFunc == null)
                {
                    _initFunc = BindableTargetImpl.State?.GetFunction("Adapter.Init");
                }

                return _initFunc;
            }
        }

        public string GetInstanceID()
        {
            return _uniqId;
        }

        public override void OnAwake()
        {
            Init();
        }

        public void Init()
        {
            _uniqId = $"BT_{GetHashCode()}_{ID}";
            var path = BindableTargetImpl.GetLuaPath(LuaPath);
            InitFunc?.Call(this, path);
            CallFunc?.Call(this, (int)MethodType.OnAwake);
        }

        public override void OnStart()
        {
            CallFunc?.Call(this, (int)MethodType.OnStart);
        }

        public override TaskStatus OnUpdate()
        {
            if (CallFunc != null)
            {
                return (TaskStatus)CallFunc.Invoke<ActionBinder, int, int>(this, (int)MethodType.OnUpdate);
            }
            return TaskStatus.Success;
        }

        public override void OnPause(bool paused)
        {
            CallFunc?.Call(this, (int)MethodType.OnPause, paused);
        }

        public override void OnBehaviorComplete()
        {
            CallFunc?.Call(this, (int)MethodType.OnBehaviorComplete);
        }

        public override void OnEnd()
        {
            CallFunc?.Call(this, (int)MethodType.OnEnd);
        }

        public override void OnBehaviorRestart()
        {
            CallFunc?.Call(this, (int)MethodType.OnBehaviorRestart);
        }

        public override void OnReset()
        {
            CallFunc?.Call(this, (int)MethodType.OnReset);
        }

        
    }
}