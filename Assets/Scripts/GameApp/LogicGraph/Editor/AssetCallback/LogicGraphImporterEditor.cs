using UnityEditor;
using UnityEditor.Callbacks;

using UnityEngine;

namespace GameApp.LogicGraph
{
	[CustomEditor(typeof(LogicGraphImporter))]
	public class LogicGraphImporterEditor : UnityEditor.AssetImporters.ScriptedImporterEditor
	{	
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}

		private static bool ShowGraphEditWindow(string path)
		{
			var guid = AssetDatabase.AssetPathToGUID(path);
			
			if (!path.EndsWith(LogicGraphImporter.NameEnds))
				return false;

			var foundWindow = false;
			foreach (var w in Resources.FindObjectsOfTypeAll<LogicGraphEditorWindow>())
			{
				if (w.SelectedGuid == guid)
				{
					foundWindow = true;
					w.Focus();
				}
			}

			if (!foundWindow)
			{
				var window = CreateInstance<LogicGraphEditorWindow>();
				window.Show();
				window.SetGraph(guid);
			}

			return true;
		}

		[MenuItem("Tools/Logic Graph", false, 102)]
		private static void ShowLogicGraph()
		{
			var window = CreateInstance<LogicGraphEditorWindow>();
			window.Show();
		}

		[OnOpenAsset(0)]
		public static bool OnOpenAsset(int instanceId, int line)
		{
			var obj = EditorUtility.InstanceIDToObject(instanceId);
			if (!(obj is LogicGraphObject)) return false;
			var path = AssetDatabase.GetAssetPath(instanceId);
			return ShowGraphEditWindow(path);
		}
	}
}