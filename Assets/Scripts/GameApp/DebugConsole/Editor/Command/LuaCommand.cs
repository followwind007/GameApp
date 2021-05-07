using UnityEngine;
using UnityEditor;

namespace GameApp.DebugConsole
{
    [System.Serializable]
    public class LuaCommand : ICommand
    {
        public string name;
        public string Name 
        {
            get { return name; }
            set { name = value; }
        }

        public string describe;
        public string Describe
        {
            get { return describe; }
            set { describe = value; }
        }

        public string command;
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        public LuaCommand(string name, string describe, string command)
        {
            Name = name;
            Describe = describe;
            Command = command;
        }

        public void DoCommand()
        {
            CommandUtil.DoString(command);
        }
    }
}
