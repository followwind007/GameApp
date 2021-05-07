using GameApp.DataBinder;
using LuaInterface;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class LogicGraphToolbar
    {
        private const string NoneFlag = "None";
        
        public IMGUIContainer toolbar;
        public LogicGraphEditorWindow EditorWindow { get; set; }
        public LogicGraphEditorView EditorView { get; set; }

        private string _selected = NoneFlag;
        

        public void Init()
        {
            toolbar = new IMGUIContainer(() =>
            {
                EditorGUI.BeginDisabledGroup(EditorView.GraphObject == null);
                
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                    EditorWindow.UpdateAsset();
                
                GUILayout.Space(6);
                if (GUILayout.Button("Export", EditorStyles.toolbarButton, GUILayout.Width(50)))
                    EditorWindow.ExportGraph();

                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                    EditorWindow.ShowAsset();
                
                GUILayout.Space(6);
                if (GUILayout.Button("Reset", EditorStyles.toolbarButton, GUILayout.Width(40)))
                    EditorWindow.ResetAsset();
                
                EditorGUI.EndDisabledGroup();
                
                GUILayout.Space(6);
                EditorGUI.BeginChangeCheck();
                LogicGraphPrefs.Instance.isDebug = GUILayout.Toggle(LogicGraphPrefs.Instance.isDebug, "Debug", EditorStyles.toolbarButton, GUILayout.Width(50));
                if (EditorGUI.EndChangeCheck()) Refresh();
                
                DrawDebug();

                GUILayout.FlexibleSpace();
                LogicGraphPrefs.Instance.autoActive = GUILayout.Toggle(LogicGraphPrefs.Instance.autoActive, "Auto Active", EditorStyles.toolbarButton, GUILayout.Width(70));
                GUILayout.EndHorizontal();
            });
        }

        public void Refresh()
        {
            
        }
        
        private void DrawDebug()
        {
            if (!LogicGraphPrefs.Instance.isDebug)
            {
                return;
            }

            if (Application.isPlaying)
            {
                GUILayout.Space(6);
                if (GUILayout.Button(_selected, EditorStyles.toolbarDropDown, GUILayout.Width(80)))
                {
                    var menu = GetGraphMenu();
                    menu.ShowAsContext();
                }
            }
        }
        
        private GenericMenu GetGraphMenu()
        {
            var gm = new GenericMenu();

            var state = BindableTargetImpl.State;
            var list = state.Invoke<LuaTable>("LogicGraphMgr.GetAllGraph", true);
            if (list == null)  return gm;
            
            var count = 0;
            gm.AddItem(new GUIContent($"<{NoneFlag}>"), false, () => OnSelectGraph(NoneFlag, null));
            foreach (var entry in list.ToDictTable())
            {
                count++;
                var tbl = entry.Value as LuaTable;
                if (tbl != null)
                {
                    var graphId = tbl["graphId"] as string;
                    gm.AddItem(new GUIContent($"{count}. {graphId}"), false, () => OnSelectGraph(graphId, tbl));
                }
            }
            return gm;
        }

        private void OnSelectGraph(string graphId, LuaTable tbl)
        {
            _selected = graphId;
            EditorView.ShowGraphAndDebug(graphId, tbl);
        }
        
    }
}