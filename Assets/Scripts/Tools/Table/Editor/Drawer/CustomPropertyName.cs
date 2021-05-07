using UnityEditor;
using UnityEngine;

namespace Tools.Table
{
    [System.Serializable]
    public class CustomPropertyNameAttribute : PropertyAttribute
    {
        public readonly string fieldInt;

        public CustomPropertyNameAttribute(string fieldInt)
        {
            this.fieldInt = fieldInt;
        }
    } 
    
    [CustomPropertyDrawer(typeof(CustomPropertyNameAttribute))]
    public class CustomPropertyName : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var attr = attribute as CustomPropertyNameAttribute;
            if (attr == null) return;

            var name = property.name;
            if (!string.IsNullOrEmpty(attr.fieldInt))
            {
                name = property.FindPropertyRelative(attr.fieldInt).intValue.ToString();
            }

            EditorGUI.PropertyField(position, property, new GUIContent(name), true);
            EditorGUI.EndProperty();
        }
    }
}