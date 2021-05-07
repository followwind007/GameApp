using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameApp.EmojiText
{
    [RequireComponent(typeof(TMP_Text))]
    public class EmojiTextTMP : MonoBehaviour, IPointerClickHandler
    {
        public class TagEvent : UnityEvent<TagDataTMP> { };
        public readonly TagEvent onClickTag = new TagEvent();

        public Camera renderCamera;
        
        private readonly List<TagDataTMP> _allTags = new List<TagDataTMP>();
        private readonly List<TagDataTMP> _linkTags = new List<TagDataTMP>();

        private TMP_Text _tmpText;

        [SerializeField]
        private string currentText;
        public string text
        {
            get => _tmpText.text;
            set => SetText(value);
        }

        protected void Awake()
        {
            _tmpText = GetComponent<TMP_Text>();
            
            if (renderCamera == null && _tmpText is TextMeshProUGUI tmpUI)
            {
                renderCamera = tmpUI.canvas.worldCamera;
            }
            
            if (!string.IsNullOrEmpty(currentText))
            {
                text = currentText;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            DispatchEvent();
        }

        private void SetText(string value)
        {
#if UNITY_EDITOR
            currentText = value;
#endif
            _allTags.Clear();
            _linkTags.Clear();
            
            var parsed = Parse(value);
            _tmpText.text = parsed;

            foreach (var t in _allTags)
            {
                if (t.T == TagDataTMP.TypeLink)
                {
                    _linkTags.Add(t);
                }
            }
        }

        private void DispatchEvent()
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_tmpText, UnityEngine.Input.mousePosition, renderCamera);
            if (linkIndex < 0) return;
            
            if (linkIndex < _linkTags.Count)
            {
                onClickTag.Invoke(_linkTags[linkIndex]);
            }
            else
            {
                Debug.LogError($"Link index ({linkIndex}) bigger than link count ({_linkTags})");
            }
        }
        
        private static readonly Regex SRegex = new Regex(@"<t=(.+?)>", RegexOptions.Singleline);
        public string Parse(string sourceString)
        {
#if !TEMPLATE_MODE
            if (Localization.GetLocale() == Localization.LocaleZh)
            {
                sourceString = sourceString.Replace(" ","\u00A0");
            }
#endif
            var parsed = SRegex.Replace(sourceString, (m) =>
            {
                var t = new TagDataTMP(m.Value);
                _allTags.Add(t);
                return t.populateText;
            });

            return parsed;
        }
        
    }
}