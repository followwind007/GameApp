using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameApp.Assets
{
    public abstract class AsyncOperationBase
    {
        public enum ResultState
        {
            None,
            Success,
            Error
        }
        
        public int ReferenceCount
        {
            get { return referenceCount; }
            private set
            {
                referenceCount = value;
                if (referenceCount == 0)
                {
                    Release();
                }
            }
        }

        protected int referenceCount;

        public ResultState State { get; protected set; } = ResultState.None;

        public bool IsFinish => State != ResultState.None;

        public bool IsSuccess => State == ResultState.Success;
        public bool IsError => State == ResultState.Error;

        public bool IsRunning { get; private set; }

        public virtual bool ShouldStart => !IsRunning && !IsFinish;
        
        public bool HasResult => Result != null;

        public object Result { get; protected set; }

        private Action _onComplete;
        
        private EventWaitHandle _waitHandle;
        internal WaitHandle WaitHandle
        {
            get
            {
                if (_waitHandle == null) 
                    _waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
                _waitHandle.Reset();
                return _waitHandle;
            }
        }
        
        public event Action OnComplete
        {
            add
            {
                if (!IsRunning && IsFinish)
                {
                    value?.Invoke();
                    return;
                }
                _onComplete += value;
            }
            remove => _onComplete -= value;
        }
        
        public Task<TObject> GetTask<TObject>()
        {
            if (State == ResultState.Error)
                return Task.FromResult(default(TObject));
            
            if (State == ResultState.Success)
                return Task.FromResult((TObject)Result);

            var handle = WaitHandle;
            return Task.Factory.StartNew((o) =>
            {
                handle.WaitOne(); 
                return (TObject)Result;
            }, this);
        }

        public virtual void StartAsync()
        {
            IsRunning = true;
            State = ResultState.None;
        }

        public virtual void Increase()
        {
            ReferenceCount++;
        }

        public virtual void Decrease()
        {
            ReferenceCount--;
        }

        public virtual void Release()
        {
            Reset();
        }

        protected void OnSuccess()
        {
            if (State == ResultState.Success) return;
            State = ResultState.Success;
            OnFinish();
        }

        protected void OnError()
        {
            if (State == ResultState.Error) return;
            
            State = ResultState.Error;
            OnFinish();
        }

        protected void OnFinish()
        {
            IsRunning = false;
            _onComplete?.Invoke();
            if (_waitHandle != null) _waitHandle.Set();
            _onComplete = null;
        }

        protected void Reset()
        {
            IsRunning = false;
            State = ResultState.None;
            _onComplete = null;
            Result = null;
        }
    }
}