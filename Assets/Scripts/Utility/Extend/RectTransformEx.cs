
using UnityEngine;

public static class RectTransformEx
{
    public static void SetParentFull(this RectTransform rectTrans, Transform parent)
    {
        rectTrans.SetParent(parent);
        rectTrans.localScale = Vector3.one;
        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMin = Vector2.zero;
        rectTrans.offsetMin = Vector2.zero;
        rectTrans.offsetMax = Vector2.zero;
        rectTrans.anchorMax = Vector2.one;
        var lp = rectTrans.localPosition;
        rectTrans.localPosition = new Vector3(lp.x, lp.y, 0);
    }

    public static void SetParentCenter(this RectTransform rectTrans, Transform parent)
    {
        rectTrans.SetParent(parent, false);
        rectTrans.localScale = Vector3.one;
        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
        rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
        rectTrans.anchoredPosition = Vector2.zero;
        var lp = rectTrans.localPosition;
        rectTrans.localPosition = new Vector3(lp.x, lp.y, 0);
        rectTrans.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    public static Rect GetRect(this RectTransform rectTrans, RectTransform canvasRectTrans)
    {
        var rs = new Vector3[4];
        canvasRectTrans.GetWorldCorners(rs);
        
        var cs = new Vector3[4];
        rectTrans.GetWorldCorners(cs);

        var scale = canvasRectTrans.localScale.x;
        var size = (cs[2] - cs[0]) / scale;

        var start = (cs[0] - rs[0]) / scale;
        
        return new Rect(start, size);
    }
    
}