using System;

namespace GameApp.Assets
{
    public readonly struct AssetLocation : IEquatable<AssetLocation>
    {
        public readonly string name;
        public readonly Type type;

        public AssetLocation(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }
        
        public override int GetHashCode()
        {
            unchecked
            {
                return ((name != null ? name.GetHashCode() : 0) * 397) ^ (type != null ? type.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is AssetLocation other && Equals(other);
        }

        public bool Equals(AssetLocation other)
        {
            return name == other.name && type == other.type;
        }
    }
}