using UnityEditor.ProjectWindowCallback;
using UnityEditor;

namespace GameApp.LogicGraph
{
    public class CreateFlowGraph : EndNameEditAction
    {
        [MenuItem("Assets/Create/Graph/Flow Graph", false, 208)]
        public static void CreateGraph()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists
                (0, CreateInstance<CreateFlowGraph>(), string.Format("New Flow Graph.{0}.asset", LogicGraphImporter.Extension), null, null);
        }
        
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            var graph = CreateInstance<LogicGraphObject>();
            graph.type = LogicGraphObject.GraphType.Flow;

            AssetDatabase.CreateAsset(graph, pathName);
            AssetDatabase.Refresh();
        }
    }
}