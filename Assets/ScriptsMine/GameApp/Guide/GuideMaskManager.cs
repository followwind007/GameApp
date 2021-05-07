using System;
using UnityEngine;
using System.Collections.Generic;

namespace GameApp.Guide
{
    public class GuideMaskManager : MonoBehaviour
    {
        public class MaskReference
        {
            public readonly string Id;
            public string ExtraParams;
            public int Count;
            public MaskReference(string id, string extraParams)
            {
                Id = id;
                ExtraParams = extraParams;
            }
        }
        
        public static GuideMaskManager Instance;
        
        public Transform maskRoot;
        public Dictionary<string, MaskReference> targetItems { get; private set; }
        public HashSet<GuideMaskableItem> maskItems { get; private set; }

        public Action<RectTransform, bool> onSetFocused;

        public void OnMaskableItemEnable(GuideMaskableItem item)
        {
            maskItems.Add(item);
            UpdateMaskStatus();
        }

        public void OnMaskableItemDisable(GuideMaskableItem item)
        {
            maskItems.Remove(item);
            UpdateMaskStatus();
        }

        public bool IsMaskItemValid(GuideMaskableItem item)
        {
            if (!string.IsNullOrEmpty(item.id) && targetItems.ContainsKey(item.id))
            {
                var target = targetItems[item.id];
                if (target.Count < 1) return false;
                return (target.ExtraParams == null || target.ExtraParams.Equals(item.extraParams)) && item.CheckItemValid();
            }

            return false;
        }

        public void RetainMaskItem(string id, string extraParams = null)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (!targetItems.ContainsKey(id)) 
                targetItems[id] = new MaskReference(id, extraParams);
            
            var target = targetItems[id];
            if (target.ExtraParams != extraParams)
            {
                target.ExtraParams = extraParams;
                target.Count = 0;
            }
            targetItems[id].Count++;
            UpdateMaskStatus();
        }

        public void ReleaseMaskItem(string id, string extraParams = null)
        {
            if (string.IsNullOrEmpty(id) || !targetItems.ContainsKey(id)) return;
            var target = targetItems[id];
            if (target.ExtraParams == extraParams)
            {
                target.Count--;
                UpdateMaskStatus();
                if (targetItems[id].Count <= 0)
                {
                    targetItems.Remove(id);
                }
            }
        }

        public void UpdateMaskStatus()
        {
            var maskVisible = false;
            GameObject maskGo = null;

            foreach (var item in maskItems)
            {
                var valid = IsMaskItemValid(item);
                if (valid) maskGo = item.gameObject;
                item.SetFocused(valid);

                var setMaskRoot = valid && item.type != GuideMaskableItem.ValidType.NoneBlock;
                if (setMaskRoot) maskVisible = true;
                
                item.transform.SetParent(setMaskRoot ? maskRoot : item.Parent, true);
            }

            if (maskVisible && maskGo != null)
                onSetFocused?.Invoke(maskGo.GetComponent<RectTransform>(), true);
            else
                onSetFocused?.Invoke(null, false);

            maskRoot.gameObject.SetActive(maskVisible);
        }
        
        private void Awake()
        {
            targetItems = new Dictionary<string, MaskReference>();
            maskItems = new HashSet<GuideMaskableItem>();
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }
        
    }
}
