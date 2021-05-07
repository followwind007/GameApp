
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.ResCheck
{
    public class EntryManipulator : MouseManipulator
    {
        private readonly string _item;
        private readonly object _data;
        
        public EntryManipulator(string assetPath, object data = null)
        {
            _item = assetPath;
            _data = data;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(_item);
            if (_data != null)
            {
                ResCheckWindow.resCheckView.RefreshDetail(_data);
            }
        }
        

    }
}