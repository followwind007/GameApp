using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.Util
{
    public class ReorderableListView : VisualElement
    {
        public readonly SerializedProperty property;
        public readonly VisualElement container;

        public int Selected { get; private set; } = -1;

        public GenericMenu addMenu;

        public ReorderableListView(SerializedProperty property)
        {
            this.property = property;
            
            AddToClassList("ReorderableListView");
            this.AddStyleSheetPath("Styles/CommonStyle");
            this.AddStyleSheetPath("Styles/ReorderableListView");
            
            var listContainer = new VisualElement { name = "listContainer" };
            Add(listContainer);
            
            var head = new VisualElement { name = "head" };
            listContainer.Add(head);
            
            var labelHead = new Label(property.displayName) { name = "labelHead" };
            head.Add(labelHead);
            
            container = new VisualElement { name = "container" };
            listContainer.Add(container);
            
            if (!property.isArray)
            {
                Debug.LogError("can only create reorderble list for list");
                return;
            }

            var horEnd = new VisualElement { name = "horEnd" };
            Add(horEnd);

            var btns = new VisualElement { name = "btns" };
            horEnd.Add(btns);

            var btnAdd = new Button(OnClickAdd) { name = "btnAdd", text = "+" };
            btnAdd.AddToClassList("RlBtnSmall");
            btns.Add(btnAdd);
            
            var btnRemove = new Button(OnClickRemove) { name = "btnRemove", text = "-"};
            btnRemove.AddToClassList("RlBtnSmall");
            btns.Add(btnRemove);
            
            Refresh();
        }

        public void Refresh()
        {
            var elements = container.Children().OfType<ReorderableListElement>().ToList();
            foreach (var element in elements) container.Remove(element);
            
            for (var i = 0; i < property.arraySize; i++) AddElementAtIndex(i);
        }

        private void OnClickAdd()
        {
            if (addMenu != null)
            {
                addMenu.ShowAsContext();
                return;
            }
            property.arraySize++;
            property.serializedObject.ApplyModifiedProperties();
            AddElementAtIndex(property.arraySize - 1);
        }

        private void OnClickRemove()
        {
            if (Selected < 0) return;
            
            property.DeleteArrayElementAtIndex(Selected);
            property.serializedObject.ApplyModifiedProperties();
            container.RemoveAt(Selected);
            
            Selected = property.arraySize > 0 ? Mathf.Clamp(Selected, 0, property.arraySize - 1) : -1;
            if (Selected >= 0)
            {
                var elements = container.Children().OfType<ReorderableListElement>().ToList();
                for (var i = 0; i < elements.Count; i++)
                {
                    elements[i].Selected = i == Selected;
                }
            }
        }

        private void AddElementAtIndex(int index)
        {
            var p = property.GetArrayElementAtIndex(index);
            var element = new ReorderableListElement(p)
            {
                onSelect = OnSelect
            };
            container.Add(element);
            container.MarkDirtyRepaint();
        }

        private void OnSelect(SerializedProperty p)
        {
            var es = container.Children().OfType<ReorderableListElement>().ToList();
            for (var i = 0; i < es.Count; i++)
            {
                var element = es[i];
                if (element.property == p)
                {
                    Selected = i;
                }
                else if (element.Selected)
                {
                    element.Selected = false;
                }
            }
        }
        
    }
}