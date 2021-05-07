using System;

namespace GameApp.AnimatorBehaviour
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomEditorProvider : Attribute
    {
        public readonly Type type;
        public CustomEditorProvider(Type t)
        {
            type = t;
        }
    }
}