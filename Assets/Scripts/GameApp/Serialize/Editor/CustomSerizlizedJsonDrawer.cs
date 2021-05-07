using System;

namespace GameApp.Serialize
{
    public class CustomSerizlizedJsonDrawer : Attribute
    {
        public readonly Type type;

        public CustomSerizlizedJsonDrawer(Type type)
        {
            this.type = type;
        }
    }
}