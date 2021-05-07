using System.Collections.Generic;

namespace GameApp.Assets
{
    public class GroupSequence : GroupOperation
    {
        public GroupSequence(List<AsyncOperationBase> operations) : base(operations) { }
        
        private int _curindex;
        
        public override void StartAsync()
        {
            base.StartAsync();
            _curindex = 0;
            errorCount = 0;

            Schedule();
        }

        private void Schedule()
        {
            var op = operations[_curindex];
            if (op.ShouldStart) op.StartAsync();
                
            op.OnComplete += OnOperationComplete;
        }

        private void OnOperationComplete()
        {
            var op = operations[_curindex];
            if (!op.IsSuccess) errorCount++;
            
            if (_curindex < operations.Count - 1)
            {
                _curindex++;
                Schedule();
            }
            else
            {
                if (errorCount <= 0)
                    OnSuccess();
                else
                    OnError();
            }
        }
        
    }
}