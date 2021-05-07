using System;

namespace GameApp.AnimatorBehaviour
{
    public class TransforCompatible : Attribute
    {
        public readonly Type from;
        public readonly Type to;

        public TransforCompatible(Type from, Type to)
        {
            this.from = from;
            this.to = to;
        }
    }
}