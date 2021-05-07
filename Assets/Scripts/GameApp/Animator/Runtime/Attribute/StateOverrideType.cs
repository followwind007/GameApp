using System;

namespace GameApp.AnimatorBehaviour
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StateOverrideType : Attribute
    {
        public readonly Type type;

        public StateOverrideType(Type type)
        {
            this.type = type;
        }
    }
}