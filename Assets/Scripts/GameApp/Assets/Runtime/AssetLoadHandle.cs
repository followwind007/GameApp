using System;
using System.Threading.Tasks;

namespace GameApp.Assets
{
    public struct AssetLoadHandle<TObject>
    {
        private AsyncOperationBase _operation;

        public AsyncOperationBase Operation
        {
            get
            {
                if (_operation == null)
                {
                    throw new Exception("Can not find any valid operation!");
                }

                return _operation;
            }
        }

        public TObject Result => (TObject)Operation.Result;

        public Task<TObject> Task => Operation.GetTask<TObject>();

        public event Action OnComplete
        {
            add => Operation.OnComplete += value;
            remove => Operation.OnComplete -= value;
        }

        public AssetLoadHandle(AsyncOperationBase operation)
        {
            _operation = operation;
            Operation.Increase();
        }

        public void Release()
        {
            Operation.Decrease();
            _operation = null;
        }
    }
}