using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class ResCheckView : VisualElement
    {
        private readonly string[] _views = { "Atlas", "Atlas Module", "Prefab" };
        private string _view = "Atlas";

        private readonly VisualElement _tree;
        private readonly VisualElement _scroll;
        private readonly VisualElement _detail;

        private readonly IChecker[] _checkers = { AtlasChecker.Instance, EffectChecker.Instance };

        public ResCheckView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("ResCheck/Styles/ResCheckView"));
            var toolbar = new Toolbar { name = "toolbar" };
            Add(toolbar);
            
            var btnAtlas = new ToolbarButton(() =>
            {
                TextureChecker.Instance.StartCheck();
            }) {text = "Check"};
            toolbar.Add(btnAtlas);
            
            var btnAnalyse = new ToolbarButton(() =>
            {
                foreach (var checker in _checkers) checker.StartCheck();
                Refresh();
            }){text = "Analyse"};
            toolbar.Add(btnAnalyse);
            
            /*var btnTest = new ToolbarButton(() =>
            {
            }){text = "Test"};
            toolbar.Add(btnTest);*/
            
            var panelDrop = new ToolbarMenu { text = _view};
            foreach (var v in _views)
            {
                panelDrop.menu.AppendAction(v, a =>
                {
                    OnSelectView(v);
                    panelDrop.text = v;
                }, action => DropdownMenuAction.Status.Normal);
                toolbar.Add(panelDrop);
            }
            
            var content = new VisualElement { name = "content" };
            Add(content);

            var treeScrollView = new ScrollView { name = "treeScrollView", verticalScrollerVisibility = ScrollerVisibility.Auto };
            content.Add(treeScrollView);
            _tree = new VisualElement { name = "treeContainer" };
            treeScrollView.Add(_tree);
            
            var scrollView = new ScrollView { name = "scrollView", verticalScrollerVisibility = ScrollerVisibility.Auto };
            scrollView.contentViewport.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
            content.Add(scrollView);
            
            _scroll = new VisualElement { name = "scrollContent" };
            scrollView.Add(_scroll);
            
            var detailScrollView = new  ScrollView { name = "detailScrollView", verticalScrollerVisibility = ScrollerVisibility.Auto };
            content.Add(detailScrollView);
            
            _detail = new VisualElement { name = "detailContent" };
            detailScrollView.Add(_detail);
        }

        public void Refresh()
        {
            OnSelectView(_view);
        }

        public void RefreshDetail(object data)
        {
            _detail.Clear();
            VisualElement element = null;
            if (data is AtlasChecker.AtlasItem item)
            {
                element = new AtlasItemPanel(item);
            }

            if (element != null)
            {
                _detail.Add(element);
            }
        }

        private void OnSelectView(string type)
        {
            _view = type;
            
            _tree.Clear();
            _scroll.Clear();
            
            switch (_view)
            {
                case "Atlas":
                    SetAtlasView();
                    break;
                case "Atlas Module":
                    SetAtlasModuleView();
                    break;
                case "Prefab":
                    SetPrefabView();
                    break;
            }
        }

        private void SetAtlasView()
        {
            foreach (var atlas in AtlasChecker.Atlases)
            {
                var btn = new Button(() => { RefreshAtlasScroll(atlas.Value); }) { name = "button", text = atlas.Key};
                _tree.Add(btn);
            }
        }
        
        private void RefreshAtlasScroll(AtlasChecker.AtlasInfo atlas)
        {
            _scroll.Clear();
            foreach (var item in atlas.sprites.Values)
            {
                var entry = new AtlasEntry(item);
                _scroll.Add(entry);
            }
        }

        private void SetAtlasModuleView()
        {
            foreach (var module in DataSource.Modules)
            {
                var btn = new Button(() => { RefreshAtlasModuleScroll(module.Value); }) { name = "button", text = module.Key };
                _tree.Add(btn);
            }
        }

        private void RefreshAtlasModuleScroll(DataSource.ModuleInfo module)
        {
            _scroll.Clear();
            foreach (var atlas in module.atlasDict.Values)
            {
                var entry = new ModuleEntry(atlas, module);
                _scroll.Add(entry);
            }
        }

        private void SetPrefabView()
        {
            foreach (var module in DataSource.Modules)
            {
                var btn = new Button(() => { RefreshPrefabScroll(module.Value); }) { name = "button", text = module.Key };
                _tree.Add(btn);
            }
        }

        private void RefreshPrefabScroll(DataSource.ModuleInfo module)
        {
            _scroll.Clear();
            foreach (var prefab in module.prefabs.Values)
            {
                var entry = new PrefabEntry(prefab);
                _scroll.Add(entry);
            }
        }


    }
}