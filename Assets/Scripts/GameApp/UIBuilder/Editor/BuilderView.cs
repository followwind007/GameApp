using System.Collections.Generic;
using System.IO;
using Tools;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameApp.UIBuilder
{
    public class BuilderView : VisualElement
    {
        private readonly VisualElement _treeContainer;
        private readonly VisualElement _scrollView;
        private readonly VisualElement _scrollContent;

        private DirectoryInfo _currentPath;

        private string _searchContent;
        private string searchContent
        {
            get { return _searchContent;}
            set
            {
                _searchContent = value;
                RefreshWithFilter();
            }
        }
        
        private readonly Dictionary<string, ElementView> _elements = new Dictionary<string, ElementView>();

        public BuilderView()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("UIBuilder/Styles/BuilderView"));

            var prefs = Prefs.instance;
            
            //toolbar
            var toolbar = new Toolbar { name = "toolbar" };
            
            var refreshPreviwBtn = new ToolbarButton (() =>
            {
                if (!Application.isPlaying) return;
                PreviewUtil.RegenerateAll();
                RefreshCurrentPreviews();
            }) { name = "refreshPreviewBtn", text = "Refresh"};
            toolbar.Add(refreshPreviwBtn);
            
            var bgColorField = new ColorField { name = "bgColorField", showEyeDropper = false, value = prefs.bgColor};
            bgColorField.RegisterValueChangedCallback(evt =>
            {
                _scrollView.style.backgroundColor = evt.newValue;
                prefs.bgColor = evt.newValue;
            });
            toolbar.Add(bgColorField);

            var scaleSlider = new Slider { name = "scaleSlider", lowValue = 0.5f, highValue = 2f, value = prefs.scale};
            scaleSlider.RegisterValueChangedCallback(evt =>
            {
                prefs.scale = evt.newValue;
                RefreshScale();
            });
            toolbar.Add(scaleSlider);
            
            var searchField = new ToolbarSearchField { name = "searchField" };
            searchField.RegisterValueChangedCallback(evt => searchContent = evt.newValue);
            toolbar.Add(searchField);

            //tree
            _treeContainer = new VisualElement { name = "treeContainer" };

            //scroll
            _scrollView = new ScrollView { name = "scrollView", verticalScrollerVisibility = ScrollerVisibility.Auto };
            _scrollView.style.backgroundColor = prefs.bgColor;
            _scrollContent = new VisualElement { name = "scrollContent" };
            _scrollView.Add(_scrollContent);
            
            //content
            var content = new VisualElement { name = "content" };
            
            content.Add(_treeContainer);
            content.Add(_scrollView);
            
            Add(toolbar);
            Add(content);
            
            CreateTree();
        }

        private void CreateTree()
        {
            var rootPath = BuilderSettings.instance.prefabRootPath;
            var rootDir = new DirectoryInfo(rootPath);
            foreach (var dir in rootDir.GetDirectories())
            {
                var btn = new Button (() =>
                {
                    _currentPath = dir;
                    RefreshCurrentPreviews();
                }) {name = "treeButton", text = dir.Name};
                
                _treeContainer.Add(btn);
            }

            var files = new List<FileInfo>();
            AssetUtil.GetFiles(rootPath, files, "*.prefab");
            foreach (var file in files)
            {
                var ele = new ElementView(file);
                var eleName = BuilderSettings.GetUniqueId(file.FullName);
                _elements[eleName] = ele;
            }
        }

        private void RefreshCurrentPreviews()
        {
            if (_currentPath == null) return;
            _scrollContent.Clear();
            foreach (var file in _currentPath.GetFiles())
            {
                if (!file.FullName.EndsWith(".prefab")) continue;
                var eleName = BuilderSettings.GetUniqueId(file.FullName);
                if (_elements.ContainsKey(eleName))
                {
                    _scrollContent.Add(_elements[eleName]);
                }
            }
            RefreshScale();
        }

        private void RefreshWithFilter()
        {
            _scrollContent.Clear();
            foreach (var kv in _elements)
            {
                if (string.IsNullOrEmpty(searchContent) || kv.Key.ToLower().Contains(searchContent.ToLower()))
                {
                    _scrollContent.Add(kv.Value);
                }
            }
            RefreshScale();
        }

        private void RefreshScale()
        {
            foreach (var ele in _scrollContent.Children())
            {
                if (ele is ElementView t)
                {
                    t.RefreshSize(Prefs.instance.scale);
                }
            }
        }
        
        
    }
}