using System.Collections.Generic;
using UnityEngine;

namespace GameApp.Assets
{
    [CreateAssetMenu(fileName = "BuildPlayerSettings", menuName = "Custom/Build/BuildPlayerSettings", order = 0)]
    public class BuildPlayerSettings : ScriptableObject
    {
        public List<BuildCommand> preCommands;
        public List<BuildCommand> postSettings;
    }
}