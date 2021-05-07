using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorPort : Port
    {
        public static AnimatorPort Create(AnimatorNode node, Direction direction)
        {
            var port = new AnimatorPort(Orientation.Horizontal, direction, Capacity.Multi, null);
            var connector = new EdgeConnector<Edge>(node.View.ConnectorListener);
            port.AddManipulator(connector);
            return port;
        }
        
        protected AnimatorPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : 
            base(portOrientation, portDirection, portCapacity, type)
        {
            this.AddStyleSheetPath("Animator/Styles/AnimatorPort");
        }
        
    }
}