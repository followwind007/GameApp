using UnityEditor.ProjectWindowCallback;
using UnityEditor;

namespace GameApp.LogicGraph
{
    public class CreateStateGraph : EndNameEditAction
    {
        [MenuItem("Assets/Create/Graph/State Graph", false, 209)]
        public static void CreateGraph()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists
                (0, CreateInstance<CreateStateGraph>(), string.Format("New State Graph.{0}.asset", LogicGraphImporter.Extension), null, null);
        }
        
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var graph = CreateInstance<LogicGraphObject>();
            graph.type = LogicGraphObject.GraphType.State;
            AssetDatabase.CreateAsset(graph, pathName);
            AssetDatabase.Refresh();
        }
    }
}