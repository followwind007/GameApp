using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomPropertyDrawer(typeof(AnimatorConditionHost))]
    public class AnimatorConditionHostDrawer : PropertyDrawer
    {
        public class ConditionView : VisualElement
        {
            private readonly SerializedProperty _property;
            
            public readonly TextField textName;
            
            public ConditionView(SerializedProperty property)
            {
                _property = property;

                style.flexDirection = FlexDirection.Row;
                style.marginTop = 2;
                style.marginBottom = 2;
                style.alignItems = Align.Center;

                var propName = property.FindPropertyRelative("parameterName");
                textName = new TextField("Parameter")
                {
                    value = propName.stringValue
                };
                {
                    textName.SetEnabled(false);
                    textName.style.width = 220;
                    textName.style.marginRight = 5;
                    textName.RegisterValueChangedCallback(e =>
                    {
                        propName.stringValue = e.newValue;
                        Save();
                    });
                }
                Add(textName);
                
                var propCompare = property.FindPropertyRelative("compare");
                var comp = new EnumField((AnimatorCompare) propCompare.intValue);
                {
                    comp.style.width = 90;
                }
                comp.BindProperty(propCompare);
                
                Add(comp);

                var propTarget = property.FindPropertyRelative("target");
                var targetField = new PropertyField(propTarget);
                Add(targetField);
                
                AddParamPop();
                
                var propLink = property.FindPropertyRelative("linkType");
                var linkField = new EnumField((AnimatorConditionLink) propLink.intValue);
                {
                    linkField.style.width = 50;
                }
                linkField.RegisterValueChangedCallback(e =>
                {
                    propLink.intValue = Convert.ToInt32(e.newValue);
                    Save();
                });
                Add(linkField);
            }

            private void AddParamPop()
            {
                var win = AnimatorWindow.Instance;
                if (win && win.Data && win.Data.parameters.Count > 0)
                {
                    var paramPop = new PopupField<AnimatorParameterHost>(win.Data.parameters, win.Data.parameters[0]);
                    paramPop.RegisterValueChangedCallback(e =>
                    {
                        textName.value = e.newValue.name;
                        SaveParameter(e.newValue);
                    });

                    paramPop.style.width = 80;

                    Add(paramPop);
                    
                    if (string.IsNullOrEmpty(textName.value))
                    {
                        var p0 = win.Data.parameters[0];
                        SaveParameter(p0);
                    }
                }
            }

            private void SaveParameter(AnimatorParameterHost host)
            {
                _property.FindPropertyRelative("parameterName").stringValue = host.name;
                var t = _property.FindPropertyRelative("target");
                t.FindPropertyRelative("type").stringValue = host.obj.type;
                Save();
            }

            private void Save()
            {
                _property.serializedObject.ApplyModifiedProperties();
            }
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new ConditionView(property);
        }
    }
}