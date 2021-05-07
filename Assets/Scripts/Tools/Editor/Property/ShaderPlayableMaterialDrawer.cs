#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GameApp.Timeline
{
    [CustomPropertyDrawer(typeof(MaterialElement), true)]
    public class ShaderPlayableMaterialDrawer : PropertyDrawer
    {
        public const string NAME = "property name";
        public const string SELECTED = "selected";
        
        public const string RESET_MATERIAL = "reset";

        public readonly float propHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color bfbgColor = GUI.backgroundColor;
            EditorGUI.BeginProperty(position, label, property);

            GUI.backgroundColor = ColorUtil.lightBlue;
            var pos = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var rect = new Rect(pos.x - 47, pos.y, pos.width, propHeight);
            var selectedProp = property.FindPropertyRelative("selected");
            selectedProp.boolValue = EditorGUI.Toggle(rect, SELECTED, selectedProp.boolValue);

            if (selectedProp.boolValue)
            {
                rect.y += propHeight;
                var resetMaterialProp = property.FindPropertyRelative("resetMaterial");
                resetMaterialProp.boolValue = EditorGUI.Toggle(rect, RESET_MATERIAL, resetMaterialProp.boolValue);

                if (resetMaterialProp.boolValue)
                {
                    var materialPathProp = property.FindPropertyRelative("materialPath");
                    var mpRect = new Rect(position.x, rect.y + propHeight + 2, position.width, propHeight * 2);
                    EditorGUI.PropertyField(mpRect, materialPathProp);
                }

                GUI.backgroundColor = bfbgColor;
            }

            EditorGUI.EndProperty();
        }

 
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var selectedProp = property.FindPropertyRelative("selected");
            
            if (selectedProp.boolValue)
            {
                var resetMaterialProp = property.FindPropertyRelative("resetMaterial");
                if (resetMaterialProp.boolValue)
                {
                    return propHeight * 4 + 5;
                }
                else
                {
                    return propHeight * 2 + 5;
                }
            }
            else
            {
                return propHeight + 2;
            }
        }


    }
}
#endif