using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using GameApp.Pool;
using UnityEngine.Events;

namespace GameApp.EmojiText
{
    public class EmojiText : Text
    {
        public Action<GameObject, TagData> onCreateIcon;
        public Action<GameObject, TagData> onCreateButton;
        public Action<GameObject, TagData> onCreateEmoji;
        public Action<GameObject, TagData> onCreateLink;

        private static ObjectPool _poolIcon;
        private static ObjectPool _poolButton;
        private static ObjectPool _poolEmoji;
        private static ObjectPool _poolLink;
        
        private readonly List<GameObject> _showIcons = new List<GameObject>();
        private readonly List<GameObject> _showButtons = new List<GameObject>();
        private readonly List<GameObject> _showLinks = new List<GameObject>();
        private readonly List<GameObject> _showEmojis = new List<GameObject>();

        private readonly Dictionary<int, TagData> _tagDict = new Dictionary<int, TagData>();
        
        protected string _lastText = string.Empty;
        protected int _lastSize = -1;

        private static readonly Regex SRegex = new Regex(@"<t=(.+?)>", RegexOptions.Singleline);
        private static readonly Regex RichRegex = new Regex(@"<(.+?)>", RegexOptions.Singleline);
        private static readonly Regex RichClosureRegex = new Regex(@"</(.+?)>", RegexOptions.Singleline);

        protected bool _isDirty;
        protected bool _populated;

        //unit test code
        //protected string _polished = "";
        public override string text
        {
            set
            {
                _lastText = value;
                _lastSize = fontSize;
                base.text = Parse(value);
                
                /* //unit test code
                 _polished = RichRegex.Replace(base.text, "");
                _polished = RichClosureRegex.Replace(_polished, "");
                _polished = _polished.Replace(" ", "");*/
                
                _isDirty = true;
                _populated = false;
                
                SetVerticesDirty();
                SetLayoutDirty();
            }
        }
        public string basetext
        {
            set => base.text = value;
            get => base.text;
        }
        public virtual TextGenerationSettings setting { get => GetGenerationSettings(rectTransform.rect.size); }
        protected override void Awake()
        {
            base.Awake();
            if (!Application.isPlaying) return;
            if (_poolIcon == null)
            {
                _poolIcon = Pool.Pool.Create("EMOJI_ICON", GetTemplate(TagData.TYPE_ICON));
            }
            
            if (_poolButton == null)
            {
                _poolButton = Pool.Pool.Create("EMOJI_BUTTON", GetTemplate(TagData.TYPE_BUTTON));
            }
            
            if (_poolEmoji == null)
            {
                _poolEmoji = Pool.Pool.Create("EMOJI_EMOJI", GetTemplate(TagData.TYPE_EMOJI));
            }

            if (_poolLink == null)
            {
                _poolLink = Pool.Pool.Create("EMOJI_LINK", GetTemplate(TagData.TYPE_LINK));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onCreateButton = null;
            onCreateEmoji = null;
            onCreateIcon = null;
            onCreateLink = null;
        }

        private GameObject GetTemplate(string t)
        {
            var go = new GameObject(t);
            var rectTrans = go.AddComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.zero;
            rectTrans.sizeDelta = new Vector2(100f, 100f);

            switch (t)
            {
                case TagData.TYPE_ICON:
                    go.AddComponent<RawImage>();
                    break;
                case TagData.TYPE_BUTTON:
                    var img = go.AddComponent<Image>();
                    AddButton(go, img);
                    break;
                case TagData.TYPE_EMOJI:
                    break;
                case TagData.TYPE_LINK:
                    var hotArea = go.AddComponent<HotArea>();
                    AddButton(go, hotArea);
                    break;
            }
            
            return go;
        }

        private void AddButton(GameObject go, Graphic graphic)
        {
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = graphic;
            var btnHelper = go.AddComponent<ButtonHelper>();
            btnHelper.checkLongPress = true;
        }
        protected string Parse(string sourceString)
        {
            #if !TEMPLATE_MODE
            if (Localization.GetLocale() == Localization.LocaleZh)
            {
                sourceString = sourceString.Replace(" ","\u00A0");
            }
            #endif
            
            _tagDict?.Clear();
            
            var mc = SRegex.Matches(sourceString);
            if (mc.Count <= 0)
            {
                return sourceString;
            }
            
            var sb = new StringBuilder();
            
            var tagStartIndex = 0;
            var lastMatchEndIndex = 0;
            
            for (var i = 0; i < mc.Count; i++)
            {
                var match = mc[i];

                var stringBeforeTag = sourceString.Substring(lastMatchEndIndex, match.Index - lastMatchEndIndex);
                sb.Append(stringBeforeTag);
                stringBeforeTag = RichRegex.Replace(stringBeforeTag, "");
                stringBeforeTag = RichClosureRegex.Replace(stringBeforeTag, "");
                tagStartIndex += GetMeshedTextLength(stringBeforeTag);
                
                var t = new TagData(match.Value, fontSize) {StartIndex = tagStartIndex};
                
                _tagDict?.Add(tagStartIndex, t);
                //Debug.Log($"{t.Text}:{t.StartIndex}-{t.EndIndex}");

                sb.Append(t.PopulateText);
                tagStartIndex += t.HostLength;

                lastMatchEndIndex = match.Index + match.Length;
            }

            if (lastMatchEndIndex < sourceString.Length)
            {
                sb.Append(sourceString.Substring(lastMatchEndIndex, sourceString.Length - lastMatchEndIndex));
            }
            
            return sb.ToString();
        }
        private static readonly Regex MeshRegex = new Regex(@"(\u3000|\u0020|\u0009|\u000d|\u000a|\u000b|\u000f)*", RegexOptions.Singleline);
        public static int GetMeshedTextLength(string input)
        {
            return string.IsNullOrEmpty(input) ? 0 : MeshRegex.Replace(input, "").Length;
        }

        public static string GetParsedText(string sourceString, UnityAction<List<TagData>> tagsCall = null)
        {
            var tags = new List<TagData>();
            if (string.IsNullOrEmpty(sourceString))
            {
                tagsCall?.Invoke(tags);
                return sourceString;
            }
            var mc = SRegex.Matches(sourceString);
            StringBuilder sb = null;

            var tagStartIndex = 0;
            var lastMatchEndIndex = 0;

            for (var i = 0; i < mc.Count; i++)
            {
                var match = mc[i];
                var quadData = new TagData(match.Value, default);
                if (sb == null) sb = new StringBuilder();

                var stringBeforeTag = sourceString.Substring(lastMatchEndIndex, match.Index - lastMatchEndIndex);
                sb.Append(stringBeforeTag);
                tagStartIndex += stringBeforeTag.Length;
                quadData.StartIndex = tagStartIndex;

                tags.Add(quadData);
                sb.Append(quadData.PopulateText);
                tagStartIndex += quadData.PopulateText.Length;

                lastMatchEndIndex = match.Index + match.Length;
                if (i == mc.Count - 1 && lastMatchEndIndex < sourceString.Length)
                {
                    sb.Append(sourceString.Substring(lastMatchEndIndex, sourceString.Length - lastMatchEndIndex));
                }
            }

            tagsCall?.Invoke(tags);
            return sb != null ? sb.ToString() : sourceString;
        }
        
        private void Update()
        {
            if (!Application.isPlaying) return;
            
            if (_lastSize != -1 && _lastSize != fontSize)
            {
                _lastSize = fontSize;
                text = _lastText;
            }

            if (_isDirty && _populated)
            {
                _isDirty = false;
                _populated = false;
                SetVerticesDirty();
                StartCoroutine(CreateTag());
            }
        }
        
        private IEnumerator CreateTag()
        {
            yield return null;
            DirectRecycleAll();
            if (Application.isPlaying && _tagDict != null)
            {
                foreach (var tagData in _tagDict.Values)
                {
                    if (!tagData.Valid)
                    {
                        Debug.LogWarning($"Create Tag Invalid {tagData.MatchText}");
                        continue;
                    }
                    
                    switch (tagData.T)
                    {
                        case TagData.TYPE_ICON:
                            CreateIcon(tagData);
                            break;
                        case TagData.TYPE_EMOJI:
                            CreateEmoji(tagData);
                            break;
                        case TagData.TYPE_BUTTON:
                            CreateButton(tagData);
                            break;
                        case TagData.TYPE_LINK:
                            Createlink(tagData);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                } 
            }
        }
        
        private void DirectRecycleAll()
        {
            if (!Application.isPlaying) return;
            _poolIcon.Recycle(_showIcons);
            _showIcons.Clear();
            _poolButton.Recycle(_showButtons);
            _showButtons.Clear();
            _poolEmoji.Recycle(_showEmojis);
            _showEmojis.Clear();
            foreach (var link in _showLinks)
            {
                if (link != null) UIEventCleaner.RemoveButton(link.gameObject);
            }
            _poolLink.Recycle(_showLinks);
            _showLinks.Clear();
        }

        private void CreateIcon(TagData tagData)
        {
            var go = _poolIcon.Fetch();
            
            var rectTrans = go.GetComponent<RectTransform>();
            InitElement(rectTrans);
            rectTrans.SetParent(transform);
            rectTrans.sizeDelta = tagData.Size;
            rectTrans.anchoredPosition3D = tagData.StartPosition;
            
            RawImage rawImage = go.GetComponent<RawImage>();
            if (tagData.Color != null)
            {
                ColorUtility.TryParseHtmlString(tagData.Color, out var c);
                rawImage.color = c;
            }
            else
                rawImage.color = Color.white;

            onCreateIcon?.Invoke(go, tagData);
            _showIcons.Add(go);
        }

        private void CreateButton(TagData tagData)
        {
            var go = _poolButton.Fetch();

            var rectTrans = go.GetComponent<RectTransform>();
            InitElement(rectTrans);
            rectTrans.sizeDelta = tagData.Size;
            rectTrans.anchoredPosition3D = tagData.StartPosition;

            onCreateButton?.Invoke(go, tagData);
            _showButtons.Add(go);
        }

        private void Createlink(TagData tagData)
        {
            var boundList = tagData.BoundList;
            if (boundList == null || boundList.Count == 0)
            {
                Debug.LogWarning($"can not find bounds in {tagData.MatchText}");
                return;
            }
            foreach (var bound in boundList)
            {
                var go = _poolLink.Fetch();
                var rectTrans = go.GetComponent<RectTransform>();
                InitElement(rectTrans);
                rectTrans.sizeDelta = new Vector2(bound.z - bound.x, bound.w - bound.y);
                rectTrans.anchoredPosition3D = new Vector3(bound.x, bound.y, 0);
                
                DealUnderline(go, tagData);
                
                onCreateLink?.Invoke(go, tagData);
                _showLinks.Add(go);
            }
        }

        private void DealUnderline(GameObject go, TagData tagData)
        {
            var underTrans = go.transform.Find(TagData.TYPE_UNDERLINE) as RectTransform;
            if (tagData.HasUnderline)
            {
                RawImage img;
                if (underTrans == null)
                {
                    var underGo = new GameObject(TagData.TYPE_UNDERLINE);
                    underTrans = underGo.AddComponent<RectTransform>();
                    img = underGo.AddComponent<RawImage>();
                    
                    underTrans.SetParentFull(go.transform);

                    underTrans.anchorMin = new Vector2(0f, 0f);
                    underTrans.anchorMax = new Vector2(1f, 0f);
                }
                else
                {
                    img = underTrans.gameObject.GetComponent<RawImage>();
                }
                
                //height
                underTrans.anchoredPosition = new Vector2(0, -tagData.UnderlineHeight);
                underTrans.sizeDelta = new Vector2(0, tagData.UnderlineHeight);
                
                //color                
                if (tagData.UnderlineColor != null)
                {
                    ColorUtility.TryParseHtmlString(tagData.UnderlineColor, out var col);
                    img.color = col;
                }
                else
                {
                    img.color = Color.green;
                }

                underTrans.gameObject.SetActive(true);
            }
            else if (underTrans != null)
            {
                underTrans.gameObject.SetActive(false);
            }
        }

        public void CreateEmoji(TagData tagData)
        {
            var go = _poolEmoji.Fetch();
            var rectTrans = go.GetComponent<RectTransform>();
            InitElement(rectTrans);
            rectTrans.sizeDelta = tagData.Size;
            rectTrans.anchoredPosition3D = tagData.StartPosition;

            onCreateEmoji?.Invoke(go, tagData);
            _showEmojis.Add(go);
        }
        private void InitElement(RectTransform rectTrans)
        {
            rectTrans.SetParent(transform);
            rectTrans.localScale = Vector3.one;
            rectTrans.localRotation = Quaternion.identity;
            rectTrans.pivot = Vector2.zero;
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.zero;
        }
        
        protected Vector4 GetCharBound()
        {
            var result = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
            for (var i = 0; i < 4; i++)
            {
                var position = _tempVerts[i].position;
                if (position.x < result.x) result.x = position.x;
                if (position.y < result.y) result.y = position.y;
                if (position.x > result.z) result.z = position.x;
                if (position.y > result.w) result.w = position.y;
            }
            return result;
        }

        private readonly UIVertex[] _tempVerts = new UIVertex[4];
        private bool IsTempVertexValid()
        {
            var vector0 = _tempVerts[0].position;
            var vector1 = _tempVerts[1].position;
            var valid = !(Vector2.Distance(vector0, vector1) < 0.01f);
            return valid;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null) return;
            
            _populated = true;
            m_DisableFontTextureRebuiltCallback = true;
            cachedTextGenerator.PopulateWithErrors(basetext, setting, gameObject);

            // Apply the offset to the vertices
            var rawVerts = cachedTextGenerator.verts;
            var verts = new List<UIVertex>();
            for (var i = 0; i < rawVerts.Count; i++)
            {
                var idx = i & 3;
                _tempVerts[idx] = rawVerts[i];
                if (idx != 3) continue;
                if (IsTempVertexValid())
                {
                    verts.Add(_tempVerts[0]);
                    verts.Add(_tempVerts[1]);
                    verts.Add(_tempVerts[2]);
                    verts.Add(_tempVerts[3]);
                }
            }
            
            var unitsPerPixel = 1 / pixelsPerUnit;
            var vertCount = verts.Count;
            
            toFill.Clear();
            if (vertCount <= 0) return;

            var roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;

            if (_tagDict != null) foreach (var tagData in _tagDict.Values) tagData.Valid = false;
            TagData linkTag = null;
            
            var xOffset = rectTransform.sizeDelta.x * rectTransform.pivot.x;
            var yOffset = rectTransform.sizeDelta.y * rectTransform.pivot.y;
            //Debug.Log($"{_polished.Length}:{xOffset},{yOffset};{rectTransform.sizeDelta};{rectTransform.pivot}");

            var buttonBound = new Vector4(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
            var buttonBoundInit = false;
            var lastCharMinY = float.MaxValue;

            for (var i = 0; i < vertCount; ++i)
            {
                var tempVertsIndex = i & 3;
                _tempVerts[tempVertsIndex] = verts[i];
                _tempVerts[tempVertsIndex].position *= unitsPerPixel;
                if (roundingOffset != Vector2.zero)
                {
                    _tempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    _tempVerts[tempVertsIndex].position.y += roundingOffset.y;
                }
                if (tempVertsIndex != 3) continue;
                
                var curIdx = i >> 2;
                
                //Debug.Log($"{_polished[curIdx]}|{curIdx}: {_tempVerts[0].position} {_tempVerts[1].position} {_tempVerts[2].position} {_tempVerts[3].position}");
                
                if (_tagDict != null && _tagDict.ContainsKey(curIdx))
                {
                    var tagData = _tagDict[curIdx];
                    tagData.Valid = true;
                    if (tagData.UseQuad)
                    {
                        var minX = float.MaxValue;
                        var minY = float.MaxValue;
                        for (var j = 0; j < 4; j++)
                        {
                            _tempVerts[j].uv0 = Vector2.zero;//清除占位符显示 也可以用<color=#00000000><quad></color>来隐藏

                            if (_tempVerts[j].position.x < minX) minX = _tempVerts[j].position.x;//获取占位符左下角坐标
                            if (_tempVerts[j].position.y < minY) minY = _tempVerts[j].position.y;//获取占位符左下角坐标
                        }
                        tagData.SetStartPosition(new Vector3(minX + xOffset, minY + yOffset, 0));
                    }
                    else if (tagData.T == TagData.TYPE_LINK)
                    {
                        tagData.ClearBound();
                        linkTag = tagData;
                        if (IsTempVertexValid())
                        {
                            var charBound = GetCharBound();
                            buttonBound = charBound;
                            lastCharMinY = charBound.y;
                            buttonBoundInit = true;
                        }
                    }
                }
                
                if (linkTag != null && curIdx < linkTag.EndIndex)
                {
                    CalculateHyperlinkBound(linkTag, ref buttonBound, ref buttonBoundInit, ref lastCharMinY, xOffset, yOffset, curIdx);
                }

                toFill.AddUIVertexQuad(_tempVerts);
            }
            m_DisableFontTextureRebuiltCallback = false;  
        }

        private void CalculateHyperlinkBound(TagData hyperlinkTagData, ref Vector4 buttonBound, ref bool buttonBoundInit, ref float lastCharMinY, float xOffset, float yOffset, int stringIndex)
        {
            if (IsTempVertexValid())
            {
                var charBound = GetCharBound();
                if (buttonBoundInit)
                {
                    if (Math.Abs(lastCharMinY - float.MaxValue) > -0.01f && lastCharMinY > charBound.w)
                    {
                        buttonBound.x += xOffset;
                        buttonBound.y += yOffset;
                        buttonBound.z += xOffset;
                        buttonBound.w += yOffset;
                        hyperlinkTagData.AddBound(buttonBound);
                        lastCharMinY = charBound.y;
                        buttonBound = charBound;
                    }
                    else
                    {
                        lastCharMinY = charBound.y;
                        if (charBound.x < buttonBound.x) buttonBound.x = charBound.x;
                        if (charBound.y < buttonBound.y) buttonBound.y = charBound.y;
                        if (charBound.z > buttonBound.z) buttonBound.z = charBound.z;
                        if (charBound.w > buttonBound.w) buttonBound.w = charBound.w;
                    }
                }
                else
                {
                    buttonBound = charBound;
                    lastCharMinY = charBound.y;
                    buttonBoundInit = true;
                }
            }

            if (hyperlinkTagData.EndIndex - 1 == stringIndex)
            {
                if (buttonBoundInit)
                {
                    buttonBound.x += xOffset;
                    buttonBound.y += yOffset;
                    buttonBound.z += xOffset;
                    buttonBound.w += yOffset;
                    hyperlinkTagData.AddBound(buttonBound);
                }
                buttonBoundInit = false;
            }
        }

        
    }
}