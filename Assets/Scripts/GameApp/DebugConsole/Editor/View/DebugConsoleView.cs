using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.DebugConsole
{
    public class DebugConsoleView : VisualElement
    {
        public AnalyzeView Analyze => (AnalyzeView) _tabView[TabAnalyze];
        public ConsoleView Console => (ConsoleView) _tabView[TabConsole];
        
        public const string TabConsole = "Console";
        public const string TabAnalyze = "Analyze";

        private readonly string[] _tabs = {TabConsole, TabAnalyze};
        private int _selectedTab;

        private readonly VisualElement _content;
        private readonly Dictionary<string, VisualElement> _tabView = new Dictionary<string, VisualElement>();
        
        private DebugConsolePrefs Prefs => DebugConsolePrefs.Instance;

        public DebugConsoleView()
        {
            AddToClassList("DebugConsoleView");
            this.AddStyleSheetPath("Styles/DebugConsole/DebugConsoleView");
            var tb = new Toolbar();
            Add(tb);
            
            var tglAutoUpload = new ToolbarToggle { text = "Auto Upload", value = Prefs.autoUpload, tooltip = "auto upload changes to remove client" };
            tglAutoUpload.RegisterValueChangedCallback(e => { Prefs.autoUpload = e.newValue; });
            tb.Add(tglAutoUpload);

            for (var i = 0; i < _tabs.Length; i++) if (_tabs[i] == Prefs.selectedTab) _selectedTab = i;

            var tabs = new IMGUIContainer(() =>
            {
                EditorGUI.BeginChangeCheck();
                _selectedTab = GUILayout.Toolbar(_selectedTab, _tabs);
                if (EditorGUI.EndChangeCheck()) OnSelectTab(_tabs[_selectedTab]);
            }) {name = "tabs"};
            Add(tabs);
            
            _content = new VisualElement { name = "content" };
            Add(_content);

            _tabView[TabConsole] = new ConsoleView();
            _tabView[TabAnalyze] = new AnalyzeView();

            OnSelectTab(Prefs.selectedTab);
        }

        private void OnSelectTab(string tab)
        {
            Prefs.selectedTab = tab;
            foreach (var kv in _tabView)
            {
                if (kv.Key != tab) kv.Value.RemoveFromHierarchy();
                else _content.Add(kv.Value);
            }
        }
        
    }
}