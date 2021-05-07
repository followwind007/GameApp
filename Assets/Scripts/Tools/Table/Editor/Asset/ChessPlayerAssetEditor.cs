using System.IO;
using UnityEditor;
using UnityEngine;
using GameApp.Serialize;

namespace Tools.Table
{
    //[CustomEditor(typeof(ChessPlayerAsset))]
    public class ChessPlayerAssetEditor : Editor
    {
        private const string LuaPath = "Assets/Lua/app/RookieConfig/";
        private ChessPlayerAsset _asset;

        private string _lua;
        
        private string ConfigPath => $"{LuaPath}{_asset.playerInfos.stageId}.lua";

        private void OnEnable()
        {
            _asset = (ChessPlayerAsset)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Create Lua", GUILayout.Width(80)))
            {
                _lua = _asset.playerInfos?.EncodeToLua();
                
                File.WriteAllText(ConfigPath, $"local content = {_lua}\nreturn content");
                Debug.Log($"save config to: {ConfigPath}");
            }
            
            /*if (GUILayout.Button("Fetch Lua", GUILayout.Width(80)))
            {
                var infos = _lua.DecodeFromLua<ChessPlayerAsset.PlayerInfos>();
                _asset.playerInfos = infos;
                EditorUtility.SetDirty(_asset);
            }*/
            
            if (GUILayout.Button("Delete Lua", GUILayout.Width(80)))
            {
                File.Delete(ConfigPath);
                Debug.Log($"delete config from: {ConfigPath}");
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            
            _lua = EditorGUILayout.TextArea(_lua, GUILayout.MinHeight(100));
        }

    }
}