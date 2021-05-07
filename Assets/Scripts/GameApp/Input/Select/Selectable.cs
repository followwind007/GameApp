using System;
using UnityEngine;

namespace GameApp.Input
{
    public class Selectable : MonoBehaviour, ISelectable
    {
        public Action<float> onSelect;

        public Action onDeselect;
        
        public UnityEngine.UI.Selectable uiSelectable;

        private float _selectCount;
        
        public void Select()
        {
            _selectCount = 0f;
            onSelect?.Invoke(_selectCount);
            if (uiSelectable)
            {
                uiSelectable.OnSelect(null);
            }
        }

        public void Continue()
        {
            _selectCount += Time.deltaTime;
            onSelect?.Invoke(_selectCount);
        }

        public void Deselect()
        {
            _selectCount = 0f;
            onDeselect?.Invoke();
            if (uiSelectable)
            {
                uiSelectable.OnDeselect(null);
            }
        }
        
    }
}