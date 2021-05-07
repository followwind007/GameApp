using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameApp.DataBinder
{
    [CustomEditor(typeof(BehaviourBinder))]
    public class DataBinderEditor : Editor
    {
        protected IBindableTarget bind;
        public void OnEnable()
        {
            bind = target as IBindableTarget;
        }
        
        protected BindableInfo bInfo;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (bind != null)
            {
                DrawLuaSupport(bind.Vals, bind.LuaPath, bind.Interfaces);
            }
        }

        protected void DrawLuaSupport(BindableValues vals, string destPath, List<LuaPath> interfacePaths)
        {
            if (bInfo == null)
            {
                SetBindaleInfo(destPath, interfacePaths);
            }
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(bInfo != null && bInfo.bindType == BindableInfo.BindType.Static);
            if (GUILayout.Button("Generate Lua", GUILayout.Width(100)))
            {
                LuaExporter.GenerateLuaCode(destPath, vals);
            }
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Fetch File Field", GUILayout.Width(100)))
            {
                SetBindaleInfo(destPath, interfacePaths);
            }
            
            EditorGUILayout.EndHorizontal();
        }

        private void SetBindaleInfo(string destPath, List<LuaPath> interfacePaths)
        {
            SyncBindableInfo(destPath, true);
            if (interfacePaths != null)
            {
                foreach (var p in interfacePaths) SyncBindableInfo(p.path);
            }
            BindableValuesDrawer.SetBindableInfo(bInfo);
        }

        private void SyncBindableInfo(string path, bool setFalg = false)
        {
            var info = BindableInfo.FetchField(path);
            if (setFalg || bInfo == null)
            {
                bInfo = info;
            }
            
            foreach (var wrap in info.wraps.Values)
            {
                var exist = bInfo.wraps.ContainsKey(wrap.name);
                if (!exist)
                {
                    bInfo.wraps.Add(wrap.name, wrap);
                }
            }
        }
        

    }
}