using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp.EmojiText
{
    public class TagData
    {
        public const float ICON_SCALE = 1.5f;
        public const float EMOJI_SCALE = 1.5f;
        public const float BUTTON_SCALE = 2f;

        public const string TYPE_UNDERLINE = "underline";

        public const string TYPE_ICON = "icon";
        public const string TYPE_EMOJI = "emoji";
        public const string TYPE_BUTTON = "button";
        public const string TYPE_LINK = "link";

        public const string KEY_TYPE = "type";
        public const string KEY_T = "t";
        public const string KEY_ID = "id";
        public const string KEY_TEXT = "text";
        public const string KEY_COLOR = "color";
        public const string KEY_CONTENT = "content";
        public const string KEY_UNDERLINE = "u";
        public const string KEY_UNDERLINE_COLOR = "ucolor";
        public const string KEY_UNDERLINE_HEIGHT = "uheight";

        public string T => PropDict.ContainsKey(KEY_T) ? PropDict[KEY_T] : null;
        
        public int Id 
        {
            get { return PropDict.ContainsKey(KEY_ID) ? int.Parse(PropDict[KEY_ID]) : -1; }
        }

        public string Type
        {
            get { return PropDict.ContainsKey(KEY_TYPE) ? PropDict[KEY_TYPE] : null; }
        }

        public string Color
        {
            get { return PropDict.ContainsKey(KEY_COLOR) ? PropDict[KEY_COLOR] : null; }
        }

        public string Text
        {
            get { return PropDict.ContainsKey(KEY_TEXT) ? PropDict[KEY_TEXT] : null; }
        }

        public string Content
        {
            get { return PropDict.ContainsKey(KEY_CONTENT) ? PropDict[KEY_CONTENT] : null; }
        }

        public bool HasUnderline
        {
            get { return PropDict.ContainsKey(KEY_UNDERLINE) && PropDict[KEY_UNDERLINE] == "true"; }
        }

        public float UnderlineHeight
        {
            get { return PropDict.ContainsKey(KEY_UNDERLINE_HEIGHT) ? Convert.ToSingle(PropDict[KEY_UNDERLINE_HEIGHT]) : 1f; }
        }

        public string UnderlineColor
        {
            get { return PropDict.ContainsKey(KEY_UNDERLINE_COLOR) ? "#" + PropDict[KEY_UNDERLINE_COLOR] : null; }
        }

        public bool UseQuad
        {
            get { return T != TYPE_LINK; }
        }
        
        public int EndIndex
        {
            get { return StartIndex + HostLength; }
        }
        
        public bool Valid { get; set; }
        
        public readonly string PopulateText;//填充文本
        public Vector3 StartPosition;
        public readonly Vector2 Size;
        public readonly int FontSize;
        
        public int HostLength => T == TYPE_LINK ? EmojiText.GetMeshedTextLength(Text) : 1;
        
        public readonly Dictionary<string, string> PropDict;
        
        public List<Vector4> BoundList { get; private set; }

        public int StartIndex { get; set; }

        public string MatchText { get; set; }

        public TagData(string param, int fontSize)
        {
            MatchText = param;
            param = param.Replace("<", "");
            param = param.Replace(">", "");
            
            var splitArray = param.Split(',');

            PropDict = new Dictionary<string, string>();
            foreach (var t in splitArray)
            {
                var prop = t.Split('=');
                if (prop.Length >= 2 && !PropDict.ContainsKey(prop[0]))
                {
                    PropDict.Add(prop[0],prop[1]);
                }
            }
            
            switch (T)
            {
                case TYPE_ICON:
                    PopulateText = string.Format("<color=#00000000><quad Size={0} Width={1}></color>", fontSize, ICON_SCALE);
                    Size = new Vector2(ICON_SCALE, ICON_SCALE) * fontSize;
                    break;
                case TYPE_EMOJI:
                    PopulateText = string.Format("<color=#00000000><quad Size={0} Width={1}></color>", fontSize, EMOJI_SCALE);
                    Size = new Vector2(EMOJI_SCALE, EMOJI_SCALE) * fontSize;
                    break;
                case TYPE_BUTTON:
                    PopulateText = string.Format("<color=#00000000><quad Size={0}, Width={1}></color>", fontSize, BUTTON_SCALE);
                    Size = new Vector2(2, 1) * fontSize;
                    break;
                case TYPE_LINK:
                    PopulateText = string.Format("<color=#{0}>{1}</color>", Color, Text);
                    break;
            }
            FontSize = fontSize;
        }

        public void SetStartPosition(Vector3 position)
        {
            var offsetY = (Size.y - FontSize) / 2f + 2; //2为固定偏移值 可以根据项目情况微调
            position.Set(position.x, position.y - offsetY, position.z);
            StartPosition = position;
        }
        
        public void AddBound(Vector4 bound)
        {
            if (BoundList == null)
            {
                BoundList = new List<Vector4>();
            }
            BoundList.Add(bound);
        }

        public void ClearBound()
        {
            BoundList?.Clear();
        }
        
    }
}