using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorEdge : Edge
    {
        public readonly AnimatorView view;
        public AnimatorEdge(AnimatorView view, AnimatorTransfer transfer)
        {
            this.view = view;
            RegisterCallback<MouseDownEvent>(e => { view.OnSelectObject(transfer); });
        }
    }
}