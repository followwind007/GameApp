using System;

namespace GameApp.Serialize
{
    public class NotExportEmptyAttribute : Attribute
    {
        
    }

    public class NotExportDefaultAttribute : Attribute
    {
        
    }

    public enum EnumSaveType
    {
        Int,
        String
    }
    
    public class EnumValueAttribute : Attribute
    {
        public readonly EnumSaveType saveType;
        
        public EnumValueAttribute(EnumSaveType saveType)
        {
            this.saveType = saveType;
        }
    }
}