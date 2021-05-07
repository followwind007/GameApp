using UnityEditor;
using UnityEngine;

namespace GameApp.DataBinder
{
    [CustomPropertyDrawer(typeof(BindableValues))]
    public class BindableValuesDrawer : PropertyDrawer
    {
        private readonly BindableValuesDrawerSlave _slave = new BindableValuesDrawerSlave();

        private static BindableValuesDrawer _instance;
        
        public static void SetBindableInfo(BindableInfo info)
        {
            _instance?._slave.SetBindableInfo(info);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
//            if (_instance == null) _instance = this;
            _instance = this;
            return _slave.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _slave.OnGui(position, property, label);
        }

    }
}