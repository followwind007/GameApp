using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomEditor(typeof(ClipTransfer), true)]
    public class ClipTransferEditor : AnimatorTransferEditor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var view = base.CreateInspectorGUI();
            
            view.Add(new PropertyField(serializedObject.FindProperty("hasExitTime")));
            view.Add(new PropertyField(serializedObject.FindProperty("tweenDuration")));

            return view;
        }
    }
}