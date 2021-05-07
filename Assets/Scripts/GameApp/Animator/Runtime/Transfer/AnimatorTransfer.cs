using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [Serializable]
    public abstract class AnimatorTransfer : ScriptableObject
    {
        [HideInInspector]
        public AnimatorState from;
        [HideInInspector]
        public AnimatorState to;
        
        public List<AnimatorConditionHost> conditions = new List<AnimatorConditionHost>();

        [NonSerialized]
        public readonly List<AnimatorConditionGroup> groups = new List<AnimatorConditionGroup>();

        public void Init()
        {
            var list = new List<AnimatorConditionHost>();
            conditions.ForEach(c =>
            {
                if (c.linkType == AnimatorConditionLink.And)
                {
                    list.Add(c);
                }
                else if (c.linkType == AnimatorConditionLink.Or && list.Count > 0)
                {
                    groups.Add(new AnimatorConditionGroup(list));
                    list.Clear();
                    list.Add(c);
                }
            });
            if (list.Count > 0)
            {
                groups.Add(new AnimatorConditionGroup(list));
            }
        }

        public abstract AnimatorTransferBehaviour CreateBehaviour(AnimatorRunner runner);

    }
}