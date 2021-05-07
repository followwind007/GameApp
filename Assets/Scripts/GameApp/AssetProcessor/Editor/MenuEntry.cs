using System;
using System.Collections.Generic;
using System.IO;
using GameApp.Util;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameApp.AssetProcessor
{
    public static class MenuEntry
    {
        [MenuItem("Assets/AssetProcessor/Check", false,101)]
        public static void Check()
        {
            CallCheck(false);
        }

        [MenuItem("Assets/AssetProcessor/Repair", false,102)]
        public static void Repair()
        {
            CallCheck(true);
        }

        public static void CallCheck(bool doFix)
        {
            var paths = new List<string>();
            
            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path) )
                {
                    var guids = AssetDatabase.FindAssets("t:GameObject", new[] {path});
                    foreach (var guid in guids)
                    {
                        paths.Add(AssetDatabase.GUIDToAssetPath(guid));
                    }
                }
                else if (obj is GameObject go)
                {
                    paths.Add(AssetDatabase.GetAssetPath(go));
                }
            }
            
            CheckObjects(paths, doFix);
        }

        public static void CheckObjects(List<string> paths, bool doFix)
        {
            try
            {
                for (var i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];
                    EditorUtility.DisplayProgressBar("Asset Process", path, (float)i / paths.Count);

                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    
                    var ps = AssetProcessorSetting.GetGameObjectProcessors(path);

                    foreach (var p in ps)
                    {
                        foreach (var gp in p.gameObjectProcessors)
                        {
                            var ctx = new GameObjectCheckerContext {sourceObject = go, doFix = doFix};
                            CompileEntry.CallGameObjectCheckers(gp.checkerIds, ctx);
                        }

                        foreach (var cp in p.componentProcessors)
                        {
                            var t = TypeUtil.GetType(cp.type);
                            if (t == null) continue;
                            var components = go.GetComponentsInChildren(t);
                            foreach (var component in components)
                            {
                                var ctx = new ComponentCheckerContext()
                                    {component = component, doFix = doFix, sourceObject = go};
                                CompileEntry.CallComponentCheckers(t, cp.checkerIds, ctx);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            if (doFix)
            {
                AssetDatabase.SaveAssets();
            }
        }
        
    }
}