using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.LogicGraph
{
    public class LogicGraphEditorWindow : EditorWindow
    {
        private LogicGraphObject _graphObject;
        private string Path => AssetDatabase.GUIDToAssetPath(SelectedGuid);

        public LogicGraphEditorView EditorView { get; private set; }
        public string SelectedGuid { get; private set; }

        public void SetGraph(string guid)
        {
            SelectedGuid = guid;
            if (string.IsNullOrEmpty(guid))
            {
                titleContent = new GUIContent("None");
                EditorView.SetGraphObject(null);
                return;
            }
            SetGraphObject();
        }

        public void UpdateAsset()
        {
            foreach (var slot in EditorView.GetInputPort())
            {
                slot.RemoveValue();
            }
            
            _graphObject.binds.Regenerate();
            var serializeNodes = _graphObject.logicGraphData.serializedNodes;
            serializeNodes.Clear();
            foreach (var node in EditorView.GetNodes())
            {
                node.NodeEditor.SerializedNode.json = JsonUtility.ToJson(node.NodeEditor);
                serializeNodes.Add(node.NodeEditor.SerializedNode);
            }
            EditorUtility.SetDirty(_graphObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void ResetAsset()
        {
            _graphObject.binds.wraps.Clear();
            _graphObject.properties.Clear();
            UpdateAsset();
        }

        public void ShowAsset()
        {
            Selection.activeObject = _graphObject;
        }

        public void ExportGraph()
        {
            var gen = _graphObject.type == LogicGraphObject.GraphType.Flow ? 
                new LuaGraphGenerator(Path) : new StateGraphGenerator(Path);
            gen.Generate();
        }
        
        private void Initialize(string guid)
        {
            SelectedGuid = guid;
            if (EditorView == null)
            {
                InitEditorView();
            }
            
            if (string.IsNullOrEmpty(guid)) return;
            SetGraphObject();
        }

        private void SetGraphObject()
        {
            _graphObject = AssetDatabase.LoadAssetAtPath<LogicGraphObject>(Path);
            _graphObject.binds.Init();

            titleContent = new GUIContent(_graphObject.name);
            EditorView.SetGraphObject(_graphObject);
            Repaint();
        }

        private void InitEditorView()
        {
            EditorView = new LogicGraphEditorView(this);
            EditorView.RegisterCallback<KeyDownEvent>(KeyDown);
            EditorView.RegisterDebug();
            
            rootVisualElement.Add(EditorView);
        }

        private void OnEnable()
        {
            Initialize(SelectedGuid);
            Undo.undoRedoPerformed += OndoRedoPerformed;
            LogicGraphPrefs.SavePrefs();
        }

        private void OnDisable()
        {
            LogicGraphPrefs.SavePrefs();
            if (EditorView != null)
            {
                rootVisualElement.Remove(EditorView);
            }
            
            // ReSharper disable once DelegateSubtraction
            Undo.undoRedoPerformed -= OndoRedoPerformed;
        }

        private void OndoRedoPerformed()
        {
            _graphObject.binds.Init();
            EditorView.SetGraphObject(_graphObject);
        }

        private void OnDestroy()
        {
            if (_graphObject != null)
            {
                if (EditorView != null)
                {
                    UpdateAsset(); 
                    EditorView.SaveLogicGraphViewTransform();
                }

                Undo.ClearUndo(_graphObject);
            }

            EditorView?.DeregisterDebug();
            EditorView = null;
        }

        private void KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.S && evt.ctrlKey)
            {
                UpdateAsset();
            }
        }
        
    }
}