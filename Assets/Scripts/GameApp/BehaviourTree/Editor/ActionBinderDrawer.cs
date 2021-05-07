using System;
using System.Collections.Generic;
using System.IO;
using BehaviorDesigner.Runtime.Tasks;
using GameApp.DataBinder;
using Tools;
using UnityEditor;
using UnityEngine;
using ValueType = GameApp.DataBinder.ValueType;

namespace BehaviorDesigner.Editor
{
    [CustomObjectDrawer(typeof(BindableValues))]
    public class ActionBinderDrawer : ObjectDrawer
    {
        private BindableInfo _info;
        
        private BindableValues Vals => (BindableValues)value;

        private const float PropWidth = 182f;
        
        #if TEMPLATE_MODE
        private const string LuaPath = "Assets/Lua/App/BehaviourTree/";
        #else
        private const string LuaPath = "Assets/Lua/app/BehaviourTree/";
        #endif
        
        public override void OnGUI(GUIContent label)
        {
            Init();
            EditorGUILayout.BeginVertical();

            Vals.Init();
            EditorGUI.BeginChangeCheck();
            foreach (var w in Vals.wraps)
            {
                EditorGUILayout.BeginHorizontal();
                DrawWrap(w);
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Save();
            }
            
            EditorGUILayout.EndVertical();
            if (task != null && task.Owner != null)
            {
                EditorUtility.SetDirty(task.Owner);
            }
        }

        private void DrawWrap(ValueWrap w)
        {
            EditorGUILayout.LabelField(w.name, GUILayout.Width(60f));
            switch (w.type)
            {
                case ValueType.Int:
                    w.value = EditorGUILayout.IntField((int) w.value);
                    break;
                case ValueType.Float:
                    w.value = EditorGUILayout.FloatField((float) w.value);
                    break;
                case ValueType.String:
                    DrawStringField(w);
                    break;
                case ValueType.Vector2:
                    w.value = EditorGUILayout.Vector2Field(GUIContent.none, (Vector2) w.value, GUILayout.Width(PropWidth));
                    break;
                case ValueType.Vector3:
                    w.value = EditorGUILayout.Vector3Field(GUIContent.none, (Vector3) w.value, GUILayout.Width(PropWidth));
                    break;
                case ValueType.Vector4:
                    w.value = EditorGUILayout.Vector4Field(GUIContent.none, (Vector4) w.value, GUILayout.Width(PropWidth));
                    break;
                case ValueType.Rect:
                    w.value = EditorGUILayout.RectField(GUIContent.none, (Rect) w.value, GUILayout.Width(PropWidth));
                    break;
                case ValueType.Bounds:
                    w.value = EditorGUILayout.BoundsField(GUIContent.none, (Bounds) w.value, GUILayout.Width(PropWidth));
                    break;
                case ValueType.Color:
                    w.value = EditorGUILayout.ColorField((Color) w.value);
                    break;
                case ValueType.Curve:
                    w.value = EditorGUILayout.CurveField((AnimationCurve) w.value);
                    break;
                case ValueType.Bool:
                    w.value = EditorGUILayout.Toggle((bool) w.value);
                    break;
                case ValueType.Object:
                    var rect = EditorGUILayout.GetControlRect();
                    BindableValuesDrawerSlave.DrawObjectField(w, rect, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _info.Properties.TryGetValue(w.name, out var prop);
            if (prop != null && prop.literals.Count > 0)
            {
                if (GUILayout.Button(">", GUILayout.Width(20f), GUILayout.Height(14f)))
                {
                    BindableValuesDrawerSlave.GetMenu(prop.literalDescs, i =>
                    {
                        w.value = prop.literals[i];
                        Save();
                    }).ShowAsContext();
                }
            }
        }

        private void DrawStringField(ValueWrap w)
        {
            if (w.name == ActionBinder.PathKey)
            {
                var w1 = w;
                EditorGUILayout.BeginHorizontal();
                
                var rectStr = EditorGUILayout.GetControlRect(GUILayout.Width(PropWidth));
                PathRefDrawer.DrawField(rectStr, null, typeof(UnityEngine.Object), false, 
                    () => w1.value as string, s => w1.value = s);
                
                if (GUILayout.Button(">", GUILayout.Width(20f), GUILayout.Height(14f)))
                {
                    var list = new List<FileInfo>();
                    AssetUtil.GetFiles(LuaPath, list, "*.lua");
                    
                    var options = new List<string>();
                    foreach (var file in list)
                    {
                        if (file.DirectoryName != null && 
                            !file.DirectoryName.Contains("Action") && 
                            !file.DirectoryName.Contains("Condition"))
                        {
                            continue;
                        }
                        options.Add(AssetUtil.GetRelativePath(file.FullName).Replace(LuaPath, ""));
                    }

                    BindableValuesDrawerSlave.GetMenu(options, i =>
                    {
                        w.value = $"{LuaPath}{options[i]}";
                        Save();
                    }).ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (Equals(w.value, Vals[ActionBinder.PathKey])) { w.value = null; Save(); }
                w.value = EditorGUILayout.TextField((string) w.value);
            }
        }
        
        private void Save()
        {
            Vals.Regenerate(Vals.wraps);
            Value = Vals;
            BehaviorDesignerWindow.instance.SaveBehavior();
        }
        
        private void Init()
        {
            Vals.Init();
            
            var path = Vals[ActionBinder.PathKey] as string;
            
            _info = BindableInfo.FetchField(path);
            var wrap = new ValueWrap {name = ActionBinder.PathKey, type = ValueType.String};
            _info.wraps[wrap.name] = wrap;
            
            RemoveUndeclared();
            foreach (var w in _info.wraps.Values) AddField(w);
        }

        private void RemoveUndeclared()
        {
            var undeclared = new List<ValueWrap>();
            foreach (var w in Vals.wraps)
            {
                var exist = _info.wraps.ContainsKey(w.name);
                if (!exist) undeclared.Add(w);
            }

            foreach (var ud in undeclared) Vals.wraps.Remove(ud);
            Save();
        }

        private void AddField(ValueWrap w)
        {
            if (Vals == null) return;
            var exsit = false;
            foreach (var wrap in Vals.wraps)
            {
                if (w.name.Equals(wrap.name))
                {
                    exsit = true;
                    break;
                }
            }

            if (!exsit)
            {
                w.value = BindableValues.GetDefault(w.type);
                Vals.wraps.Add(w);
            }
        }
        
    }
}