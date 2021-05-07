using System;

namespace UnityEngine
{
    public static class GameObjectEx
    {
        public static Component GetComponent(this GameObject item, string path, string componentName)
        {
            var targetTrans = item.transform.Find(path);
            return targetTrans != null ? targetTrans.gameObject.GetComponent(componentName) : null;
        }

        public static Component GetComponent(this GameObject item, string path, Type componentType)
        {
            var targetTrans = item.transform.Find(path);
            return targetTrans != null ? targetTrans.gameObject.GetComponent(componentType) : null;
        }

        public static T GetComponent<T>(this GameObject item, string path) where T:Component
        {
            var targetTrans = item.transform.Find(path);
            return targetTrans != null ? targetTrans.gameObject.GetComponent<T>() : null;
        }

        #if TEMPLATE_MODE
        public static void SetVisible(this GameObject item, bool isVisible)
        {
            item.transform.localScale = isVisible ? Vector3.one : Vector3.zero;

            var visibleObjects = item.GetComponents<IVisibleObject>();
            if (visibleObjects != null)
            {
                foreach (var vo in visibleObjects)
                {
                    vo.OnVisible(isVisible);
                }
            }
        }
        #endif

        public static T GetOrAddComponent<T>(this GameObject item) where T : Component
        {
            return GetOrAddComponent(item, typeof(T)) as T;
        }

        public static Component GetOrAddComponent(this GameObject item, Type componentType)
        {
            var comp = item.GetComponent(componentType);
            if (comp == null)
            {
                comp = item.AddComponent(componentType);
            }

            return comp;
        }

    }
}
