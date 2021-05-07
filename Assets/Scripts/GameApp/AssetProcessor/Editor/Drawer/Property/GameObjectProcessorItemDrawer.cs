using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GameApp.AssetProcessor.Drawer.Property
{
    [CustomPropertyDrawer(typeof(AssetProcessorSetting.GameObjectProcessorItem))]
    public class GameObjectProcessorItemDrawer : PropertyDrawer, ICheckerSelector
    {
        private float LineHeight => EditorGUIUtility.singleLineHeight + 2;
        private float FieldHeight => LineHeight - 2;

        private string _checkerIds;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return LineHeight * 1;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect1 = new Rect(position.x, position.y, position.width - LineHeight, FieldHeight);
            var prop1 = property.FindPropertyRelative("checkerIds");

            var rect12 = new Rect(rect1.x + rect1.width, rect1.y, LineHeight, FieldHeight);
            if (GUI.Button(rect12, ">"))
            {
                this.GetCheckerMenu(prop1, () => 
                    CompileEntry.GameObjectCheckers.Values.Select(item => item.attribute.id).ToList())
                    .ShowAsContext();
            }
            
            EditorGUI.BeginChangeCheck();
            _checkerIds = this.ArrayToString(prop1);
            _checkerIds = EditorGUI.TextField(rect1, "Checkers", _checkerIds);
            if (EditorGUI.EndChangeCheck()) this.StringToArray(prop1, _checkerIds);
        }
    }
}