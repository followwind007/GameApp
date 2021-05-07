using UnityEngine;
using UnityEngine.UI;
using GameApp.ScenePlayable;

namespace GameApp.RenderTarget
{
    [RequireComponent(typeof(RawImage))]
    public class RenderTargetHelper : MonoBehaviour
    {
        public bool displayOnEnable = true;

        public bool isUnique;

        [PathRef(typeof(GameObject))] [HideInInspector]
        public string target;

        public RenderTarget renderTarget { get; private set; }

        public GameObject targetPrefab;

        public bool releaseOnDisable;
        
        private RawImage _rawImg;

        public void ShowTexture()
        {
            if (renderTarget == null)
            {
                renderTarget = RenderTargetManager.instance.GetRenderTextureTarget(targetPrefab, isUnique);
            }
            else
            {
                RenderTargetManager.instance.RetainItem(targetPrefab, renderTarget.gameObject);
            }
            
            if (renderTarget)
            {
                _rawImg.texture = renderTarget.targetTexture;
            }
            else
            {
                Debug.LogWarning("null RenderTarget");
            }
        }

        public void ResetTexture(GameObject prefab)
        {
            if (renderTarget && targetPrefab)
            {
                RenderTargetManager.instance.ReleaseItem(targetPrefab, renderTarget.gameObject);
            }
            targetPrefab = prefab;
            renderTarget = null;
            _rawImg.texture = null;
        }

        private void Awake()
        {
            _rawImg = GetComponent<RawImage>();
            if (!string.IsNullOrEmpty(target) && renderTarget == null)
            {
                targetPrefab = PlayableLoader.LoadAssetAtPath<GameObject>(target);
            }
        }

        private void OnEnable()
        {
            if (displayOnEnable)
            {
                ShowTexture();
            }
        }
        
        private void OnDisable()
        {
            if (releaseOnDisable)
            {
                ResetTexture(targetPrefab);                
            }
        }

        private void OnDestroy()
        {
            ResetTexture(null);
        }
        
        
    }
}
