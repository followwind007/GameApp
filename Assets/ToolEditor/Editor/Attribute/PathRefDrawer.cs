using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(PathRef))]
public class PathRefDrawer : PropertyDrawer
{
    private readonly float _propHeight = EditorGUIUtility.singleLineHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (PathRef) attribute;
        DrawField(position, label, attr.PropType, attr.ShowTitle,() => property.stringValue, s => property.stringValue = s );
    }

    public static void DrawField(Rect position, GUIContent label, Type t, bool showTitle, Func<string> onGet, Action<string> onChange)
    {
        var bfColor = GUI.color;
        
        DealEvent(position, t, onChange);

        if (showTitle)
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        }

        position.width -= 30;
        
        GUI.color = new Color(0f, 0.86f, 1f);
        if (string.IsNullOrEmpty(onGet()))
        {
            EditorGUI.TextField(position,string.Format("{0}", t));
        }
        else
        {
            var path = EditorGUI.TextField(position, onGet());
            onChange(path);
        }
        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(onGet()));
        if (GUI.Button(new Rect(position.x + position.width + 5, position.y - 1, 25, position.height) , "@"))
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(onGet());
        }
        EditorGUI.EndDisabledGroup();

        GUI.color = bfColor;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return _propHeight * 1;
    }

    public static string DealEvent(Rect position, Type targetType, Action<string> onChange, bool usePath = false)
    {
        string path = null;
        var e = Event.current;
        if ((e.type == EventType.DragUpdated || e.type == EventType.DragPerform) 
            && position.Contains(e.mousePosition) && DragAndDrop.objectReferences.Length > 0)
        {
            var selectObj = DragAndDrop.objectReferences[0];
            
            if (selectObj.GetType() == targetType || 
                CheckExceptionType(selectObj.GetType(), targetType))
            {
                if (e.type == EventType.DragPerform)
                {
                    if (usePath)
                    {
                        path = DragAndDrop.paths[0];
                    }
                    else
                    {
                        onChange?.Invoke(DragAndDrop.paths[0]);
                    }
                    GUI.changed = true;
                }
                else if (e.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    DragAndDrop.AcceptDrag();
                }
                e.Use();
            }
        }

        return path;
    }

    public static string DealEvent(Rect position, Type targetType)
    {
        return DealEvent(position, targetType, null, true);
    }

    private static bool CheckExceptionType(Type typeSelect, Type typeTarget)
    {
        if (typeTarget == typeof(Object)) return true;
        
        return (typeSelect == typeof(RuntimeAnimatorController) || typeSelect == typeof(AnimatorController)) 
               && typeTarget == typeof(RuntimeAnimatorController);
    }

}