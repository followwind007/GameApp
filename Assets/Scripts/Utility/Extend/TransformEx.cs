using UnityEngine;

public static class TransformEx
{
    public static void SetParentScaled(this Transform trans, Transform parent)
    {
        if (trans == null || parent == null)
        {
            return;
        }
        trans.SetParent(parent);
        trans.localScale = Vector3.one;
    }
    
    public static void AddChild(this Transform trans, GameObject childGameObject, bool resetLayer = false)
    {
        if (childGameObject != null)
        {
            AddChild(trans, childGameObject.transform, resetLayer);
        }
    }

    public static void AddChild(this Transform trans, Transform childTrans, bool resetLayer = false)
    {
        if (trans == null || childTrans == null)
        {
            return;
        }
        childTrans.SetParent(trans);
        childTrans.localPosition = Vector3.zero;
        childTrans.localRotation = Quaternion.identity;
        childTrans.localScale = Vector3.one;
        if (resetLayer)
        {
            ResetLayer(trans, childTrans);            
        }
    }

    public static void ResetLayer(this Transform trans, Transform child)
    {
        child.gameObject.layer = trans.gameObject.layer;
        for (var i = 0; i < child.childCount; i++)
        {
            ResetLayer(child, child.GetChild(i));
        }
    }

    public static void SetVisible(this Transform trans, bool visible)
    {
        if (trans == null)
        {
            return;
        }

        trans.localScale = visible ? Vector3.one : Vector3.zero;
    }
    
    public static string GetPath(this Transform ct, Transform rt)
    {
        var sb = "";
        while (ct && ct != rt)
        {
            sb = $"{ct.gameObject.name}/{sb}";
            ct = ct.parent;
        }

        sb = sb.Remove(sb.Length - 1);
        return sb;
    }
    
}
