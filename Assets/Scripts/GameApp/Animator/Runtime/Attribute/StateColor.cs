using System;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public class StateColor : Attribute
    {
        public readonly Color node;
        public readonly Color input;
        public readonly Color output;
        
        public StateColor(string node, string input, string output)
        {
            ColorUtility.TryParseHtmlString(node, out this.node);
            ColorUtility.TryParseHtmlString(input, out this.input);
            ColorUtility.TryParseHtmlString(output, out this.output);
        }
        
    }
}