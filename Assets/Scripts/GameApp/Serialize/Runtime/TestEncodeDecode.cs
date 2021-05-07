using System.Collections.Generic;
using UnityEngine;

namespace GameApp.Serialize
{
    public class TestEncodeDecode : MonoBehaviour
    {
        [System.Serializable]
        public class Item
        {
            public int intVal;
        }
        
        [System.Serializable]
        public class ClsA
        {
            public int intVal;
            public string stringVal;
            public float floatVal;
            public bool boolVal;
            public List<Item> listVal;
        }

        public ClsA a = new ClsA();

        private string luaStr;
        private void OnGUI()
        {
            if (GUILayout.Button("Encode To Lua"))
            {
                luaStr = a.EncodeToLua();
                Debug.Log($"encode to: {luaStr}");
            }
            
            if (GUILayout.Button("Encode To Lua"))
            {
                var obj = luaStr.DecodeFromLua<ClsA>();
                Debug.Log($"encode to: {obj.EncodeToLua()}");
            }
        }
    }
}