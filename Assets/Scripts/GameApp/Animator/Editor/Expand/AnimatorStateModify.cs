using System;
using UnityEditor;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public static class AnimatorStateModify
    {
        public static T InternalAddTransfer<T>(this AnimatorState state, AnimatorState targetState) where T : AnimatorTransfer
        {
            return (T)InternalAddTransfer(state, targetState, typeof(T));
        }

        public static AnimatorTransfer InternalAddTransfer(this AnimatorState state, AnimatorState targetState, Type type)
        {
            Undo.RegisterCompleteObjectUndo(state, "add animator transfer");
            var transfer = (AnimatorTransfer)ScriptableObject.CreateInstance(type);
            Undo.RegisterCreatedObjectUndo(transfer, "create animator transfer");
            transfer.from = state;
            transfer.to = targetState;
            state.transfers.Add(transfer);
            
            AssetDatabase.AddObjectToAsset(transfer, state);
            return transfer;
        }

        public static void InternalRemoveTransfer(this AnimatorState state, AnimatorTransfer transfer)
        {
            Undo.RegisterCompleteObjectUndo(state, "aniamtor state remove transfer");
            if (!state.transfers.Contains(transfer))
            {
                Debug.LogError("can not remove transfer not exist!");
                return;
            }
            
            state.transfers.Remove(transfer);
            Undo.DestroyObjectImmediate(transfer);
        }
        
    }
}