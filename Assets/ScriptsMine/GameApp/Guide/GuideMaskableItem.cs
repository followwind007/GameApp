using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameApp.Guide
{
    [RequireComponent(typeof(Graphic))]
    public class GuideMaskableItem : MonoBehaviour
    {
        public enum ValidType
        {
            NoneBlock,
            Block,
        }

        public string id;
        public ValidType type;
        
        public string extraParams;
        
        public GameObject hint;
        
        [NonSerialized] public Transform Parent;

        public Func<bool> onCheckItemValid;

        public bool CheckItemValid()
        {
            return onCheckItemValid == null || onCheckItemValid();
        }

        public void SetFocused(bool val)
        {
            if (hint == null) return;
            hint.SetActive(val);
        }

        private void Awake()
        {
            Parent = transform.parent;
            if (GuideMaskManager.Instance == null) return;
            GuideMaskManager.Instance.OnMaskableItemEnable(this);
        }

        private void OnDestroy()
        {
            if (GuideMaskManager.Instance == null) return;
            GuideMaskManager.Instance.OnMaskableItemDisable(this);
        }

    }
}