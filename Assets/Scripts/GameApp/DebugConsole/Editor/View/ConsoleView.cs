using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameApp.DebugConsole
{
    public class ConsoleView : VisualElement
    {
        public enum ConsoleMode { Local, Remote }
        public readonly TextField textCommand;
        public readonly TextField textName;
        public readonly TextField textDesc;

        private readonly CommandTreeView _tree;
        
        private DebugConsolePrefs Prefs => DebugConsolePrefs.Instance;
        
        public ConsoleView()
        {
            AddToClassList("TabView");
            
            var cmdState = new TreeViewState();
            
            _tree = new CommandTreeView(cmdState)
            {
                commandList = CommandManager.Instance.LuaCommandList
            };
            _tree.Init();
            _tree.useCommand = c =>
            {
                if (c is LuaCommand lc)
                {
                    textName.value = lc.name;
                    textDesc.value = lc.describe;
                    textCommand.value = lc.command;
                }
            };
            
            var tree = new IMGUIContainer();
            tree.AddToClassList("DebugConsoleTree");
            tree.onGUIHandler = () =>
            {
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(tree.contentRect.height));
                _tree.OnGUI(rect);
            };
            
            Add(tree);
            
            textName = new TextField("Name") { value = Prefs.commandName };
            Add(textName);
            textDesc = new TextField("Describe") { value = Prefs.commandDesc};
            Add(textDesc);
            
            textCommand = new TextField { name = "textCommand", value = Prefs.command, multiline = true };
            Add(textCommand);

            var btns = new VisualElement { name = "btns" };
            Add(btns);

            var enumMode = new EnumField(Prefs.consoleMode);
            enumMode.AddToClassList("BtnNormal");
            enumMode.RegisterValueChangedCallback(e => { Prefs.consoleMode = (ConsoleMode)e.newValue; });
            btns.Add(enumMode);
            
            var btnAdd = new Button(AddCommand) { text = "Add" };
            btnAdd.AddToClassList("BtnSmall");
            btns.Add(btnAdd);
            var btnRun = new Button(DoCommand) { text = "Run" };
            btnRun.AddToClassList("Grow");
            btns.Add(btnRun);
        }

        private void DoCommand()
        {
            var c = textCommand.value;
            switch (Prefs.consoleMode)
            {
                case ConsoleMode.Local:
                    CommandUtil.DoString(c);
                    break;
                case ConsoleMode.Remote:
                    if (Application.isPlaying)
                    {
                        ServerDebugHandler.Instance.DoCommand(c);
                    }
                    break;
            }
        }

        private void AddCommand()
        {
            if (string.IsNullOrEmpty(textName.value)) return;
            var command = new LuaCommand(textName.value, textDesc.value, textCommand.value);
            CommandManager.Instance.AddLuaCommand(command);
            _tree.Reload();
        }
        
    }
}