using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomEditor(typeof(AnimatorTransfer), true)]
    public class AnimatorTransferEditor : Editor
    {
        public class TransferView : VisualElement
        {
            public TransferView(SerializedObject serializedObject)
            {
                Add(new PropertyField(serializedObject.FindProperty("conditions")));
            }
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            return new TransferView(serializedObject);
        }

    }
    
}