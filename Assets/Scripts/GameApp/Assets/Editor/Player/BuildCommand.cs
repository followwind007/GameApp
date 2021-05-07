using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GameApp.Assets
{
    public abstract class BuildCommand : ScriptableObject
    {
        public abstract void Execute(BuildReport report);
    }
}