using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorNode: Node
    {
        public AnimatorView View { get; }
        
        public AnimatorState State => (AnimatorState)userData;

        public readonly Image hintImg;

        public bool isRunning;

        private readonly VisualElement _border;

        public AnimatorNode(AnimatorView view, AnimatorState state)
        {
            this.AddStyleSheetPath("Animator/Styles/AnimatorNode");
            userData = state;
            View = view;

            SetPosition(new Rect(state.position, Vector2.zero));
            
            titleButtonContainer.Clear();

            var portInput = AnimatorPort.Create(this, Direction.Input);
            inputContainer.Add(portInput);

            var portOutput = AnimatorPort.Create(this, Direction.Output);
            outputContainer.Add(portOutput);

            var stateColor = AnimatorTypeUtil.GetStateColor(State);
            if (stateColor != null)
            {
                titleContainer.style.backgroundColor = new StyleColor(stateColor.node);
                portInput.portColor = stateColor.input;
                portOutput.portColor = stateColor.output;
            }

            hintImg = new Image {image = Resources.Load<Texture>("Animator/Icon/r_w")};
            {
                hintImg.style.marginTop = 5;
                hintImg.style.marginRight = 5;
                hintImg.style.width = 12;
                hintImg.style.height = 12;
            }
            titleContainer.Add(hintImg);

            RegisterCallback<MouseDownEvent>(e => { view.OnSelectObject(State); });
            
            _border = this.Q<VisualElement>("selection-border");
            
            Refresh();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is AnimatorNode n)
            {
                evt.menu.AppendAction("Set as default", action =>
                    {
                        View.Data.enterState = n.State;
                        View.RegisterUndo("set enter state");
                    }, 
                    n.State == View.Data.enterState ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
            }
            base.BuildContextualMenu(evt);
        }

        public void Refresh()
        {
            title = State.StateName;
            hintImg.visible = true;
            if (View.Data.enterState == State)
            {
                hintImg.tintColor = new Color(1f, 0.6f, 0f);
            }
            else
            {
                hintImg.visible = false;
            }

            var selColor = isRunning ? new Color(1f, 0.6f, 0f, 0.2f) : new Color(0, 0, 0, 0);
            _border.style.backgroundColor = new StyleColor(selColor);
        }

    }
}