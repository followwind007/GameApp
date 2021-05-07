using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class InspectorBoardProvider
    {
        private Object _currentAsset;
        public Object CurrentAsset
        {
            get
            {
                return _currentAsset;
            }
            set
            {
                _currentAsset = value;
                Refresh();
            }
        }

        public const string Title = "Inspector";

        public readonly AnimatorView view;
        public readonly Blackboard board;
        public readonly VisualElement container;

        public InspectorBoardProvider(AnimatorView view)
        {
            this.view = view;
            board = new Blackboard
            {
                scrollable = true,
                subTitle = Title
            };
            board.SetPosition(AnimatorPrefs.Instance.inspectorPos);

            container = new VisualElement();
            container.style.flexDirection = FlexDirection.Column;
            container.style.flexGrow = 1;
            board.Add(container);
            
            container.ClearClassList();
            container.styleSheets.Clear();
            container.AddToClassList("InspectorContainer");
            container.AddStyleSheetPath("Animator/Styles/InspectorContainer");
        }

        public void Refresh()
        {
            container.Clear();
            if (CurrentAsset == null) return;
            
            var inspector = new InspectorElement();
            inspector.Bind(new SerializedObject(CurrentAsset));
            container.Add(inspector);
        }


    }
}