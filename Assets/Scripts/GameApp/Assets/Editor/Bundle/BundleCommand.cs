using UnityEditor;
using UnityEngine;

namespace GameApp.Assets
{
    public abstract class BundleCommand : ScriptableObject
    {
        public class BundleReport
        {
            public BuildTarget target;
        }
        
        public abstract void Execute(BundleReport report);
    }
}