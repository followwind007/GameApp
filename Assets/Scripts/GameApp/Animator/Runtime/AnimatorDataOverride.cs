using System.Collections.Generic;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [CreateAssetMenu(fileName = "Animator Data Override", menuName = "Animator/Override", order = 202)]
    public class AnimatorDataOverride : ScriptableObject
    {
        public AnimatorData animator;

        public List<AnimatorStateOverride> overrides = new List<AnimatorStateOverride>();
    }
}