using System;
using DG.Tweening;
using GameApp.Util;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectExpander : MonoBehaviour
{
    public RectTransform content;
    public GameObject child;
    
    public Action<int, GameObject> onRefresh;
    public Func<int, GameObject> onGet;
    public Action<GameObject> onRecycle;

    public Vector2 childSize;
    public Vector2 defaultSize;

    public LayoutElement layoutElement;

    public int childCount;

    public bool useTween = true;
    public float tweenTime = 0.2f;
    public Ease ease = Ease.Linear;
    
    public bool Expanded { get; private set; }

    private LayoutGroup _contentLayout;

    public void Toggle()
    {
        if (Expanded)
        {
            Hide();
            return;
        }
        Expand(childCount);
    }

    public void Expand(int count)
    {
        Expanded = true;
        childCount = count;
        
        DoExpand();
        ResetSize(count);
    }

    public void Hide()
    {
        Expanded = false;
        if (useTween)
        {
            Timer.Add(tweenTime / childCount, DoHide, childCount);
        }
        else
        {
            content.gameObject.SetActive(false);
        }
        ResetSize(0);
    }
    
    private void DoExpand()
    {
        content.gameObject.SetActive(true);
        
        var curCount = content.childCount;
        for (var i = 0; i < curCount; i++) content.GetChild(i).gameObject.SetActive(false);

        var gap = childCount - curCount;
        if (gap > 0)
        {
            for (var i = 0; i < gap; i++)
            {
                var go = onGet != null ? onGet(curCount + i) : Instantiate(child);
                content.AddChild(go);
                go.SetActive(false);
            }
        }
        else if (gap < 0)
        {
            for (var i = gap; i < 0; i++)
            {
                var go = content.GetChild(content.childCount + i).gameObject;
                if (onRecycle != null)
                    onRecycle(go);
                else
                    go.SetActive(false);
            }
        }

        if (useTween)
        {
            Timer.Add(tweenTime / childCount, DoShow, childCount);
        }
        else
        {
            for (var i = 0; i < childCount; i++) DoShow(i);
        }
    }

    private void DoShow(int index)
    {
        var go = content.GetChild(index).gameObject;
        go.SetActive(true);
        onRefresh?.Invoke(index, go);
    }

    private void DoHide(int index)
    {
        var go = content.GetChild(index).gameObject;
        go.SetActive(false);
    }

    private void ResetSize(int count, Action callback = null)
    {
        if (_contentLayout == null)
        {
            _contentLayout = content.GetComponent<LayoutGroup>();
            if (_contentLayout == null)
            {
                Debug.LogError("null layout group on content!");
                return;
            }
        }

        var size = _contentLayout is VerticalLayoutGroup ? 
            new Vector2(defaultSize.x, defaultSize.y + childSize.y * count) : 
            new Vector2(defaultSize.x + childSize.x * count, defaultSize.y);
        if (useTween)
        {
            var tn = layoutElement.DOPreferredSize(size, tweenTime, true);
            tn.SetEase(ease);
            tn.onComplete = () => { callback?.Invoke(); };
        }
        else
        {
            layoutElement.preferredWidth = size.x;
            layoutElement.preferredHeight = size.y;
            callback?.Invoke();
        }
    }

    private void OnDestroy()
    {
        onRefresh = null;
        onGet = null;
        onRecycle = null;
    }
    
}
