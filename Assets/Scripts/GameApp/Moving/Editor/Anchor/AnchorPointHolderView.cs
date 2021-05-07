using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.Moving
{
    public class AnchorPointHolderView : VisualElement
    {
        public AnchorPointHolderView(SerializedObject serializedObject)
        {
            var propPoint = new PropertyField(serializedObject.FindProperty(AnchorPointHolder.PropPoint));
            propPoint.SetEnabled(false);
        }
    }
    
    [CustomEditor(typeof(AnchorPointHolder))]
    public class AnchorPointHolderEditor : Editor
    {
        private AnchorPointHolder _holder;
        private AnchorSetHolder _setHolder;
        
        public override VisualElement CreateInspectorGUI()
        {
            return new AnchorPointHolderView(serializedObject);
        }

        private void OnEnable()
        {
            _holder = (AnchorPointHolder) target;
            _setHolder = _holder.transform.parent.GetComponent<AnchorSetHolder>();
        }

        private void OnDisable()
        {
            TryPolish();
        }

        private void OnSceneGUI()
        {
            var trans = _holder.transform;
            var point = _holder.point;
            
            var idx = trans.GetSiblingIndex();
            var p = _setHolder.set.Points[idx];
            
            if (trans.localPosition != point.position)
            {
                point.position = trans.localPosition;
                p.position = trans.localPosition;
                _setHolder.set.Points[idx] = p;
                serializedObject.ApplyModifiedProperties();
            }

            if (trans.localRotation != point.rotation)
            {
                point.rotation = trans.rotation;
                p.rotation = trans.localRotation;
                _setHolder.set.Points[idx] = p;
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void TryPolish()
        {
            var sel = Selection.activeObject as GameObject;
            if (sel == null || _holder == null) return;
            
            var isParentSelected = sel.transform == _holder.transform.parent;
            var isSiblingSelected = sel.transform.parent == _holder.transform.parent;
            
            if (isParentSelected || isSiblingSelected) return;
            
            AnchorSetHolderEditor.PolishChilds(_holder.transform.parent);
        }
    }
}