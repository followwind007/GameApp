using UnityEngine;
using System.Collections.Generic;

namespace Pangu.GUIs
{
    public static class TextureTools
    {
        public static Texture2D MergeMultiTexture(Texture2D[] texList)
        {
            if (texList.Length == 0) return null;
            //定义新图的宽高  
            int width = 0, height = 0;
            for (int i = 0; i < texList.Length; i++)
            {
                width += texList[i].width;
                if (i > 0)
                {
                    if (texList[i].height > texList[i - 1].height)
                        height = texList[i].height;
                }
                else height = texList[i].height;
            }

            Texture2D tex = new Texture2D(width, height);
            int x = 0, y = 0;
            for (int i = 0; i < texList.Length; i++)
            {
                Color32[] color = texList[i].GetPixels32(0);
                if (i > 0) tex.SetPixels32(x += texList[i - 1].width, y, texList[i].width, texList[i].height, color);
                else tex.SetPixels32(x, y, texList[i].width, texList[i].height, color);
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D OverlayWithTexture(Texture2D background, Texture2D foreground, Vector2 lowerleftAnchor)
        {
            if (background == null || foreground == null) return null;
            int bWidth = background.width;
            int bHeight = background.height;
            Texture2D tex = new Texture2D(bWidth, bHeight);
            tex.SetPixels(0, 0, bWidth, bHeight, background.GetPixels());

            int sx = (int)lowerleftAnchor.x;
            int sy = (int)lowerleftAnchor.y;
            int fWidth = foreground.width;
            int fHeight = foreground.height;
            int fx = 0;
            int fy = 0;

            if (sx <= -fWidth || sx >= bWidth || sy <= -fHeight || sy >= bHeight)
            {
                tex.Apply();
                return tex;
            }

            //处理边界, 修正采样坐标点
            if (sx < 0)
            {
                fx = -(int)lowerleftAnchor.x;
                sx = 0;
                fWidth = foreground.width + (int)lowerleftAnchor.x;
            }
            else if (sx >= bWidth - fWidth)
                fWidth = bWidth - (int)lowerleftAnchor.x;

            if (sy < 0)
            {
                fy = -(int)lowerleftAnchor.y;
                sy = 0;
                fHeight = foreground.height + (int)lowerleftAnchor.y;
            }
            else if (sy >= bHeight - fHeight)
                fHeight = bHeight - (int)lowerleftAnchor.y;

            Color[] fPixels = foreground.GetPixels(fx, fy, fWidth, fHeight);
            Color[] bPixels = tex.GetPixels(sx, sy, fWidth, fHeight);
            for (int i = 0; i < bPixels.Length; i++)
            {
                Color fc = fPixels[i];
                Color bc = bPixels[i];
                fPixels[i] = AlphaBlend(bc, fc);
            }
            
            tex.SetPixels(sx, sy, fWidth, fHeight, fPixels);
            tex.Apply();
            return tex;
        }

        public static Texture2D OverlayWithPolygon(Texture2D background, List<List<Vector2>> polygons, Color maskColor)
        {
            Texture2D tex = GetTextureForOperation(background);
            if (tex == null) return null;
            foreach (List<Vector2> polygon in polygons)
            {
                ApplyPolygonMask(ref tex, polygon.ToArray(), maskColor);
            }
            tex.Apply();
            return tex;
        }

        public static Texture2D OverlayWithPolygon(Texture2D background, Vector2[] polygon, Color maskColor)
        {
            Texture2D tex = GetTextureForOperation(background);
            if (tex == null) return null;
            ApplyPolygonMask(ref tex, polygon, maskColor);
            tex.Apply();
            return tex;
        }

        public static Color AlphaBlend(Color bc, Color fc)
        {
            float r = fc.a * fc.r + (1 - fc.a) * bc.r;
            float g = fc.a * fc.g + (1 - fc.a) * bc.g;
            float b = fc.a * fc.b + (1 - fc.a) * bc.b;
            return new Color(r, g, b);
        }

        private static Texture2D GetTextureForOperation(Texture2D background)
        {
            if (background == null) return null;
            int bWidth = background.width;
            int bHeight = background.height;
            Texture2D tex = new Texture2D(bWidth, bHeight);
            tex.SetPixels(0, 0, bWidth, bHeight, background.GetPixels());
            return tex;
        }

        private static void ApplyPolygonMask(ref Texture2D tex, Vector2[] polygon, Color maskColor)
        {
            Rect rect = GetPolygonTextureRect(polygon);

            int bWidth = tex.width, bHeight = tex.height;
            int startX = (int)(bWidth * rect.position.x);
            int startY = (int)(bHeight * rect.position.y);
            int width = (int)(bWidth * rect.width);
            int height = (int)(bHeight * rect.height);

            Vector2[] rawPolygon = GetRawPolygon(polygon, bWidth, bHeight);
            for (int i = startX; i < startX + width; i++)
            {
                for (int j = startY; j < startY + height; j++)
                {
                    if (IsPointInPolygon(new Vector2(i, j), rawPolygon))
                    {
                        if (maskColor.a >= 1f)
                        {
                            tex.SetPixel(i, j, maskColor);
                        }
                        else
                        {
                            Color bgColor = tex.GetPixel(i, j);
                            Color blend = AlphaBlend(bgColor, maskColor);
                            tex.SetPixel(i, j, blend);
                        }
                    }
                }
            }
        }

        private static Rect GetPolygonTextureRect(Vector2[] polygon)
        {
            Vector2 lowLeft = Vector2.one;
            Vector2 upperRight = Vector2.zero;
            foreach (Vector2 point in polygon)
            {
                lowLeft.x = Mathf.Min(point.x, lowLeft.x);
                lowLeft.y = Mathf.Min(point.y, lowLeft.y);
                upperRight.x = Mathf.Max(point.x, upperRight.x);
                upperRight.y = Mathf.Max(point.y, upperRight.y);
            }
            Vector2 size = upperRight - lowLeft;
            Rect rect = new Rect(lowLeft, size);
            return rect;
        }

        private static Vector2[] GetRawPolygon(Vector2[] polygon, int width, int height)
        {
            Vector2[] raw = new Vector2[polygon.Length];
            for (int i = 0; i < polygon.Length; i++)
            {
                raw[i] = new Vector2(polygon[i].x * width, polygon[i].y * height);
            }
            return raw;
        }

        private static bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
        {
            int polygonLength = polygon.Length, i = 0;
            bool inside = false;

            float pointX = point.x, pointY = point.y;
            Vector2 endPoint = polygon[polygonLength - 1];
            float startX, startY, endX = endPoint.x, endY = endPoint.y;

            while (i < polygonLength)
            {
                startX = endX; startY = endY; endPoint = polygon[i++];
                endX = endPoint.x; endY = endPoint.y;
                inside ^= (endY > pointY ^ startY > pointY) && ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }

    }
}
