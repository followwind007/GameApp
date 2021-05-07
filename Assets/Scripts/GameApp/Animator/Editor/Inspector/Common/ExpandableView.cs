using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class ExpandableView : VisualElement
    {
        public bool expanded;

        protected readonly VisualElement head = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Row
            }
        };

        protected readonly VisualElement detail = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Column
            }
        };

        protected ExpandableView()
        {
            style.marginTop = 2;
            style.marginBottom = 2;
            head.RegisterCallback<MouseDownEvent>(e =>
            {
                expanded = !expanded;
                Refresh();
            });
        }

        public void Refresh()
        {
            Clear();
            Add(head);
            if (expanded)
            {
                Add(detail);
            }
        }

        public void SetExpanded(bool isExpanded)
        {
            expanded = isExpanded;
            Refresh();
        }
        
    }
}