using System;
using UnityEngine;

namespace Tools.Table
{
    /// <summary>
    /// tabed items
    /// </summary>
    public class TabItemAttribute : Attribute
    {
        public readonly string tab;

        /// <summary>
        /// serialized property with same tab name will be grouped in same tab
        /// </summary>
        /// <param name="tab">tab name</param>
        public TabItemAttribute(string tab)
        {
            this.tab = tab;
        }
    }

    /// <summary>
    /// line items
    /// </summary>
    public class LineItemAttribute : Attribute
    {
        public readonly string line;
        
        /// <summary>
        /// serialized property with same line name will be grouped in same line
        /// </summary>
        /// <param name="line">line name</param>
        public LineItemAttribute(string line)
        {
            this.line = line;
        }
    }

    /// <summary>
    /// use ReorderableList to show this list
    /// </summary>
    public class ListItemAttribute : Attribute
    {
        public bool useSearch;
    }

    public class ReorderableItemAttribute : Attribute
    {
        public bool dragable = true;
        public bool displayHeader = true;
        public bool displayAddButton = true;
        public bool displayRemoveButton = true;
    }

    /// <summary>
    /// use ToString() value as display name
    /// </summary>
    public class CustomDescAttribute : Attribute
    {
        public bool showIndex = false;
        public int indexBase = 0;
    }

    /// <summary>
    /// expose this function in the editor which can be called at runtime
    /// </summary>
    public class ExposedFuncAttribute : Attribute
    {
        public string displayName;
        /// <summary>
        /// number with the same /100 value, will be in same line 
        /// </summary>
        public int priority;
    }

    /// <summary>
    /// use custom name for property
    /// </summary>
    public class CustomNameAttribute : Attribute
    {
        public readonly string name;

        public CustomNameAttribute(string name)
        {
            this.name = name;
        }
    }
    
    public class MinMaxAttribute : PropertyAttribute
    {
        public readonly float minLimit;
        public readonly float maxLimit;
        public readonly bool showEditRange;
        public readonly bool showDebugValues;

        public MinMaxAttribute(float min, float max)
        {
            minLimit = min;
            maxLimit = max;
        }
        
        public MinMaxAttribute(float min, float max, bool showDebugValues) : this(min, max)
        {
            this.showDebugValues = showDebugValues;
        }
        
        public MinMaxAttribute(float min, float max, bool showDebugValues, bool showEditRange) : this(min, max, showDebugValues)
        {
            this.showEditRange = showEditRange;
        }
        
    }
    
    
}