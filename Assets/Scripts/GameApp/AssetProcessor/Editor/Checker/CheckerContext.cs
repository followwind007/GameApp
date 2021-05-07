using UnityEngine;

namespace GameApp.AssetProcessor
{
    public struct GameObjectCheckerContext
    {
        public Object sourceObject;
        public bool doFix;
    }
    
    public struct ComponentCheckerContext
    {
        public Component component;
        public Object sourceObject;
        public bool doFix;
    }
}