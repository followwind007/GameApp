using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameApp.DataBinder;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameApp.EmojiText
{
    public static class ReplaceTMPEntry
    {
        private static readonly Dictionary<FontStyle, FontStyles> FontStyleMap = 
            new Dictionary<FontStyle, FontStyles>()
        {
            {FontStyle.Bold, FontStyles.Bold},
            {FontStyle.Italic, FontStyles.Italic},
            {FontStyle.Normal, FontStyles.Normal},
            {FontStyle.BoldAndItalic, FontStyles.Italic | FontStyles.Bold},
        };

        private static readonly Dictionary<TextAnchor, TextAlignmentOptions> UIAlignmentMap = 
            new Dictionary<TextAnchor, TextAlignmentOptions>()
        {
            {TextAnchor.LowerCenter, TextAlignmentOptions.Bottom},
            {TextAnchor.LowerLeft, TextAlignmentOptions.BottomLeft},
            {TextAnchor.LowerRight, TextAlignmentOptions.BottomRight},
            {TextAnchor.MiddleCenter, TextAlignmentOptions.Center},
            {TextAnchor.MiddleLeft, TextAlignmentOptions.Left},
            {TextAnchor.MiddleRight, TextAlignmentOptions.Right},
            {TextAnchor.UpperCenter, TextAlignmentOptions.Top},
            {TextAnchor.UpperLeft, TextAlignmentOptions.TopLeft},
            {TextAnchor.UpperRight, TextAlignmentOptions.TopRight}
        };

        [MenuItem("Assets/Tools/Replace Text UI")]
        public static void ReplaceSelectedText()
        {
            ForeachSelected<Text>((go) =>
            {
                var text = go.GetComponent<Text>();
                var tmp = go.GetComponent<TextMeshProUGUI>();
                if (tmp != null) return false;
                
                var bind = go.GetComponentInParent<IBindableTarget>();
                var dirtyBinds = new List<ValueWrap>();
                if (bind != null)
                {
                    bind.Vals.Init();
                    dirtyBinds = bind.Vals.wraps.Where(w => w.value is Text t && t == text).ToList();
                    foreach (var w in bind.Vals.wraps)
                    {
                        if (w.value is Text t && t == text)
                        {
                            dirtyBinds.Add(w);
                        }
                    }
                }
                
                var cp = Object.Instantiate(text);
                Object.DestroyImmediate(text);
                tmp = go.AddComponent<TextMeshProUGUI>();

                if (dirtyBinds.Count > 0)
                {
                    dirtyBinds.ForEach(w => w.value = tmp);
                    bind?.Vals.Regenerate();
                }
                
                tmp.text = cp.text;
                tmp.color = cp.color;
                tmp.fontSize = cp.fontSize;
                tmp.enableAutoSizing = cp.resizeTextForBestFit;
                tmp.fontStyle = FontStyleMap[cp.fontStyle];
                tmp.alignment = UIAlignmentMap[cp.alignment];
                tmp.enableWordWrapping = cp.horizontalOverflow == HorizontalWrapMode.Wrap;
                tmp.overflowMode = cp.verticalOverflow == VerticalWrapMode.Overflow ? 
                    TextOverflowModes.Overflow : TextOverflowModes.Truncate;
                tmp.lineSpacing = (cp.lineSpacing - 1) * 100;
                
                Object.DestroyImmediate(cp.gameObject);
                return true;
            });
        }

        [MenuItem("Assets/Tools/Replace Text Mesh")]
        public static void ReplaceSelectedTextMesh()
        {
            ForeachSelected<TextMesh>((go) =>
            {
                var text = go.GetComponent<TextMesh>();
                var tmp = go.GetComponent<TextMeshPro>();
                if (tmp != null) return false;
                
                var bind = go.GetComponentInParent<IBindableTarget>();
                var dirtyBinds = new List<ValueWrap>();
                if (bind != null)
                {
                    bind.Vals.Init();
                    dirtyBinds = bind.Vals.wraps.Where(w => w.value is TextMesh t && t == text).ToList();
                    foreach (var w in bind.Vals.wraps)
                    {
                        if (w.value is TextMesh t && t == text)
                        {
                            dirtyBinds.Add(w);
                        }
                    }
                }
                
                var cp = Object.Instantiate(text);
                Object.DestroyImmediate(text);
                
                var tmpTrans = go.AddComponent<RectTransform>();
                tmpTrans.sizeDelta = Vector2.one;
                
                tmp = go.AddComponent<TextMeshPro>();
                
                if (dirtyBinds.Count > 0)
                {
                    dirtyBinds.ForEach(w => w.value = tmp);
                    bind?.Vals.Regenerate();
                }

                tmp.text = cp.text;
                tmp.color = cp.color;
                tmp.fontSize = cp.fontSize * go.transform.localScale.x;
                
                tmp.fontStyle = FontStyleMap[cp.fontStyle];
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.lineSpacing = (cp.lineSpacing - 1) * 100;
                
                go.transform.localScale = Vector3.one;
                
                Object.DestroyImmediate(cp.gameObject);
                return true;
            });
        }

        public static void ForeachSelected<T>(Func<GameObject, bool> operation) where T : Component
        {
            var paths = new HashSet<string>();
            
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path) )
                {
                    var guids = AssetDatabase.FindAssets("t:GameObject", new[] {path});
                    foreach (var guid in guids) 
                        paths.Add(AssetDatabase.GUIDToAssetPath(guid));
                }
                else if (obj is GameObject go)
                {
                    paths.Add(AssetDatabase.GetAssetPath(go));
                }
            }

            try
            {
                var count = 0f;
                var total = paths.Count;
                foreach (var path in paths)
                {
                    EditorUtility.DisplayProgressBar("Deal Prefab",path, ++count / total);
                    
                    var dirty = false;
                    var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    var inst = PrefabUtility.InstantiatePrefab(go) as GameObject;
                    PrefabUtility.UnpackPrefabInstance(inst, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
                    if (inst == null) continue;
                    var cmps = inst.GetComponentsInChildren<T>();
                    var cmpGos = cmps.Select(cmp => cmp.gameObject);
                
                    foreach (var cmpGo in cmpGos)
                    {
                        if (PrefabUtility.IsPartOfPrefabInstance(cmpGo)) continue;
                        var isDirty = operation.Invoke(cmpGo);
                        dirty = dirty || isDirty;
                    }

                    if (dirty)
                    {
                        PrefabUtility.SaveAsPrefabAsset(inst, path);
                    }
                    Object.DestroyImmediate(inst);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.SaveAssets();
        }

    }
}