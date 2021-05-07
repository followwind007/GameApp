using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameApp.AnimatorBehaviour
{
    [InitializeOnLoad]
    public class AnimatorBehaviourCreater : Editor
    {
        static AnimatorBehaviourCreater()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;
        }

        private static void HierarchyWindowItemCallback(int id, Rect rect)
        {
            if (Event.current.type == EventType.DragPerform && rect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.AcceptDrag();
                var selects = new List<GameObject>();
                foreach (var sel in DragAndDrop.objectReferences)
                {
                    if (sel is AnimatorData || sel is AnimatorDataOverride)
                    {
                        var gameObject = EditorUtility.InstanceIDToObject(id) as GameObject;
                        if (gameObject == null) continue;
                        
                        var behaviour = gameObject.GetOrAddComponent<AnimatorBehaviour>();

                        if (sel is AnimatorData data)
                        {
                            behaviour.Data = data;
                        }
                        else
                        {
                            behaviour.Overrides = sel as AnimatorDataOverride;
                        }
                        selects.Add(gameObject);
                    }
                }
                if (selects.Count == 0) return;
                Event.current.Use();
            }
        }
        
    }
}