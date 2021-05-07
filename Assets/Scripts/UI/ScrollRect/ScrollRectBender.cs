using GameApp.Util;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectBender : MonoBehaviour
{
    public float zBase = 1f;
    public AnimationCurve zCurve = new AnimationCurve();

    public float rotateBase = 100f;
    public AnimationCurve rotateCurve = new AnimationCurve();
    
    private ScrollRect _scroll;

    public void Refresh()
    {
        OnScrollValueChanged(Vector2.zero);
    }
    
    private void Start()
    {
        _scroll = GetComponent<ScrollRect>();
        _scroll.onValueChanged.AddListener(OnScrollValueChanged);
        Timer.Add(0, Refresh, 1);
    }

    private void OnScrollValueChanged(Vector2 val)
    {
        var content = _scroll.content;
        if (content == null) return;
        
        var isHori = _scroll.horizontal;
        
        var sRect = ((RectTransform)_scroll.transform).rect;
        var sLength = isHori ? sRect.width : sRect.height;

        var cAnchorPos = content.anchoredPosition;
        var cOffset = isHori ? cAnchorPos.x : -cAnchorPos.y;

        for (var i = 0; i < content.childCount; i++)
        {
            var item = content.GetChild(i);
            if (item.childCount > 0)
            {
                var iRectTrans = (RectTransform)item.transform;
                var iAnchorPos = iRectTrans.anchoredPosition;
                var iPosLen = isHori ? iAnchorPos.x : -iAnchorPos.y;

                var iRect = iRectTrans.rect;
                var iLength = isHori ? iRect.width : iRect.height;

                var iPivot = iRectTrans.pivot;
                var iPivotLen = isHori ? iPivot.x : iPivot.y;
                var iPivotOffset = iLength * (0.5f - iPivotLen);
                
                //anchor pos + content offset + item width offset
                var iPosX = iPosLen + cOffset + iPivotOffset;
                var bendContent = item.GetChild(0);
                DealBend((RectTransform)bendContent.transform, iPosX / sLength);
            }
        }
    }

    private void DealBend(RectTransform bend, float input)
    {
        //z offset
        var zOffset = zCurve.Evaluate(input) * zBase;
        var localPos = bend.localPosition;
        localPos.z = zOffset;
        bend.localPosition = localPos;
        
        //y rorate
        var rotate = rotateCurve.Evaluate(input) * rotateBase;
        bend.localRotation = _scroll.horizontal ? Quaternion.Euler(0f, rotate, 0f) : Quaternion.Euler(rotate, 0f, 0f);
    }


}
