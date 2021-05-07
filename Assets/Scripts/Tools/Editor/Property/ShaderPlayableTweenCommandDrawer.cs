#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace GameApp.Timeline
{
    [CustomPropertyDrawer(typeof(ShaderPlayableTweenCommand), true)]
    public class ShaderPlayableTweenCommandDrawer : ShaderPlayableCommandDrawer
    {
        public const string VALUE_CURVE = "curve";
        public const string VALUE_FACTOR = "curve factor";
        public const string COLOR_R = "color r";
        public const string COLOR_G = "color g";
        public const string COLOR_B = "color b";
        public const string COLOR_A = "color a";

        public override void DrawValueField(Rect pos, SerializedProperty property, ShaderCommand.CommandType type)
        {
            Rect useCurveRect = new Rect(pos.x, pos.y + propHeight * 2 + 4, pos.width, propHeight);
            var useCurveProp = property.FindPropertyRelative("useCurve");
            useCurveProp.boolValue = EditorGUI.Toggle(useCurveRect, "use curve", useCurveProp.boolValue);

            if (useCurveProp.boolValue)
            {
                DrawCurve(useCurveRect, property, type);
            }
            else
            {
                DrawValue(useCurveRect, property, type);
            }
        }

        private void DrawCurve(Rect rect, SerializedProperty property, ShaderCommand.CommandType type)
        {
            if (type == ShaderCommand.CommandType.SetColor)
            {
                rect.y += propHeight + 2;
                var valueCurve0Prop = property.FindPropertyRelative("valueCurve0");
                valueCurve0Prop.animationCurveValue =
                    EditorGUI.CurveField(rect, COLOR_R, valueCurve0Prop.animationCurveValue);

                rect.y += propHeight + 2;
                var valueCurve1Prop = property.FindPropertyRelative("valueCurve1");
                valueCurve1Prop.animationCurveValue =
                    EditorGUI.CurveField(rect, COLOR_G, valueCurve1Prop.animationCurveValue);

                rect.y += propHeight + 2;
                var valueCurve2Prop = property.FindPropertyRelative("valueCurve2");
                valueCurve2Prop.animationCurveValue =
                    EditorGUI.CurveField(rect, COLOR_B, valueCurve2Prop.animationCurveValue);

                rect.y += propHeight + 2;
                var valueCurve3Prop = property.FindPropertyRelative("valueCurve3");
                valueCurve3Prop.animationCurveValue =
                    EditorGUI.CurveField(rect, COLOR_A, valueCurve3Prop.animationCurveValue);
            }
            else
            {
                rect.y += propHeight + 2;
                var valueCurve0Prop = property.FindPropertyRelative("valueCurve0");
                valueCurve0Prop.animationCurveValue =
                    EditorGUI.CurveField(rect, VALUE_CURVE, valueCurve0Prop.animationCurveValue);

                rect.y += propHeight + 2;
                var valueFactorProp = property.FindPropertyRelative("valueFactor");
                valueFactorProp.floatValue = EditorGUI.FloatField(rect, VALUE_FACTOR, valueFactorProp.floatValue);
            }
        }

        private void DrawValue(Rect rect, SerializedProperty property, ShaderCommand.CommandType type)
        {
            rect.y += propHeight + 2;
            var useStartProp = property.FindPropertyRelative("useStartValue");
            useStartProp.boolValue = EditorGUI.Toggle(rect, "use start value", useStartProp.boolValue);

            rect.y += propHeight + 2;
            switch (type)
            {
                case ShaderCommand.CommandType.SetInt:
                    if (useStartProp.boolValue)
                    {
                        var valueInt0Prop = property.FindPropertyRelative("valueInt0");
                        valueInt0Prop.intValue = EditorGUI.IntField(rect, VALUE_START, valueInt0Prop.intValue);
                        rect.y += propHeight + 2;
                    }
                    var valueInt1Prop = property.FindPropertyRelative("valueInt1");
                    valueInt1Prop.intValue = EditorGUI.IntField(rect, VALUE_END, valueInt1Prop.intValue);
                    break;
                case ShaderCommand.CommandType.SetFloat:
                    if (useStartProp.boolValue)
                    {
                        var valueFloat0Prop = property.FindPropertyRelative("valueFloat0");
                        valueFloat0Prop.floatValue = EditorGUI.FloatField(rect, VALUE_START, valueFloat0Prop.floatValue);
                        rect.y += propHeight + 2;
                    }
                    var valueFloat1Prop = property.FindPropertyRelative("valueFloat1");
                    valueFloat1Prop.floatValue = EditorGUI.FloatField(rect, VALUE_END, valueFloat1Prop.floatValue);
                    break;
                case ShaderCommand.CommandType.SetColor:
                    if (useStartProp.boolValue)
                    {
                        var valueColor0Prop = property.FindPropertyRelative("valueColor0");
                        valueColor0Prop.colorValue = EditorGUI.ColorField(rect, VALUE_START, valueColor0Prop.colorValue);
                        rect.y += propHeight + 2;
                    }
                    var valueColor1Prop = property.FindPropertyRelative("valueColor1");
                    valueColor1Prop.colorValue = EditorGUI.ColorField(rect, VALUE_END, valueColor1Prop.colorValue);
                    break;
                default:
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var typeProp = property.FindPropertyRelative("type");
            ShaderCommand.CommandType type = (ShaderCommand.CommandType)typeProp.intValue;

            bool useStartValue = property.FindPropertyRelative("useStartValue").boolValue;
            bool useCurve = property.FindPropertyRelative("useCurve").boolValue;

            float offset = 0f;
            if (useCurve)
            {
                offset = propHeight * 2 + 4;
                if (type == ShaderCommand.CommandType.SetColor)
                {
                    offset += propHeight * 2 + 4;
                }
            }
            else
            {
                offset = propHeight * 2 + 4;
                if (useStartValue)
                {
                    offset += propHeight + 2;
                }
            }
            return propHeight * 3 + 10 + offset;
        }
    }
}
#endif