using System;
using UnityEngine;

namespace GameApp.LogicGraph
{
    [Serializable]
    public class LogicGroup
    {
        public Guid groupGuid;

        public string title;
        
        public Vector2 position;
        
        public LogicGroup(string title, Vector2 position)
        {
            
        }
        public LogicGroup() { }
    }
}