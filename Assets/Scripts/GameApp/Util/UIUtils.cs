using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace GameApp.Util
{
    public static class UIUtils
    {
        public static readonly Vector2 ReferenceResolution = new Vector2(1280, 720);
        
        public static void SetCanvas(GameObject go, Camera camera, int order, int distance)
        {
            var canvas = go.GetOrAddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.sortingOrder = order;
            canvas.planeDistance = distance;

            var scaler = go.GetOrAddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            go.GetOrAddComponent<GraphicRaycaster>();
        }
        
        
        private static readonly Regex AnnoRegex = new Regex(@"<(.+?)>");
        private static readonly Regex AnnoRegexClose = new Regex(@"</(.+?)>");

        public static string ShrinkText(string text, int limit, string append = null)
        {
            if (string.IsNullOrEmpty(text)) return text;

            var mcs = AnnoRegex.Matches(text);
            int leftCount = 0 , rightCount = 0, totalCount = 0, lastEndMc = -1;

            var rawText = AnnoRegex.Replace(text, "");
            var breakIndex = rawText.Length > limit ? limit - 1 : text.Length - 1;

            for (var i = 0; i < mcs.Count; i++)
            {
                var mc = mcs[i];
                var count = mc.Index - lastEndMc - 1;
                var newCount = totalCount + count;
                
                //find the index reach the limit
                if (newCount > limit)
                {
                    breakIndex = lastEndMc + limit - totalCount;
                    break;
                }

                totalCount = newCount;
                lastEndMc = mc.Index + mc.Length - 1;
            
                //match the annotation in pair
                var isClose = mc.Value.StartsWith("</");
                if (isClose)
                {
                    rightCount++;
                    if (rightCount == leftCount)
                    {
                        leftCount = 0;
                        rightCount = 0;
                    }
                }
                else
                {
                    leftCount++;
                }
            }

            var sub = text.Substring(0, breakIndex + 1);
            var sb = new StringBuilder(sub);

            //append shrinked close annotation
            if (leftCount > rightCount)
            {
                var right = text.Substring(breakIndex + 1);
                mcs = AnnoRegexClose.Matches(right);
                if (mcs.Count < leftCount)
                {
                    Debug.LogError($"rich text annotation missmatch in [{text}]");
                }
                else
                {
                    for (var i = rightCount; i < leftCount; i++)
                    {
                        var mc = mcs[i];
                        sb.Append(mc.Value);
                    }
                }
            }

            if (sub.Length < text.Length && !string.IsNullOrEmpty(append)) sb.Append(append);
            
            return sb.ToString();
        }
    }
    
}