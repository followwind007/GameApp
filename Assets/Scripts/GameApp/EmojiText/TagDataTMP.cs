using System.Collections.Generic;

namespace GameApp.EmojiText
{
    public class TagDataTMP
    {
        public const string TypeIcon = "icon";
        public const string TypeLink = "link";

        public const string KeyType = "type";
        public const string KeyT = "t";
        public const string KeyID = "id";
        public const string KeyText = "text";
        public const string KeyColor = "color";

        public string T => propDict.ContainsKey(KeyT) ? propDict[KeyT] : null;
        
        public int Id => propDict.ContainsKey(KeyID) ? int.Parse(propDict[KeyID]) : -1;

        public string Type => propDict.ContainsKey(KeyType) ? propDict[KeyType] : null;

        public string Color => propDict.ContainsKey(KeyColor) ? propDict[KeyColor] : null;

        public string Text => propDict.ContainsKey(KeyText) ? propDict[KeyText] : null;
        

        public readonly Dictionary<string, string> propDict;

        public readonly string matchText;
        public readonly string populateText;
        

        public TagDataTMP(string param)
        {
            matchText = param;
            param = param.Substring(1, param.Length - 2);
            
            var splitArray = param.Split(',');

            propDict = new Dictionary<string, string>();
            foreach (var t in splitArray)
            {
                var prop = t.Split('=');
                if (prop.Length >= 2 && !propDict.ContainsKey(prop[0]))
                {
                    propDict.Add(prop[0], prop[1]);
                }
            }

            switch (T)
            {
                case TypeLink:
                    var link = $"<link=\"{Type}_{Id}\">{Text}</link>";
                    if (!string.IsNullOrEmpty(Color)) 
                        link = $"<#{Color}>{link}</color>";
                    populateText = link;
                    break;
                case TypeIcon:
                    populateText = $"<sprite=\"{Type}\" index={Id}>";
                    break;
                default:
                    populateText = matchText;
                    break;
            }
        }
        
    }
}