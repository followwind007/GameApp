using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.Util
{
    public class ReorderableListElement : VisualElement
    {
        public Action<SerializedProperty> onSelect;

        public readonly SerializedProperty property;
        
        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                RefreshSelected();
            }
        }

        public ReorderableListElement(SerializedProperty property)
        {
            this.AddStyleSheetPath("Styles/ReorderableListElement");
            AddToClassList("ReorderableListElement");
            this.property = property;
            RegisterCallback<MouseDownEvent>(e =>
            {
                if (!Selected)
                {
                    Selected = true;
                    onSelect?.Invoke(property);
                }
            });
            var prop = new PropertyField(property);
            prop.Bind(this.property.serializedObject);
            Add(prop);
            RefreshSelected();
        }

        private void RefreshSelected()
        {
            ClearClassList();
            AddToClassList(_selected ? "ReorderableListElement_Selected" : "ReorderableListElement");
        }
        
    }
}