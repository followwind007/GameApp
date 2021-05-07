#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GameApp.ScenePlayable
{
    [CustomPropertyDrawer(typeof(SceneObjectFinder), true)]
    public class SceneObjectFinderDrawer : PropertyDrawer
    {
        public const string FIND_TYPE = "Find Type";
        public const string TAG_NAME = "Tag Name";
        public const string GAME_OBJECT_Path = "GameObject Path";

        public readonly float propHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, property);
            Color bfbgColor = GUI.backgroundColor;
            GUI.backgroundColor = ColorUtil.lightBlue;

            Rect rect = new Rect(pos.x, pos.y, pos.width, propHeight);

            var findTypeProp = property.FindPropertyRelative("findType");
            FindType findType = (FindType)findTypeProp.intValue;
            findType = (FindType)EditorGUI.EnumPopup(rect, FIND_TYPE, findType);
            findTypeProp.intValue = (int)findType;

            var valueString0Prop = property.FindPropertyRelative("valueString0");
            switch (findType)
            {
                case FindType.None:
                    break;
                case FindType.Tag:
                    rect.y += propHeight + 2;
                    valueString0Prop.stringValue = EditorGUI.TextField(rect, TAG_NAME, valueString0Prop.stringValue);
                    break;
                case FindType.Path:
                    rect.y += propHeight + 2;
                    valueString0Prop.stringValue = EditorGUI.TextField(rect, GAME_OBJECT_Path, valueString0Prop.stringValue);
                    break;
                default:
                    break;
            }

            GUI.backgroundColor = bfbgColor;
            rect.y += propHeight + 2;
            rect.height = propHeight * 2;
            EditorGUI.HelpBox(rect, "Target type is always \"GameObject\", use GetComponent to get detail information.", MessageType.Info);

            GUI.backgroundColor = bfbgColor;
            EditorGUI.EndProperty();
        }

        

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float offset = 0f;
            FindType findType = (FindType)property.FindPropertyRelative("findType").intValue;
            if (findType == FindType.Tag || findType == FindType.Path)
            {
                offset += propHeight + 2;
            }
            return propHeight * 3 + 6 + offset;
        }
    }
}
#endif