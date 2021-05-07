namespace GameApp.Assets
{
    public abstract class AppendOperation : AsyncOperationBase
    {
        protected AssetBundleProvider Provider => AssetBundleProvider.Instance;

        protected readonly AsyncOperationBase prevOperation;

        protected readonly string name;

        protected AppendOperation(string name, AsyncOperationBase prevOperation = null)
        {
            this.name = name;
            this.prevOperation = prevOperation;
        }

        public override void StartAsync()
        {
            if (IsRunning) return;
            base.StartAsync();

            if (prevOperation != null)
            {
                prevOperation.OnComplete += OnPrevComplete;
                if (prevOperation.ShouldStart)
                {
                    prevOperation.StartAsync();
                }
            }
            else
            {
                LoadAsset();
            }
        }

        private void OnPrevComplete()
        {
            if (prevOperation.IsSuccess)
            {
                LoadAsset();
            }
            else
            {
                OnError();
            }
        }

        protected virtual void LoadAsset()
        {
            
        }

        public override void Increase()
        {
            base.Increase();
            prevOperation?.Increase();
        }

        public override void Decrease()
        {
            base.Decrease();
            prevOperation?.Decrease();
        }
    }
}