using System.Runtime.CompilerServices;
using UnityEngine;

namespace Elua
{
    public struct ReferenceObject
    {
        private const int NullReference = -1;
        public int Count { get; private set; }
        public int Reference { get; }

        public bool IsAlive => Count > 0;
        
        public ReferenceObject(int reference = NullReference)
        {
            Reference = reference;
            Count = 0;
            Retain();
        }
        
        public void Retain()
        {
            Count++;
        }

        public void Release()
        {
            Count--;
            if (Count <= 0)
            {
                Dispose();
            }
        }
        
        public void Dispose()
        {
            if (Count > 0)
            {
                Debug.LogWarning($"dispose object with count: {Count}");
            }
            Count = 0;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return Reference == NullReference;
            }
            var ro = (ReferenceObject)obj;
            return Reference == ro.Reference;
        }

        public static bool operator ==(ReferenceObject a, ReferenceObject b)
        {
            return Compare(a, b);
        }
        
        public static bool operator !=(ReferenceObject a, ReferenceObject b)
        {
            return !Compare(a, b);
        }

        private static bool Compare(ReferenceObject a, ReferenceObject b)
        {
            var ra = a.Reference;
            var rb = b.Reference;

            return ra == rb;
        }
        
        
    }
}