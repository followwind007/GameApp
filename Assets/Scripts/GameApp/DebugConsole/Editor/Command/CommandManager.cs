using UnityEngine;
using System.Collections.Generic;

namespace GameApp.DebugConsole
{
    public class CommandManager
    {
        private static CommandManager _instance;
        public static CommandManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommandManager
                    {
                        LuaCommandList = new List<ICommand>()
                    };
                    _instance.OnUpdate();
                }
                return _instance;
            }
        }
        private CommandManager() { }

        private const string LuaCommandName = "LUA_COMMAND_NAME";
        private const string LuaCommandDescribe = "LUA_COMMAND_DESCRIBE_";
        private const string LuaCommandCommand = "LUA_COMMAND_COMMAND_";

        public delegate void OnUpdateCommand();
        public OnUpdateCommand onUpdateCommand;

        public List<ICommand> LuaCommandList { get; private set; }

        private IEnumerable<string> CommandNames
        {
            get
            {
                var commands = PlayerPrefs.GetString(LuaCommandName);
                return commands?.Split('&');
            }
        }

        public void AddLuaCommand(LuaCommand command)
        {
            foreach (var commandName in CommandNames)
            {
                if (GetCommandName(command.Name).Equals(commandName))
                {
                    DeleteLuaCommand(command.Name);
                    break;
                }
            }
            var names = PlayerPrefs.GetString(LuaCommandName);
            if (string.IsNullOrEmpty(names)) names = "";
            names += "&" + GetCommandName(command.Name);
            PlayerPrefs.SetString(LuaCommandName, names);

            PlayerPrefs.SetString(LuaCommandDescribe + command.Name, command.Describe);
            PlayerPrefs.SetString(LuaCommandCommand + command.Name, command.Command);

            OnUpdate();
        }

        public void DeleteLuaCommand(string name)
        {
            foreach (var commandName in CommandNames)
            {
                if (GetCommandName(name).Equals(commandName))
                {
                    var names = PlayerPrefs.GetString(LuaCommandName);
                    names = names.Replace("&" + GetCommandName(name), "");
                    PlayerPrefs.SetString(LuaCommandName, names);
                    PlayerPrefs.DeleteKey(LuaCommandDescribe + name);
                    PlayerPrefs.DeleteKey(LuaCommandCommand + name);
                }
            }
            OnUpdate();
        }

        public void OnUpdate()
        {
            LuaCommandList.Clear();
            var commands = Resources.Load<DebugConsoleSettings>("LuaCommand");
            if (commands && commands.luaCommands.Count > 0)
            {
                LuaCommandList.AddRange(commands.luaCommands.ToArray());
            }
            
            foreach (var commandName in CommandNames)
            {
                if (string.IsNullOrEmpty(commandName)) continue;
                var name = GetRawName(commandName);
                var describe = PlayerPrefs.GetString(LuaCommandDescribe + name);
                var command = PlayerPrefs.GetString(LuaCommandCommand + name);                
                LuaCommandList.Add(new LuaCommand(name, describe, command));
            }

            onUpdateCommand?.Invoke();
        }

        private string GetCommandName(string name)
        {
            return string.Format("<{0}>", name);
        }

        private string GetRawName(string name)
        {
            name = name.Replace("<", "");
            return name.Replace(">", "");
        }

        public void ClearLuaCommands()
        {
            foreach (var commandName in CommandNames)
            {
                PlayerPrefs.DeleteKey(LuaCommandDescribe + commandName);
                PlayerPrefs.DeleteKey(LuaCommandCommand + commandName);
            }
            PlayerPrefs.DeleteKey(LuaCommandName);
            OnUpdate();
        }

    }

}