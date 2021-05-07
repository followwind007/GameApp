using System;
using UnityEngine;

[Serializable]
public class PathRef : PropertyAttribute
{
    public Type PropType { get; }
    public bool ShowTitle { get; }

    public PathRef(Type propType, bool showTitle = true)
    {
        PropType = propType;
        ShowTitle = showTitle;
    }
}