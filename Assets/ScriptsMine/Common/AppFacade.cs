using UnityEngine;
using System.Collections.Generic;

namespace Pangu
{
    public class AppFacade
    {
        private static AppFacade _instance;
        public static AppFacade Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppFacade();
                }
                return _instance;
            }
        }
        private AppFacade() { }

        public void SendMessageCommand(string messageName, object body = null)
        {
            Debug.Log(string.Format("receive message: {0}", messageName));
            if (body != null)
            {
                foreach (var kv in body as Dictionary<string, object>)
                {
                    Debug.Log(string.Format("{0}:{1}", kv.Key, kv.Value));
                }
            }
        }
    }

}