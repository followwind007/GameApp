using System;

namespace GameApp.LuaResolver
{
    public class LuaType
    {
        public enum ValueType {  }
        public readonly string name;

        public Type CsType => Type.GetType(name);

        public bool IsCsType => CsType != null;

        public LuaType(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return $"{name}";
        }

        public override bool Equals(object obj)
        {
            if (obj is LuaType lt)
            {
                return Equals(lt);
            }

            return false;
        }

        protected bool Equals(LuaType other)
        {
            return name == other.name;
        }

        public override int GetHashCode()
        {
            return name != null ? name.GetHashCode() : 0;
        }

        
        
    }
}