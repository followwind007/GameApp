using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameApp.AssetProcessor.Drawer.Property
{
    public interface ICheckerSelector
    {
        
    }

    public static class CheckerSelectorImpl
    {
        public static string ArrayToString(this ICheckerSelector selector, SerializedProperty prop)
        {
            var str = "";
            for (var i = 0; i < prop.arraySize; i++)
            {
                str += $"{prop.GetArrayElementAtIndex(i).stringValue},";
            }
            return str;
        }
        
        public static void StringToArray(this ICheckerSelector selector, SerializedProperty prop, string content)
        {
            prop.ClearArray();
            var parts = content.Split(',');
            foreach (var p in parts)
            {
                if (!string.IsNullOrEmpty(p))
                {
                    prop.InsertArrayElementAtIndex(prop.arraySize);
                    var ele = prop.GetArrayElementAtIndex(prop.arraySize - 1);
                    ele.stringValue = p;
                }
            }
        }
        
        public static GenericMenu GetCheckerMenu(this ICheckerSelector selector, SerializedProperty prop, Func<List<string>> getCheckerIds)
        {
            var gm = new GenericMenu();
            var checkers = getCheckerIds();
            
            foreach (var attrId in checkers)
            {
                var idx = -1;
                for (var i = 0; i < prop.arraySize; i++)
                {
                    var id = prop.GetArrayElementAtIndex(i).stringValue;
                    if (id == attrId)
                    {
                        idx = i;
                        break;
                    }
                }
                gm.AddItem(new GUIContent(attrId),idx >= 0, () =>
                {
                    if (idx >= 0)
                    {
                        prop.DeleteArrayElementAtIndex(idx);
                    }
                    else
                    {
                        prop.InsertArrayElementAtIndex(prop.arraySize);
                        prop.GetArrayElementAtIndex(prop.arraySize - 1).stringValue = attrId;
                    }

                    prop.serializedObject.ApplyModifiedProperties();
                });
            }
            
            return gm;
        }

    }
}