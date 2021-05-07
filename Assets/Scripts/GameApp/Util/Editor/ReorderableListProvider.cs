using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GameApp.Util
{
    public class ReorderableListProvider
    {
        public static ReorderableList CreateReorderbaleList(IList list, Type type, string head = null, Action<Rect, int> onDrawElement = null)
        {
            const float linePadding = 2;

            var lineHeight = EditorGUIUtility.singleLineHeight + linePadding;
            
            var hasHeader = !string.IsNullOrEmpty(head);
            
            var rl = new ReorderableList(list, type, true, false, true, true)
            {
                elementHeight = lineHeight,
                drawElementCallback = (rect, index, active, focused) =>
                {
                    rect.y -= linePadding / 2;
                    rect.height -= linePadding;
                    onDrawElement?.Invoke(rect, index);
                },
                headerHeight = hasHeader ? lineHeight : linePadding
            };
            
            if (hasHeader)
            {
                rl.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, head); };
            }
            
            return rl;
        }
    }
}