using System;
using System.Collections.Generic;
using UnityEngine;
using GameApp.ScenePlayable;
using GameApp.Util;

namespace GameApp.RenderTarget
{
    [Serializable]
    public class RenderElement
    {
        [PathRef(typeof(GameObject))]
        public string prefab;

        public Vector3 localPosition = Vector3.one;
        public bool resetRoation;
        public Vector3 rotation;
        
        public GameObject targetPrefab { get; private set; }
        public GameObject item { get; set; }

        public RenderElement(GameObject prefab)
        {
            targetPrefab = prefab;
        }

        public RenderElement(GameObject prefab, Vector3 localPosition)
        {
            targetPrefab = prefab;
            this.localPosition = localPosition;
        }

        public RenderElement(GameObject prefab, Vector3 localPosition, Vector3 rotation)
        {
            targetPrefab = prefab;
            this.localPosition = localPosition;
            resetRoation = true;
            this.rotation = rotation;
        }

        public void CreateElement(Transform parent)
        {
            if (targetPrefab == null && !string.IsNullOrEmpty(prefab))
                targetPrefab = PlayableLoader.LoadAssetAtPath<GameObject>(prefab);

            if (targetPrefab == null || item != null) return;

            item = UnityEngine.Object.Instantiate(targetPrefab, parent);
            item.hideFlags = HideFlags.DontSave;
            item.transform.localPosition = localPosition;
            if (resetRoation)
            {
                item.transform.rotation = Quaternion.Euler(rotation);
            }
            item.layer = parent.gameObject.layer;
        }

        public void DestroyElement()
        {
            if (item == null) return;
            UnityEngine.Object.DestroyImmediate(item);
        }
    }

    public class RenderTarget : MonoBehaviour
    {
        private const string TexturePrefix = "CamRenderTex ";

        public Vector2Int size = new Vector2Int(256, 256);

        public bool createElementOnAwake = true;

        public bool useRenderTexture = true;

        public Transform elementRoot;
        
        [Range(0, 10)]
        public float destroyDelay = 2f;

        public Camera renderCamera { get; private set; }

        public RenderTexture targetTexture { get; private set; }

        [SerializeField]
        private List<RenderElement> renderElements = new List<RenderElement>();

        public void InitRenderTexture()
        {
            GetRenderCamera();
            if (renderCamera == null) return;

            if (useRenderTexture)
            {
                targetTexture = RenderTexture.GetTemporary(size.x, size.y, 24, RenderTextureFormat.ARGB32);
                targetTexture.name = TexturePrefix + targetTexture.GetInstanceID();
                renderCamera.clearFlags = CameraClearFlags.SolidColor;
                renderCamera.backgroundColor = new Color(0, 0, 0, 0);
                renderCamera.targetTexture = targetTexture;
            }
            
            if (createElementOnAwake)
            {
                RefreshElements();
            }
            renderCamera.enabled = true;
        }

        public void AddElement(GameObject go)
        {
            elementRoot.AddChild(go);
        }

        public void AddElement(RenderElement element)
        {
            if (element != null)
            {
                renderElements.Add(element);
                element.CreateElement(transform);
            }
        }

        public void RemoveElement(RenderElement element)
        {
            if (renderElements.Remove(element))
            {
                element.DestroyElement();
            }
        }

        public void DestroyElements()
        {
            foreach (var element in renderElements)
            {
                element.DestroyElement();
            }
        }

        private void Awake()
        {
            if (elementRoot == null)
            {
                elementRoot = transform;
            }
            if (createElementOnAwake)
            {
                InitRenderTexture();
            }
        }

        private void GetRenderCamera()
        {
            renderCamera = gameObject.GetComponentInChildren<Camera>();
            if (renderCamera == null) return;

            renderCamera.enabled = false;
            var listener = renderCamera.gameObject.GetComponent<AudioListener>();
            if (listener) DestroyImmediate(listener);
        }

        private void RefreshElements()
        {
            if (renderElements.Count < 1) return;
            Timer.Add(0, CreateElement, renderElements.Count);
        }

        private void CreateElement(int index)
        {
            if (index < renderElements.Count)
            {
                renderElements[index]?.CreateElement(transform);
            }
        }

        private void OnDestroy()
        {
            if (renderCamera)
            {
                renderCamera.targetTexture = null;
            }
            RenderTexture.ReleaseTemporary(targetTexture);
        }

    }
}