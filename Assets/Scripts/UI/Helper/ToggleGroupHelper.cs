using UnityEngine.Events;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(ToggleGroup))]
    public class ToggleGroupHelper : MonoBehaviour
    {
        public RectTransform content;

        public int defaultActiveIndex;
        
        [System.Serializable]
        public class ToggleGroupSelectedEvent : UnityEvent<int> { }
        public ToggleGroupSelectedEvent onToggleGroupSelected = new ToggleGroupSelectedEvent();

        public System.Action<int> onToggleGroupSelectedAction;

        private ToggleGroup _group;
        private List<Toggle> _toggles;

        private int _lastActiveIndex = -1;
        
        private void Awake()
        {
            _group = GetComponent<ToggleGroup>();
            if (!content)
            {
                Debug.LogError("Toggle group helper, content is null");
            }
            _toggles = new List<Toggle>();
            Init();
        }

        public void Init()
        {
            if (content == null) return;
            _toggles.Clear();
            for (var i = 0; i < content.childCount; i++)
            {
                var toggle = content.GetChild(i).GetComponent<Toggle>();
                if (toggle == null) continue;

                toggle.group = _group;
                
                _toggles.Add(toggle);
                toggle.onValueChanged.RemoveListener(OnToggleValueChange);
                toggle.onValueChanged.AddListener(OnToggleValueChange);
            }

            if (_toggles.Count > 0)
            {
                var activeIndex = Mathf.Clamp(defaultActiveIndex, 0, _toggles.Count - 1);
                _group.NotifyToggleOn(_toggles[activeIndex]);
            }
        }

        private void OnDestroy()
        {
            foreach (var toggle in _toggles)
            {
                toggle.onValueChanged.RemoveListener(OnToggleValueChange);
            }
        }

        private void OnToggleValueChange(bool val)
        {
            if (!val) return;
            var actives = _group.ActiveToggles();
            foreach (var toggle in actives)
            {
                var index = _toggles.IndexOf(toggle);
                if (_lastActiveIndex != index)
                {
                    _lastActiveIndex = index;
                    onToggleGroupSelected.Invoke(index);
                    if (onToggleGroupSelectedAction != null)
                    {
                        onToggleGroupSelectedAction(index);
                    }
                }
            }
        }
        
        
    }

}