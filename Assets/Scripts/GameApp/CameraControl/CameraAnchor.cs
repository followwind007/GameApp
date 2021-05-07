using Cinemachine;
using UnityEngine;

namespace GameApp.CameraControl
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraAnchor : MonoBehaviour
    {
        public enum AnchorMode
        {
            ScreenPosition,
        }

        public AnchorMode mode = AnchorMode.ScreenPosition;

        public Vector2 screenPosition;
        
        private CinemachineVirtualCamera _virtualCamera;
        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Update()
        {
            if (!CinemachineCore.Instance.IsLive(_virtualCamera)) return;
            if (mode == AnchorMode.ScreenPosition)
            {
                
            }
        }
    }
}