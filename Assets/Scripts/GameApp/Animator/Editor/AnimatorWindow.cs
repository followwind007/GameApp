using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    public class AnimatorWindow : EditorWindow
    {
        public static AnimatorWindow Instance { get; private set; }

        public AnimatorData Data { get; private set; }
        public AnimatorView View { get; private set; }

        private readonly AnimatorDebuger _debuger = new AnimatorDebuger();

        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceId);
            if (!path.EndsWith(".asset")) return false;
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (obj is AnimatorData data)
            {
                OpenWindow(data);
                return true;
            }
            return false;
        }

        public static void OpenWindow(AnimatorData data)
        {
            if (Instance == null)
            {
                Instance = GetWindow<AnimatorWindow>();
            }
            Instance.Focus();
            Instance.RefreshData(data);
        }

        private void Init()
        {
            if (View != null) return;
            View = new AnimatorView();
            rootVisualElement.Add(View);
            RefreshData(Data);
        }

        private void OnEnable()
        {
            Init();
            Undo.undoRedoPerformed += UndoRedoPerformed;
            Selection.selectionChanged += OnSelectionChange;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Selection.selectionChanged -= OnSelectionChange;
        }

        private void Update()
        {
            View?.Graph?.Refresh();
            _debuger.OnUpdate();
        }

        private void OnDestroy()
        {
            AnimatorPrefs.SavePrefs();
            if (Data)
            {
                EditorUtility.SetDirty(Data);
                AssetDatabase.SaveAssets();
            }
        }

        public void RefreshData(AnimatorData data)
        {
            AnimatorPrefs.SavePrefs();
            Data = data;
            var dataName = Data ? Data.name : "null";
            titleContent = new GUIContent($"Animator-{dataName}");
            View.SetAnimatorData(Data);
        }

        private void UndoRedoPerformed()
        {
            View.SetAnimatorData(Data);
        }

        private void OnSelectionChange()
        {
            if (Application.isPlaying && Selection.activeObject is GameObject go)
            {
                var behaviour = go.GetComponent<AnimatorBehaviour>();
                if (behaviour && behaviour.Data)
                {
                    RefreshData(behaviour.Data);
                    _debuger.Reset(behaviour.Runner, View);
                }
            }
        }
    }
}