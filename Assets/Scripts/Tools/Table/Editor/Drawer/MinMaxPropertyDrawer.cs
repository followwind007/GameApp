using UnityEngine;
using UnityEditor;
namespace Tools.Table
{ 
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public static readonly float LineHight = EditorGUIUtility.singleLineHeight + 2;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // cast the attribute to make life easier
            var minMax = (MinMaxAttribute)attribute;

            // This only works on a vector2! ignore on any other property type (we should probably draw an error message instead!)
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                // if we are flagged to draw in a special mode, lets modify the drawing rectangle to draw only one line at a time
                if (minMax.showDebugValues || minMax.showEditRange)
                {
                    position = new Rect(position.x, position.y, position.width, LineHight);
                }

                // pull out a bunch of helpful min/max values....
                var minValue = property.vector2Value.x; // the currently set minimum and maximum value
                var maxValue = property.vector2Value.y;
                var minLimit = minMax.minLimit; // the limit for both min and max, min cant go lower than minLimit and maax cant top maxLimit
                var maxLimit = minMax.maxLimit;

                // and ask unity to draw them all nice for us!
                EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);

                var vec = Vector2.zero; // save the results into the property!
                vec.x = minValue;
                vec.y = maxValue;

                property.vector2Value = vec;

                // Do we have a special mode flagged? time to draw lines!
                if (minMax.showDebugValues || minMax.showEditRange)
                {
                    var isEditable = minMax.showEditRange;

                    if (!isEditable)
                        GUI.enabled = false; // if were just in debug mode and not edit mode, make sure all the UI is read only!

                    // move the draw rect on by one line
                    position.y += LineHight;

                    var val = new Vector4(minLimit, minValue, maxValue, maxLimit); 
                    // shove the values and limits into a vector4 and draw them all at once
                    val = EditorGUI.Vector4Field(position, "Values", val);

                    GUI.enabled = false; // the range part is always read only
                    position.y += LineHight;
                    EditorGUI.FloatField(position, "Range", maxValue-minValue);
                    GUI.enabled = true; // remember to make the UI editable again!

                    if (isEditable)
                    {
                        property.vector2Value = new Vector2(val.y,val.z); // save off any change to the value~
                    }
                }
            }
        }

        // this method lets unity know how big to draw the property. We need to override this because it could end up meing more than one line big
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var minMax = (MinMaxAttribute)attribute;

            // by default just return the standard line height
            var size = LineHight;
            
            // if we have a special mode, add two extra lines!
            if (minMax.showEditRange || minMax.showDebugValues)
            {
                size += LineHight * 2;
            }

            return size;
        }
    }
}