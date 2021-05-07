#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GameApp.Timeline
{
    [CustomPropertyDrawer(typeof(AnimatorPlayableCommand), true)]
    public class AnimatorPlayableCommandDrawer : PropertyDrawer
    {
        private const string NAME = "name";
        private const string VALUE = "value";

        private readonly float _propHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            Color bfbgColor = GUI.backgroundColor;
            EditorGUI.BeginProperty(pos, label, property);

            pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
            pos.x -= 47;
            pos.width += 47;

            GUI.backgroundColor = ColorUtil.lightBlue;

            var typeProp = property.FindPropertyRelative("type");
            AnimatorPlayableCommand.CommandType type = (AnimatorPlayableCommand.CommandType)typeProp.intValue;
            Rect typeRect = new Rect(pos.x, pos.y, pos.width, _propHeight);
            type = (AnimatorPlayableCommand.CommandType)EditorGUI.EnumPopup(typeRect, "type", type);
            typeProp.intValue = (int)type;

            Rect nameRect = new Rect(pos.x, pos.y + _propHeight + 2, pos.width, _propHeight);
            var nameProp = property.FindPropertyRelative("paramName");
            nameProp.stringValue = EditorGUI.TextField(nameRect, NAME, nameProp.stringValue);

            Rect valueRect = new Rect(pos.x, pos.y + _propHeight * 2 + 4, pos.width, _propHeight);
            switch (type)
            {
                case AnimatorPlayableCommand.CommandType.SetFloat:
                    var valueFloat1Prop = property.FindPropertyRelative("valueFloat1");
                    valueFloat1Prop.floatValue = EditorGUI.FloatField(valueRect, VALUE, valueFloat1Prop.floatValue);
                    break;
                case AnimatorPlayableCommand.CommandType.SetBool:
                    var valueBool1Prop = property.FindPropertyRelative("valueBool1");
                    valueBool1Prop.boolValue = EditorGUI.Toggle(valueRect, VALUE, valueBool1Prop.boolValue);
                    break;
                case AnimatorPlayableCommand.CommandType.SetInteger:
                    var valueInt1Prop = property.FindPropertyRelative("valueInt1");
                    valueInt1Prop.intValue = EditorGUI.IntField(valueRect, VALUE, valueInt1Prop.intValue);
                    break;
                default:
                    break;
            }
            GUI.backgroundColor = bfbgColor;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var typeProp = property.FindPropertyRelative("type");
            AnimatorPlayableCommand.CommandType type = (AnimatorPlayableCommand.CommandType)typeProp.intValue;
            if (type == AnimatorPlayableCommand.CommandType.SetFloat || 
                type == AnimatorPlayableCommand.CommandType.SetBool ||
                type == AnimatorPlayableCommand.CommandType.SetInteger)
            {
                return _propHeight * 3 + 10;
            }
            return _propHeight * 2 + 10;
        }
    }
}
#endif