using System;

namespace GameApp.AssetProcessor
{
    public class ComponentCheckerAttribute : Attribute
    {
        public readonly Type checkType;
        public readonly string id;
        
        public ComponentCheckerAttribute(Type type, string id)
        {
            checkType = type;
            this.id = id;
        }
    }
}