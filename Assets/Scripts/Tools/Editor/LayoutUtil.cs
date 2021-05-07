using System;
using UnityEditor;
using UnityEngine;

namespace Tools
{
    public static class LayoutUtil
    {
        public const float WidthReorderTitleHead = 14f;
        public const float WidthGap = 2f;
        public const float Indent = 15f;
        public static Rect[] GetLayoutRects(Rect rect, float[] percents)
        {
            var rects = new Rect[percents.Length];
            var begin = 0f;
            for (var i = 0; i < percents.Length; i++)
            {
                var left = rect.x + rect.width * begin;
                var right = rect.x + rect.width * (begin + percents[i]);
                
                rects[i] = new Rect(left + 1, rect.y, right - left - 2, rect.height);
                begin += percents[i];
            }
            return rects;
        }
        
        public static int Toolbar(int selectedIndex, string[] tabNames, int indent)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent * Indent);
            var idx = GUILayout.Toolbar(selectedIndex, tabNames);
            EditorGUILayout.EndHorizontal();
            return idx;
        }

        public static bool Foldout(bool fold, string title, bool labelClick = false, int space = 3)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            var res = EditorGUILayout.Foldout(fold, title, labelClick);
            EditorGUILayout.EndHorizontal();
            return res;
        }

        public static void IndentLayout(int indent, Action action)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent * Indent);
            EditorGUILayout.BeginVertical();
            action?.Invoke();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        public static float GetIndent(int indentLevel)
        {
            return indentLevel * Indent;
        }
        
    }
}