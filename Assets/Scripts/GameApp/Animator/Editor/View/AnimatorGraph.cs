using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorGraph : GraphView
    {
        public AnimatorView View { get; }

        private AnimatorPrefs.AnimatorDataPref Pref => AnimatorPrefs.Instance.TryGetAnimatorPref(View.Data);

        public AnimatorGraph(AnimatorView view)
        {
            View = view;
            
            SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            viewTransformChanged = t =>
            {
                if (!View.Data) return;
                View.RegisterUndo("view transform change");
                Pref.position = viewTransform.position;
                Pref.scale = viewTransform.scale;
                //UnityEngine.Debug.Log($"{contentContainer.worldBound}");
            };
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var cp = new List<Port>();
            var startState = ((AnimatorNode) startPort.node).State;
            ports.ForEach(p =>
            {
                var targetState = ((AnimatorNode) p.node).State;
                var ts = AnimatorTypeUtil.GetTransfer(startState, targetState);
                if (ts.Count > 0 &&
                    startState != targetState &&
                    p.direction != startPort.direction && 
                    startState.transfers.All(t => t.to != targetState))
                {
                    cp.Add(p);
                }
            });
            return cp;
        }

        public Port GetFromPort(AnimatorTransfer transfer)
        {
            var from = nodes.ToList().OfType<AnimatorNode>().FirstOrDefault(n => n.State == transfer.from);
            return from?.outputContainer.Children().FirstOrDefault(t => t != null) as Port;
        }

        public Port GetToPort(AnimatorTransfer transfer)
        {
            var to = nodes.ToList().OfType<AnimatorNode>().FirstOrDefault(n => n.State == transfer.to);
            return to?.inputContainer.Children().FirstOrDefault(t => t != null) as Port;
        }

        public void UpdateViewTransform()
        {
            if (View.Data == null)
            {
                return;
            }
            UpdateViewTransform(Pref.position, Pref.scale);
        }

        public void Refresh()
        {
            foreach (var n in nodes.ToList().OfType<AnimatorNode>())
            {
                n.Refresh();
            }
        }

    }
}