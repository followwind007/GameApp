using System;

namespace GameApp.AnimatorBehaviour
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomStateOverrideProvider : Attribute
    {
        public readonly Type target;

        public CustomStateOverrideProvider(Type target)
        {
            this.target = target;
        }
    }
}