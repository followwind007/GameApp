using System.Collections.Generic;

namespace GameApp.Assets
{
    public class GroupParallel : GroupOperation
    {
        public GroupParallel(List<AsyncOperationBase> operations) : base(operations) { }
        
        private int _finishCount;
        
        public override void StartAsync()
        {
            base.StartAsync();
            _finishCount = 0;
            errorCount = 0;

            foreach (var op in operations)
            {
                if (op.ShouldStart)
                {
                    op.StartAsync();
                }

                op.OnComplete += () => { OnOperationComplete(op); };
            }
        }

        private void OnOperationComplete(AsyncOperationBase op)
        {
            if (!op.IsSuccess) errorCount++;
            _finishCount++;

            if (_finishCount >= operations.Count)
            {
                if (errorCount <= 0)
                    OnSuccess();
                else
                    OnError();
            }
        }

    }
}