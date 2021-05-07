using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.Moving
{
    public class AnchorSetHolderView : VisualElement
    {
        public AnchorSetHolderView(SerializedObject serializedObject)
        {
            Add(new PropertyField(serializedObject.FindProperty(AnchorSetHolder.PropSet)));
        }
    }
    
    [CustomEditor(typeof(AnchorSetHolder))]
    public class AnchorSetHolderEditor : Editor
    {
        private SerializedProperty _propPoints;
        private AnchorSetHolder _holder;

        public override VisualElement CreateInspectorGUI()
        {
            return new AnchorSetHolderView(serializedObject);
        }

        public static void PolishChilds(Transform trans)
        {
            var c = trans.childCount;
            for (var i = 0; i < c; i++)
            {
                var go = trans.GetChild(0).gameObject;
                DestroyImmediate(go);
            }
        }

        private void OnEnable()
        {
            _holder = (AnchorSetHolder) target;
            var propSet = serializedObject.FindProperty(AnchorSetHolder.PropSet);
            _propPoints = propSet.FindPropertyRelative(AnchorSet.PropPoints);
        }
        
        private void OnDisable()
        {
            TryPolish();
        }

        private void OnSceneGUI()
        {
            if (_propPoints == null) return;
            
            var trans = _holder.transform;
            var count = _propPoints.arraySize;

            var gap = trans.childCount - count;
            for (var i = 0; i < Math.Abs(gap); i++)
            {
                if (gap < 0)
                {
                    var go = new GameObject {hideFlags = HideFlags.DontSave};
                    go.transform.SetParent(trans);
                    go.transform.localPosition = Vector3.zero;
                    go.AddComponent<AnchorPointHolder>();
                }
                else if (gap > 0)
                {
                    DestroyImmediate(trans.GetChild(0).gameObject);
                }
            }
            
            for (var i = 0; i < count; i++)
            {
                var t = trans.GetChild(i);
                t.gameObject.name = $"Point {i}";

                var p = _holder.set.Points[i];
                t.localPosition = p.position;
                t.localRotation = p.rotation;
            }
        }

        private void TryPolish()
        {
            var sel = Selection.activeObject as GameObject;
            if (sel != null && sel.transform.parent == _holder.transform) return;

            if (_holder)
            {
                PolishChilds(_holder.transform);
            }
        }

    }
}