namespace GameApp.LuaResolver
{
    public enum LineType
    {
        Property, PublicProperty, Class, Content, DefineClass, ClassType, Function, Annotation, Tag
    }
    
    public class LineInfo
    {
        public string str;
        public LineType type;
        public int line;
    }
}