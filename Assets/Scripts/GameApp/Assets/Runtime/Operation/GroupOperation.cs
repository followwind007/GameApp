using System.Collections.Generic;

namespace GameApp.Assets
{
    public abstract class GroupOperation : AsyncOperationBase
    {
        protected readonly List<AsyncOperationBase> operations;
        public override bool ShouldStart => !IsRunning;
        
        protected int errorCount;

        protected GroupOperation(List<AsyncOperationBase> operations)
        {
            this.operations = operations;
        }

        public override void Increase()
        {
            foreach (var op in operations)
            {
                op.Increase();
            }
        }

        public override void Decrease()
        {
            foreach (var op in operations)
            {
                op.Decrease();
            }
        }

        public override void Release()
        {
            
        }
    }
}