using System.Collections.Generic;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class MessagePlayableCommand
    {
        public enum CommandType
        {
            Default = 0,
        }

        public string name;

        public Dictionary<string, object> body = new Dictionary<string, object>();

        public void DoCommand()
        {
            if (!string.IsNullOrEmpty(name))
            {
#if !TEMPLATE_MODE
                if (UnityEngine.Application.isPlaying)
                {
                    var luaManager = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
                    if (luaManager)
                        luaManager.State.Call("Event.Brocast", name, body, false);
                }
#endif
            }
        }

    }
}
