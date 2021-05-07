using GameApp.DataBinder;
using LuaInterface;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.DebugConsole
{
    public class AnalyzeView : VisualElement
    {
        private readonly LuaDataTreeView _tree;
        
        public enum SourceType
        {
            Select, GameObject, Path
        }

        private readonly VisualElement _horContainer;
        private readonly EnumField _sourceType;
        private readonly TextField _sourcePath;
        private readonly ObjectField _sourceObject;

        private LuaState State => BindableTargetImpl.State;

        private SourceType Source => (SourceType)_sourceType.value;

        private IBindableTarget Binder
        {
            get
            {
                GameObject go = null;
                if (Source == SourceType.GameObject)
                {
                    go = _sourceObject.value as GameObject;
                }
                else if (Source == SourceType.Select)
                {
                    go = Selection.activeObject as GameObject;
                }
                return go != null ? go.GetComponent<IBindableTarget>() : null;
            }
        }

        private LuaTable _tbl;
        
        private DebugConsolePrefs Prefs => DebugConsolePrefs.Instance;

        public AnalyzeView()
        {
            AddToClassList("TabView");
            
            var luaDataState = new TreeViewState();
            _tree = new LuaDataTreeView(luaDataState);
            
            var tree = new IMGUIContainer();
            tree.AddToClassList("DebugConsoleTree");
            tree.onGUIHandler = () =>
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(tree.contentRect.height));
                _tree.OnGUI(rect);
            };
            
            Add(tree);
            
            _horContainer = new VisualElement();
            _horContainer.AddToClassList("HorContainer");
            Add(_horContainer);
            
            _sourcePath = new TextField();
            _sourcePath.AddToClassList("Grow");
            _sourceObject = new ObjectField { objectType = typeof(GameObject) };
            _sourceObject.AddToClassList("Grow");

            _sourceType = new EnumField(Prefs.sourceType) { name = "sourceType" };
            _sourceType.RegisterValueChangedCallback(e => { OnSelectType(); });
            _horContainer.Add(_sourceType);

            OnSelectType();

            var btns = new VisualElement { name = "btns" };
            Add(btns);
            
            var btnUse = new Button(OnUse) { text = "Use" };
            btnUse.AddToClassList("BtnNormal");
            btns.Add(btnUse);
            
            var btnReloadData = new Button(OnReloadData) { text = "Reload Data" };
            btnReloadData.AddToClassList("BtnNormal");
            btns.Add(btnReloadData);
            
            var btnReloadLua = new Button(OnReloadLua) { text = "Reload Lua" };
            btnReloadLua.AddToClassList("BtnNormal");
            btns.Add(btnReloadLua);
            
            var btnReloadAll = new Button(OnReloadAll) { text = "ReloadAll" };
            btnReloadAll.AddToClassList("BtnNormal");
            btns.Add(btnReloadAll);
        }

        public void RefreshTree(LuaTable tbl)
        {
            _tbl = tbl;
            RefreshTree();
        }

        private void OnSelectType()
        {
            Prefs.sourceType = Source;
            if (Source == SourceType.Select || Source == SourceType.GameObject)
            {
                _sourcePath.RemoveFromHierarchy();
                _horContainer.Add(_sourceObject);
            }
            else
            {
                _sourceObject.RemoveFromHierarchy();
                _horContainer.Add(_sourcePath);
            }
        }

        private void OnUse()
        {
            if (Source == SourceType.Select && Selection.activeObject is GameObject go) _sourceObject.value = go;
            
            _tbl = Source == SourceType.Path ? State?.DoString<LuaTable>(_sourcePath.value) : 
                State?.Invoke<IBindableTarget, LuaTable>("Adapter.GetLuaTable", Binder, true);
            if (_tbl != null) State?.DoString<LuaTable>("return _G").RawSet("_table", _tbl);
            RefreshTree();
        }

        private void OnReloadData()
        {
            if (Binder == null) return;
            var tbl = State?.Invoke<IBindableTarget, LuaTable>("Adapter.GetLuaTable", Binder, true);
            State?.Call("Adapter.LoadData", tbl, true);
            RefreshTree();
        }

        private void OnReloadLua()
        {
            if (Binder == null) return;
            State?.Call("Adapter.Reload", BindableTargetImpl.GetLuaPath(Binder.LuaPath), true);
            RefreshTree();
        }

        private void OnReloadAll()
        {
            State?.Call("Adapter.ReloadAll", true);
            State?.Call("Adapter.LoadAllData", true);
            RefreshTree();
        }

        private void RefreshTree()
        {
            _tree.RefreshWithTable(_tbl);
        }

    }
}