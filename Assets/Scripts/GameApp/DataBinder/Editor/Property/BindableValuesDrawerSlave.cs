using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameApp.LuaResolver;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameApp.DataBinder
{
    public class BindableValuesDrawerSlave
    {
        private ReorderableList _list;

        private SerializedProperty _property;
        private SerializedProperty _wrapsProp;
        private Dictionary<ValueType, SerializedProperty> _listDict;
        
        private readonly float _slh = EditorGUIUtility.singleLineHeight;
        private const float NoticeHeight = 50f;

        private List<ValueWrap> _wraps;

        private BindableInfo _info = new BindableInfo();
        private bool IsStatic => _info.bindType == BindableInfo.BindType.Static;

        private bool _hasNotice;
        
        public void SetBindableInfo(BindableInfo info)
        {
            _info = info;
            if (info.Properties != null)
            {
                foreach (var prop in info.Properties.Values)
                {
                    if (prop.isPublic && info.wraps.TryGetValue(prop.name, out var w))
                    {
                        AddField(w);
                    }
                }
            }
        }

        public float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);
            return _hasNotice ? _list.GetHeight() + NoticeHeight : _list.GetHeight();
        }

        public void OnGui(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);
            _list.DoList(position);
            CheckValid(position);
        }
        
        private void AddField(ValueWrap val)
        {
            var exsit = false;
            foreach (var wrap in _wraps)
            {
                if (val.name.Equals(wrap.name))
                {
                    exsit = true;
                    break;
                }
            }

            if (!exsit)
            {
                SetDefaultValue(val);
                _wraps.Add(val);
                SerializeAll();
            }
        }

        private void Init(SerializedProperty property)
        {
            if (_list != null) return;
            _property = property;
            _listDict = new Dictionary<ValueType, SerializedProperty>
            {
                {ValueType.Int, property.FindPropertyRelative("ints")},
                {ValueType.Float, property.FindPropertyRelative("floats")},
                {ValueType.String, property.FindPropertyRelative("strings")},
                {ValueType.Vector2, property.FindPropertyRelative("vector2")},
                {ValueType.Vector3, property.FindPropertyRelative("vector3")},
                {ValueType.Vector4, property.FindPropertyRelative("vector4")},
                {ValueType.Rect, property.FindPropertyRelative("rects")},
                {ValueType.Bounds, property.FindPropertyRelative("bounds")},
                {ValueType.Color, property.FindPropertyRelative("colors")},
                {ValueType.Curve, property.FindPropertyRelative("curves")},
                {ValueType.Bool, property.FindPropertyRelative("bools")},
                {ValueType.Object, property.FindPropertyRelative("objects")},
            };
            _wrapsProp = property.FindPropertyRelative("wraps");

            InitWraps();
            _list = new ReorderableList(_wraps, typeof(ValueType))
            {
                drawElementCallback = DrawOptionData,
                drawHeaderCallback = rect => { GUI.Label(rect, "Bind Values"); },
                elementHeightCallback = GetValueLineHeight,
                onAddCallback = OnAddValue,
                onCanAddCallback = list => !IsStatic,
                onCanRemoveCallback = list => !IsStatic,
                onChangedCallback = delegate { SerializeAll(); }
            };
        }

        private float GetValueLineHeight(int index)
        {
            var val = _wraps[index];
            if (val.type == ValueType.Rect || val.type == ValueType.Bounds)
            {
                return EditorGUIUtility.singleLineHeight * 2 + 4;
            }
            return EditorGUIUtility.singleLineHeight * 1 + 4;
        }

        private void DrawOptionData(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.BeginChangeCheck();
            var val = _wraps[index];
            
            rect.y += 2;
            EditorGUI.BeginDisabledGroup(IsStatic);
            var rectName = new Rect(rect.x, rect.y, 150f, _slh);
            val.name = EditorGUI.TextField(rectName, val.name);
            EditorGUI.EndDisabledGroup();

            var extraSum = 190f + 75f;
            var hasLiteral = false;
            PropertyInfo info = null;
            var props = _info.Properties;
            
            if (!string.IsNullOrEmpty(val.name) && props != null && props.ContainsKey(val.name))
            {
                info = props[val.name];
                if (_info.wraps.TryGetValue(info.name, out var w))
                {
                    hasLiteral = w.type == val.type && info.literals.Count > 0;
                }
            }
            
            if (hasLiteral) extraSum += 25f;
            
            var rectValue = new Rect(190f, rect.y, rect.width - extraSum, _slh);
            
            switch (val.type)
            {
                case ValueType.Int:
                    val.value = EditorGUI.IntField(rectValue, (int)val.value);
                    break;
                case ValueType.Float:
                    val.value = EditorGUI.FloatField(rectValue, (float)val.value);
                    break;
                case ValueType.String:
                    var tmp = PathRefDrawer.DealEvent(rectValue, typeof(Object));
                    if (!string.IsNullOrEmpty(tmp)) val.value = tmp;
                    val.value = EditorGUI.TextField(rectValue, (string)val.value);
                    break;
                case ValueType.Vector2:
                    val.value = EditorGUI.Vector2Field(rectValue, GUIContent.none, (Vector2)val.value);
                    break;
                case ValueType.Vector3:
                    val.value = EditorGUI.Vector3Field(rectValue, GUIContent.none, (Vector3)val.value);
                    break;
                case ValueType.Vector4:
                    val.value = EditorGUI.Vector4Field(rectValue, GUIContent.none, (Vector4)val.value);
                    break;
                case ValueType.Rect:
                    val.value = EditorGUI.RectField(rectValue, (Rect)val.value);
                    break;
                case ValueType.Bounds:
                    val.value = EditorGUI.BoundsField(rectValue, (Bounds)val.value);
                    break;
                case ValueType.Color:
                    val.value = EditorGUI.ColorField(rectValue, (Color)val.value);
                    break;
                case ValueType.Curve:
                    val.value = EditorGUI.CurveField(rectValue, val.value as AnimationCurve);
                    break;
                case ValueType.Bool:
                    val.value = EditorGUI.Toggle(rectValue, (bool)val.value);
                    break;
                case ValueType.Object:
                    DrawObjectField(val, rectValue, SerializeAll);
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                SerializeAll();
            }
            
            if (hasLiteral)
            {
                var rectLiteral = new Rect(rect.width - extraSum + 195f, rect.y, 20f, _slh);
                if (GUI.Button(rectLiteral, ">"))
                {
                    GetMenu(info.literalDescs, i =>
                    {
                        val.value = info.literals[i];
                        SerializeAll();
                    }).ShowAsContext();
                }
            }
            
            var rectType = new Rect(rect.width - 70f, rect.y, 100f, _slh);
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(IsStatic);
            val.type = (ValueType)EditorGUI.EnumPopup(rectType, val.type);
            EditorGUI.EndDisabledGroup();
            if (!EditorGUI.EndChangeCheck()) return;
            
            SetDefaultValue(val);
            
            SerializeAll();
        }

        private void SetDefaultValue(ValueWrap val)
        {
            val.value = BindableValues.GetDefault(val.type);
        }

        public static void DrawObjectField(ValueWrap val, Rect rect, Action onChange)
        {
            var e = Event.current;
            if (rect.Contains(e.mousePosition) && DragAndDrop.objectReferences.Length > 0
                && (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)) 
			{
                var obj = DragAndDrop.objectReferences[0];
                if (e.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    DragAndDrop.AcceptDrag();
                }
                else if (e.type == EventType.DragPerform && obj is GameObject gameObject)
                {
                    var components = gameObject.GetComponents<Component>();
                    var objs = new List<Object> {gameObject};
                    objs.AddRange(components);

                    var options = new List<string> {gameObject.GetType().ToString()};
                    options.AddRange(components.Select(cmp => cmp.GetType().ToString()));
                    GetMenu(options.ToArray(), idx =>
                    {
                        val.value = objs[idx]; 
                        onChange?.Invoke();
                    }).ShowAsContext();
                }
                else if(e.type == EventType.DragPerform && obj)
                {
                    val.value = obj;
                    onChange?.Invoke();
                }
				e.Use();
			}
            EditorGUI.ObjectField(rect, (Object)val.value, typeof(Object), true);
        }
        
        public static GenericMenu GetMenu(IList<string> options, Action<int> callback)
        {
            var menu = new GenericMenu();
            for (var i = 0; i < options.Count; i++)
            {
                var index = i;
                menu.AddItem(new GUIContent(options[index]), false, () => callback(index));
            }
            return menu;
        }

        private void InitWraps()
        {
            _wraps = new List<ValueWrap>();
            for (var i = 0; i < _wrapsProp.arraySize; i++)
            {
                var prop = _wrapsProp.GetArrayElementAtIndex(i);
                var wrap = new ValueWrap()
                {
                    name = prop.FindPropertyRelative("name").stringValue,
                    type = (ValueType)prop.FindPropertyRelative("type").intValue,
                    index = prop.FindPropertyRelative("index").intValue,
                };
                wrap.value = GetValue(wrap.type, wrap.index);
                _wraps.Add(wrap);
            }
        }

        private object GetValue(ValueType type, int index)
        {
            object obj = null;
            switch (type)
            {
                case ValueType.Int:
                    obj = _listDict[ValueType.Int].GetArrayElementAtIndex(index).intValue;
                    break;
                case ValueType.Float:
                    obj = _listDict[ValueType.Float].GetArrayElementAtIndex(index).floatValue;
                    break;
                case ValueType.String:
                    obj = _listDict[ValueType.String].GetArrayElementAtIndex(index).stringValue;
                    break;
                case ValueType.Vector2:
                    obj = _listDict[ValueType.Vector2].GetArrayElementAtIndex(index).vector2Value;
                    break;
                case ValueType.Vector3:
                    obj = _listDict[ValueType.Vector3].GetArrayElementAtIndex(index).vector3Value;
                    break;
                case ValueType.Vector4:
                    obj = _listDict[ValueType.Vector4].GetArrayElementAtIndex(index).vector4Value;
                    break;
                case ValueType.Rect:
                    obj = _listDict[ValueType.Rect].GetArrayElementAtIndex(index).rectValue;
                    break;
                case ValueType.Bounds:
                    obj = _listDict[ValueType.Bounds].GetArrayElementAtIndex(index).boundsValue;
                    break;
                case ValueType.Color:
                    obj = _listDict[ValueType.Color].GetArrayElementAtIndex(index).colorValue;
                    break;
                case ValueType.Curve:
                    obj = _listDict[ValueType.Curve].GetArrayElementAtIndex(index).animationCurveValue;
                    break;
                case ValueType.Bool:
                    obj = _listDict[ValueType.Bool].GetArrayElementAtIndex(index).boolValue;
                    break;
                case ValueType.Object:
                    obj = _listDict[ValueType.Object].GetArrayElementAtIndex(index).objectReferenceValue;
                    break;
            }
            return obj;
        }

        private void SerializeAll()
        {
            foreach (var kv in _listDict)
                kv.Value.ClearArray();
            _wrapsProp.ClearArray();

            foreach (var wrap in _wraps)
            {
                _wrapsProp.InsertArrayElementAtIndex(_wrapsProp.arraySize);
                var index = 0;
                var val = wrap.value;
                switch (wrap.type)
                {
                    case ValueType.Int:
                        InsertProp(ValueType.Int, out index).intValue = (int) val;
                        break;
                    case ValueType.Float:
                        InsertProp(ValueType.Float, out index).floatValue = (float) val;
                        break;
                    case ValueType.String:
                        InsertProp(ValueType.String, out index).stringValue = (string) val;
                        break;
                    case ValueType.Vector2:
                        InsertProp(ValueType.Vector2, out index).vector2Value = (Vector2) val;
                        break;
                    case ValueType.Vector3:
                        InsertProp(ValueType.Vector3, out index).vector3Value = (Vector3) val;
                        break;
                    case ValueType.Vector4:
                        InsertProp(ValueType.Vector4, out index).vector4Value = (Vector4) val;
                        break;
                    case ValueType.Rect:
                        InsertProp(ValueType.Rect, out index).rectValue = (Rect) val;
                        break;
                    case ValueType.Bounds:
                        InsertProp(ValueType.Bounds, out index).boundsValue = (Bounds) val;
                        break;
                    case ValueType.Color:
                        InsertProp(ValueType.Color, out index).colorValue = (Color) val;
                        break;
                    case ValueType.Curve:
                        InsertProp(ValueType.Curve, out index).animationCurveValue = (AnimationCurve) val;
                        break;
                    case ValueType.Bool:
                        InsertProp(ValueType.Bool, out index).boolValue = (bool) val;
                        break;
                    case ValueType.Object:
                        InsertProp(ValueType.Object, out index).objectReferenceValue = (Object) val;
                        break;
                }
                var wrapProp = _wrapsProp.GetArrayElementAtIndex(_wrapsProp.arraySize - 1);
                wrapProp.FindPropertyRelative("name").stringValue = wrap.name;
                wrapProp.FindPropertyRelative("type").intValue = (int)wrap.type;
                wrapProp.FindPropertyRelative("index").intValue = index;
            }
            _property.serializedObject.ApplyModifiedProperties();
        }

        private void OnAddValue(ReorderableList list)
        {
            var last = _wraps.Count > 0 ? _wraps[_wraps.Count - 1] : null;
            var val = new ValueWrap()
            {
                type = last?.type ?? ValueType.Int,
                value = last == null ? 0 : last.value,
            };
            _wraps.Add(val);
            SerializeAll();
        }
        
        private SerializedProperty InsertProp(ValueType type, out int index)
        {
            var prop = _listDict[type];
            prop.InsertArrayElementAtIndex(prop.arraySize);
            index = prop.arraySize - 1;
            return prop.GetArrayElementAtIndex(prop.arraySize - 1);
        }

        private void CheckValid(Rect position)
        {
            var builder = new StringBuilder();

            var keyDict = new Dictionary<string, int>();
            var sameKeys = new HashSet<string>();
            var countEmptyKey = 0;
            foreach (var wrap in _wraps)
            {
                if (string.IsNullOrEmpty(wrap.name))
                {
                    countEmptyKey++;
                    continue;
                }
                keyDict[wrap.name] = keyDict.ContainsKey(wrap.name) ? keyDict[wrap.name] + 1 : 1;
                if (keyDict[wrap.name] > 1)
                {
                    sameKeys.Add(wrap.name);
                }
            }

            if (countEmptyKey > 0)
            {
                builder.AppendLine(string.Format("Empty key: {0}", countEmptyKey));
            }

            if (sameKeys.Count > 0)
            {
                builder.Append("Same key: ");
                foreach (var key in sameKeys)
                {
                    builder.Append(key + " ");
                }
                builder.Append("\n");
            }

            _hasNotice = builder.Length > 0;
            if (_hasNotice)
            {
                var rect = new Rect(position.x, position.y + position.height - NoticeHeight, position.width, NoticeHeight);
                EditorGUI.HelpBox(rect, builder.ToString(), MessageType.Warning);
            }
        }

    }
}