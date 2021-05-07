using System.Text;
using GameApp.Network;
using GameApp.Pesistence;
using GameApp.Util;
using UnityEngine;

namespace GameApp.DebugConsole
{
    public class ClientDebugHandler : TcpClientBehaviour
    {
        private static ClientDebugHandler _instance;
        public static ClientDebugHandler Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = MonoHolder.Holder.AddComponent<ClientDebugHandler>();
                }

                return _instance;
            }
        }

        public bool autoSendLog;

        private EventDispatcher Dispatcher => EventDispatcher.Instance;

        public void Connect(string ip)
        {
            serverIp = ip;
            Connect();
        }

        protected override void Awake()
        {
            base.Awake();
            serverPort = ServerDebugHandler.ServerPort;
            _instance = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Application.logMessageReceived += OnLogMessageReceived;
            Dispatcher.AddListener<object>(typeof(CommandMessage).FullName, OnCommandMessage);
            Dispatcher.AddListener<object>(typeof(ReloadLuaMessage).FullName, OnReloadLuaMessage);
            Dispatcher.AddListener<object>(typeof(SaveFileMessage).FullName, OnSaveFileMessage);
            Dispatcher.AddListener<object>(typeof(ClearSelectedMessage).FullName, OnClearSelectedMessage);
        }
        
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (!autoSendLog) return;
            
            var msg = new LogMessage
            {
                condition = condition,
                stackTrace = stackTrace,
                type = type
            };
            Send(msg);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Application.logMessageReceived -= OnLogMessageReceived;
            Dispatcher.RemoveListener<object>(typeof(CommandMessage).FullName, OnCommandMessage);
            Dispatcher.RemoveListener<object>(typeof(ReloadLuaMessage).FullName, OnReloadLuaMessage);
            Dispatcher.RemoveListener<object>(typeof(SaveFileMessage).FullName, OnSaveFileMessage);
            Dispatcher.RemoveListener<object>(typeof(ClearSelectedMessage).FullName, OnClearSelectedMessage);
        }

        private void OnCommandMessage(object msg)
        {
            var command = (CommandMessage) msg;
            Debug.Log($"Receive Command: {command.content}");
            CommandUtil.DoString(command.content);
        }

        private void OnReloadLuaMessage(object msg)
        {
            var command = (ReloadLuaMessage) msg;
            Debug.Log($"Reload lua: {command.requirePath} \n {command.content}");
            CommandUtil.ReloadLua(command.requirePath, command.content);
        }

        private void OnSaveFileMessage(object msg)
        {
            var command = (SaveFileMessage) msg;
            Debug.Log($"Save file at: {command.path}");
            var _ = PersistFile.AsyncWriteToFile(Encoding.UTF8.GetBytes(command.content), command.path,
                PersistPipeline.RawDataPipeline);
        }

        private void OnClearSelectedMessage(object msg)
        {
            var command = (ClearSelectedMessage) msg;
            var files = "";
            command.files.ForEach(f => { files += $"{f}\n"; });
            Debug.Log($"Clear files: [{command.allFlag}] \n {files}");
            if (command.allFlag)
                command.files.ForEach(p => { PersistFile.ClearDirectory(p, PersistProfile.PersistType.Data); });
            else
                PersistFile.ClearFile(command.files, PersistProfile.PersistType.Data);
        }

        #if TEMPLATE_MODE
        private string _serverIp = "127.0.0.1";

        private void OnGUI()
        {
            GUILayout.Space(100);
            GUILayout.BeginHorizontal();
            _serverIp = GUILayout.TextField(_serverIp, GUILayout.Height(30), GUILayout.Width(160));
            if (GUILayout.Button("Connect", GUILayout.Height(30), GUILayout.Width(80)))
            {
                Connect(_serverIp);
            }
            if (GUILayout.Button("Disconnect", GUILayout.Height(30), GUILayout.Width(80)))
            {
                Disconnect();
            }
            if (GUILayout.Button("LogMessage", GUILayout.Height(30), GUILayout.Width(80)))
            {
                var msg = new LogMessage
                {
                    condition = "condition",
                    stackTrace = "stack",
                    type = LogType.Log
                };
                Send(msg);
            }
            
            /*if (GUILayout.Button("Test Reload", GUILayout.Height(30), GUILayout.Width(80)))
            {
                var str = System.IO.File.ReadAllText("Assets/Lua/Test/Test.lua");
                var msg = new ReloadLuaMessage
                {
                    requirePath = "Test/Test",
                    content = str
                };
                OnReloadLuaMessage(msg);
            }*/

            GUILayout.EndHorizontal();
        }
        #endif
    }
}