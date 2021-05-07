using UnityEngine;

namespace Pangu.GUIs
{
    [RequireComponent(typeof(RectTransform))]
    public class MiniMapElement : MonoBehaviour
    {
        public Transform targetTrans;
        public MiniMap miniMap;

        private bool _preVisible = false;
        private bool _isVisible = false;
        private float _countVisible = 0f;
        private readonly float _intervalVisible = 1f;

        private void Start()
        {
            
        }

        private void Update()
        {
            _countVisible += Time.deltaTime;

            if (_countVisible < _intervalVisible)
            {
                return;
            }
            _countVisible = 0f;
            if (targetTrans == null)
            {
                //Remove
            }
            _preVisible = _isVisible;

            _isVisible = miniMap.CheckElementVisible(targetTrans.position);
            if (_preVisible != _isVisible)
            {
                _preVisible = _isVisible;
                if (_isVisible)
                {
                    //Add
                }
                else
                {
                    //Remove
                }
            }
        }



    }

}