#if UNITY_EDITOR
using System;
using UnityEditor;

namespace GameApp.Assets
{
    public class LoadFromAsset : AsyncOperationBase
    {
        private string _name;
        private Type _type;

        public LoadFromAsset(string name, Type type)
        {
            _name = name;
            _type = type;
        }

        public override void StartAsync()
        {
            base.StartAsync();

            Result = AssetDatabase.LoadAssetAtPath(_name, _type);
            
            if (Result != null)
                OnSuccess();
            else
                OnError();
        }
    }
}
#endif