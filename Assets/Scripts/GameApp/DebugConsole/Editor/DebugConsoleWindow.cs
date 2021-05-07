using LuaInterface;
using UnityEditor;

namespace GameApp.DebugConsole
{
    public class DebugConsoleWindow : EditorWindow
    {
        public static DebugConsoleWindow Instance { get; private set; }
        public DebugConsoleView View { get; private set; }

        public LuaTable Table { set { View.Analyze.RefreshTree(value); } }

        [MenuItem("Tools/Debug Console", false, 101)]
        public static DebugConsoleWindow Open()
        {
            return GetWindow<DebugConsoleWindow>("Debug Console");
        }
        
        private void OnEnable()
        {
            Instance = this;
            if (View == null)
            {
                View = new DebugConsoleView();
                rootVisualElement.Add(View);
            }
        }

        private void OnDisable()
        {
            var prefs = DebugConsolePrefs.Instance;
            prefs.commandName = View.Console.textName.value;
            prefs.commandDesc = View.Console.textDesc.value;
            prefs.command = View.Console.textCommand.value;
            
            DebugConsolePrefs.SavePrefs();
            Instance = null;
        }
        
    }
}