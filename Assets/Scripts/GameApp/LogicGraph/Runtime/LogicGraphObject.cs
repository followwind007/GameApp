using System;
using System.Collections.Generic;
using GameApp.DataBinder;
using UnityEngine;

namespace GameApp.LogicGraph
{
    [Serializable]
    public class GraphProperty
    {
        [Serializable]
        public enum Scope
        {
            Graph,
        }

        public string name;
        public string type;
        public bool isListen;
        public bool isTrigger;
        public Scope scope;
    }
    
    public class LogicGraphObject : ScriptableObject
    {
        [Serializable]
        public enum GraphType
        {
            Flow, State
        }

        public string exportPath;

        public GraphType type = GraphType.Flow;

        public bool useAsset = true;
        
        public Vector3 lastClosePosition;
        public Vector3 lastCloseScale = new Vector3(1,1,1);
        
        public bool IsStateGraph => type == GraphType.State;

        public LogicGraphData logicGraphData = new LogicGraphData();
        
        [HideInInspector]
        public BindableValues binds = new BindableValues();

        public List<GraphProperty> properties = new List<GraphProperty>();
        public string GraphId => name.Replace(".LogicGraph", "");

    }
}