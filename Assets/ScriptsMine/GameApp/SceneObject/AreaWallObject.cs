using UnityEngine;
using System.Collections.Generic;

namespace Pangu.SceneObject
{
    [System.Serializable]
    public class ItemBelongArea : ILockableObject
    {
        public int areaId;

        public bool AreaLocked
        {
            get
            {
                return SceneAreaManager.Instance.GetAreaLockState(areaId);
            }
        }
    }

    public class AreaWallObject : MonoBehaviour, IDissolvableObject
    {
        public List<ItemBelongArea> belongAreaList = new List<ItemBelongArea>();

        public bool CanDissolve
        {
            get
            {
                foreach (var area in belongAreaList)
                {
                    if (area.AreaLocked)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

    }

}