#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GameApp.Timeline
{
    [CustomPropertyDrawer(typeof(ShaderPlayableCommand), true)]
    public class ShaderPlayableCommandDrawer : PropertyDrawer
    {
        public const string NAME = "property name";
        public const string VALUE_START = "value start";
        public const string VALUE_END = "value end";
        public const string VALUE = "value";

        public readonly float propHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            Color bfbgColor = GUI.backgroundColor;
            EditorGUI.BeginProperty(pos, label, property);

            pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
            pos.x -= 47;
            pos.width += 47;

            GUI.backgroundColor = ColorUtil.lightBlue;

            var typeProp = property.FindPropertyRelative("type");
            ShaderCommand.CommandType type = (ShaderCommand.CommandType)typeProp.intValue;
            Rect typeRect = new Rect(pos.x, pos.y, pos.width, propHeight);
            type = (ShaderCommand.CommandType)EditorGUI.EnumPopup(typeRect, "type", type);
            typeProp.intValue = (int)type;

            Rect nameRect = new Rect(pos.x, pos.y + propHeight + 2, pos.width, propHeight);
            var nameProp = property.FindPropertyRelative("paramName");
            nameProp.stringValue = EditorGUI.TextField(nameRect, NAME, nameProp.stringValue);

            DrawValueField(pos, property, type);
            GUI.backgroundColor = bfbgColor;
            EditorGUI.EndProperty();
        }

        public virtual void DrawValueField(Rect pos, SerializedProperty property, ShaderCommand.CommandType type)
        {
            Rect valueRect = new Rect(pos.x, pos.y + propHeight * 2 + 4, pos.width, propHeight);
            switch (type)
            {
                case ShaderCommand.CommandType.SetInt:
                    var valueInt1Prop = property.FindPropertyRelative("valueInt1");
                    valueInt1Prop.intValue = EditorGUI.IntField(valueRect, VALUE, valueInt1Prop.intValue);
                    break;
                case ShaderCommand.CommandType.SetFloat:
                    var valueFloat1Prop = property.FindPropertyRelative("valueFloat1");
                    valueFloat1Prop.floatValue = EditorGUI.FloatField(valueRect, VALUE, valueFloat1Prop.floatValue);
                    break;
                case ShaderCommand.CommandType.SetColor:
                    var valueColor1Prop = property.FindPropertyRelative("valueColor1");
                    valueColor1Prop.colorValue = EditorGUI.ColorField(valueRect, VALUE, valueColor1Prop.colorValue);
                    break;
                default:
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return propHeight * 3 + 10;
        }
    }
}
#endif