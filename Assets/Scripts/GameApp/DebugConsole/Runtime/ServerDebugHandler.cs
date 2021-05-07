using System.Collections.Generic;
using System.IO;
using GameApp.DataBinder;
using GameApp.Network;
using GameApp.Util;
using UnityEngine;

namespace GameApp.DebugConsole
{
    public class ServerDebugHandler : TcpServerBehaviour
    {
        public const int ServerPort = 9527;
        private static ServerDebugHandler _instance;
        public static ServerDebugHandler Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = MonoHolder.Holder.AddComponent<ServerDebugHandler>();
                }

                return _instance;
            }
        }

        private EventDispatcher Dispatcher => EventDispatcher.Instance;
        
        protected override void Awake()
        {
            _instance = this;
            port = ServerPort;
            listenOnAwake = true;
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Dispatcher.AddListener<object>(typeof(LogMessage).FullName, OnLogMessage);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Dispatcher.RemoveListener<object>(typeof(LogMessage).FullName, OnLogMessage);
        }

        public void DoCommand(string command)
        {
            var cmsg = new CommandMessage
            {
                content = command
            };
            BroadCast(cmsg);
        }

        public void Upload(string path)
        {
            SaveFile(path);
            ReloadFile(path);
        }

        public void SaveFile(string path)
        {
            var saveFileMsg = new SaveFileMessage
            {
                path = GetLuaPath(path),
                content = File.ReadAllText(path)
            };
            BroadCast(saveFileMsg);
        }

        public void ReloadFile(string path)
        {
            var reloadMsg = new ReloadLuaMessage
            {
                requirePath = BindableTargetImpl.GetLuaPath(path),
                content = File.ReadAllText(path)
            };
            BroadCast(reloadMsg);
        }

        public void ClearSelected(List<string> paths, bool allFlag)
        {
            var ps = new List<string>();
            paths.ForEach(p => { ps.Add(GetLuaPath(p)); });
            var deleteMsg = new ClearSelectedMessage
            {
                files = ps,
                allFlag = allFlag
            };
            BroadCast(deleteMsg);
        }
        
        private string GetLuaPath(string path)
        {
            #if !TEMPLATE_MODE
            var savePath = path.Replace("Assets/Lua", $"{(AppConst.Is64 ? LuaConst.Lua64StreamDir : LuaConst.LuaStreamDir)}");
            #else
            var savePath = path;
            #endif
            return savePath;
        }

        private void OnLogMessage(object msg)
        {
            var command = (LogMessage) msg;
            Debug.unityLogger.Log(command.type, $"[Remote]:{command.condition}\n{command.stackTrace}");
        }

        #if UNITY_EDITOR
        private void OnGUI()
        {
            foreach (var c in Server.Connections)
            {
                if (c.client != null && c.client.Connected)
                {
                    GUILayout.Label($"{c.client.Client.RemoteEndPoint}:{c.client.Client.Connected}");
                }
            }
        }
        #endif
    }
}