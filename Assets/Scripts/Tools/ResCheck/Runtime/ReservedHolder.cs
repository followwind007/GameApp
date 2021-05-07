#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Tools.ResCheck
{
    public class ReservedHolder : MonoBehaviour
    {
        
        public List<Component> reserves;

        public bool IsReserved(Component component)
        {
            return reserves.Contains(component);
        }
    }
}
#endif