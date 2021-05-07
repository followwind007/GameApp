using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public static class AnimatorDataOverrideModify
    {
        public static void InternalCreateAllOverride(this AnimatorDataOverride dataOverride)
        {
            if (dataOverride.animator == null) return;
            Undo.RegisterCompleteObjectUndo(dataOverride, "create all override");
            foreach (var s in dataOverride.animator.states)
            {
                if (dataOverride.overrides.Any(od => od.stateName == s.StateName))
                {
                    continue;
                }
                dataOverride.InternalCreateOverride(s);
            }
        }

        public static AnimatorStateOverride InternalCreateOverride(this AnimatorDataOverride dataOverride, AnimatorState state)
        {
            var t = AnimatorTypeUtil.GetStateOverrideType(state);
            if (t != null)
            {
                var o = (AnimatorStateOverride) ScriptableObject.CreateInstance(t);
                o.stateName = state.StateName;
                dataOverride.overrides.Add(o);
                Undo.RegisterCreatedObjectUndo(o, "create override");
                AssetDatabase.AddObjectToAsset(o, dataOverride);
                return o;
            }
            return null;
        }

        public static void InternalRemoveAllOverride(this AnimatorDataOverride dataOverride)
        {
            Undo.RegisterCompleteObjectUndo(dataOverride, "remove all overrides");
            dataOverride.overrides.ForEach(Undo.DestroyObjectImmediate);
            dataOverride.overrides.Clear();
        }

        public static void InternalCleanOverride(this AnimatorDataOverride dataOverride)
        {
            Undo.RegisterCompleteObjectUndo(dataOverride, "clean overrides");
            var list = new List<AnimatorStateOverride>();
            dataOverride.overrides.ForEach(o =>
            {
                if (dataOverride.animator == null || !o.enabled || dataOverride.animator.states.All(s => s.StateName != o.stateName))
                {
                    list.Add(o);
                }
            });
            list.ForEach(ro =>
            {
                dataOverride.overrides.Remove(ro);
                Undo.DestroyObjectImmediate(ro);
            });
        }
        
    }
}