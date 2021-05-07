using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.UIBuilder
{
    public class ElementManipulator : MouseManipulator
    {
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            var elementView = (ElementView) target;
            var selected = AssetDatabase.LoadAssetAtPath<GameObject>(elementView.prefabPath);
            
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new Object[] { selected };
            DragAndDrop.StartDrag("Prefab Tool");
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            DragAndDrop.AcceptDrag();
        }

        
    }
}