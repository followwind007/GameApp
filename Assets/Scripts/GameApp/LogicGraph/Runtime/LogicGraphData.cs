using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.LogicGraph
{
	[Serializable]
	public class SerializedNode
	{
		public string type;
		public int count;
		public string json;

		public string ClassName
		{
			get
			{
				var last = type.LastIndexOf('.');
				return last >= 0 ? type.Substring(0, last) : null;
			}
		}
	}
	
	[Serializable]
	public class SerializedEdge
	{
		public string sGuid;
		public string sPort;
		public string tGuid;
		public string tPort;
	}

	[Serializable]
	public class SerializedGroup
	{
		public string json;
	}
	
	[Serializable]
	public class LogicGraphData 
	{		
		[SerializeField]
		public List<SerializedNode> serializedNodes = new List<SerializedNode>();
		
		[SerializeField]
		public List<SerializedEdge> serializedEdges = new List<SerializedEdge>();
		
		[SerializeField]
		public List<SerializedGroup> serializedGroups = new List<SerializedGroup>();

		public int GetNodeCount(string nodeType)
		{
			var count = 0;
			foreach (var node in serializedNodes)
			{
				if (node.type == nodeType && node.count > count)
				{
					count = node.count;
				}
			}
			return count + 1;
		}
	
	}
	
}