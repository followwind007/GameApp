using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.AnimatorBehaviour
{
    [CustomEditor(typeof(AnimatorDataOverride))]
    public class AnimatorDataOverrideEditor : Editor
    {
        public class OverrideView : VisualElement
        {
            public readonly SerializedObject serializedObject;
            public readonly AnimatorDataOverride dataOverride;

            public OverrideView(SerializedObject serializedObject)
            {
                this.serializedObject = serializedObject;
                dataOverride = (AnimatorDataOverride) serializedObject.targetObject;
                Refresh();
            }

            public void Refresh()
            {
                Clear();
                var dataField = new ObjectField("Animator")
                {
                    value = dataOverride.animator,
                    objectType = typeof(AnimatorData)
                };
                dataField.RegisterValueChangedCallback(e =>
                {
                    dataOverride.animator = (AnimatorData)e.newValue;
                    dataOverride.InternalRemoveAllOverride();
                    dataOverride.InternalCreateAllOverride();
                    Save();
                    Refresh();
                });
                
                Add(dataField);
                
                dataOverride.overrides.ForEach(o =>
                {
                    var view = AnimatorTypeUtil.GetStateOverrideView(o);
                    if (view != null)
                    {
                        view.userData = serializedObject;
                        view.SetExpanded(false);
                        Add(view);
                    }
                });
                
                var btns = new VisualElement();
                {
                    btns.style.flexDirection = FlexDirection.Row;
                }
                Add(btns);
                
                var btnSync = new Button(() =>
                {
                    dataOverride.InternalCreateAllOverride();
                    Save();
                    Refresh();
                }) { text = "Sync" };
                btns.Add(btnSync);
                
                var btnClean = new Button(() =>
                {
                    dataOverride.InternalCleanOverride();
                    Save();
                    Refresh();
                }) { text = "Clean" };
                btns.Add(btnClean);
            }

            public void Save()
            {
                Undo.RegisterCompleteObjectUndo(dataOverride, "save target");
                EditorUtility.SetDirty(dataOverride);
                AssetDatabase.SaveAssets();
            }
        }

        public OverrideView View { get; private set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            View = new OverrideView(serializedObject);
            return View;
        }

        private void OnEnable()
        {
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        private void UndoRedoPerformed()
        {
            View?.Refresh();
        }
        
        
    }
}