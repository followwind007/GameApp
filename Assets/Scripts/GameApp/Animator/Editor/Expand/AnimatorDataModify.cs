
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApp.Serialize;
using UnityEditor;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public static class AnimatorDataModify
    {
        public static T InternalCreateState<T>(this AnimatorData data) where T : AnimatorState
        {
            var s = data.InternalCreateState(typeof(T));
            return s as T;
        }

        public static AnimatorState InternalCreateState(this AnimatorData data, Type type)
        {
            var s = (AnimatorState)ScriptableObject.CreateInstance(type);
            s.stateName = GetNewStateName(data);
            
            Undo.RegisterCreatedObjectUndo(s, "create animator state");
            Undo.RegisterCompleteObjectUndo(data, "add animator state");
            data.states.Add(s);
            AssetDatabase.AddObjectToAsset(s, data);
            return s;
        }

        public static void InternalRemoveState(this AnimatorData data, AnimatorState state)
        {
            Undo.RegisterCompleteObjectUndo(data, "remove animator state");
            if (!data.states.Contains(state))
            {
                Debug.LogError("can not remove state not exist!");
                return;
            }
            data.states.Remove(state);
            
            //remove transfers those state is 'to'
            var rts = new List<AnimatorTransfer>();
            data.states.ForEach(s =>
            {
                var ts = s.transfers.Where(t => t.to == state);
                
                foreach (var t in ts)
                {
                    s.transfers.Remove(t);
                    rts.Add(t);
                }
            });
            rts.ForEach(Undo.DestroyObjectImmediate);
            
            //remove transfers those state is 'from'
            state.transfers.ForEach(Undo.DestroyObjectImmediate);
            
            state.transfers.Clear();
            
            Undo.DestroyObjectImmediate(state);
        }

        public static List<AnimatorTransfer> InternalGetTransfers(this AnimatorData data)
        {
            var ts = new List<AnimatorTransfer>();
            data.states.ForEach(s =>
            {
                s.transfers.ForEach(t => ts.Add(t));
            });
            return ts;
        }

        public static AnimatorParameterHost InternalCreateParameter(this AnimatorData data, Type type, object value = null)
        {
            Undo.RegisterCompleteObjectUndo(data, "create parameter");
            var host = new AnimatorParameterHost
            {
                name = GetNewParameterName(data),
                obj = new SerializedJsonObject(type, value)
            };
            data.parameters.Add(host);
            
            return host;
        }

        public static T InternalGetState<T>(this AnimatorData data, string stateName) where T : AnimatorState
        {
            return data.states.FirstOrDefault(s => s.StateName == stateName) as T;
        }

        public static bool InternalRemoveParameter(this AnimatorData data, int index)
        {
            if (index < 0 || index >= data.parameters.Count)
            {
                return false;
            }

            var paramName = data.parameters[index].name;

            var ss = data.states.Where(s =>
            {
                return s.transfers.Any(t => t.conditions.Any(c => c.parameterName == paramName));
            });
            var animatorStates = ss.ToList();
            
            if (animatorStates.Any())
            {
                var sb = new StringBuilder();
                animatorStates.ForEach(s => { sb.Append($"{s.StateName} "); });
                
                EditorUtility.DisplayDialog("Warning...", $"Can not delete parameter [ {paramName} ] \n[ {sb}] use it!", "OK");
                return false;
            }
            
            Undo.RegisterCompleteObjectUndo(data, "remove parameter");
            data.parameters.RemoveAt(index);
            
            return true;
        }

        private static string GetNewStateName(AnimatorData data)
        {
            var sn = "";
            for (var i = 0; i <= data.states.Count; i++)
            {
                sn = $"Animator State {i}";
                if (data.states.All(s => s.stateName != sn))
                {
                    break;
                }
            }

            return sn;
        }

        private static string GetNewParameterName(AnimatorData data)
        {
            var pn = "";
            for (var i = 0; i <= data.parameters.Count; i++)
            {
                pn = $"Parameter {i}";
                if (data.parameters.All(p => p.name != pn))
                {
                    break;
                }
            }
            
            return pn;
        }

    }
}