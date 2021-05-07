using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHelper : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool checkLongPress;
        [Range(0.1f, 5.0f)]
        public float longPressDuration = 0.5f;

        private enum LongPressStage
        {
            Default, Checking, Checked, 
        }

        private LongPressStage _longPressStage = LongPressStage.Default;
        private float _longPressStart = float.MaxValue;

        public System.Action onLongPress;
        public System.Action onLongPressExit;
        
        public PointerEventData PointerDownData { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDownData = eventData;
            if (checkLongPress)
            {
                _longPressStart = Time.time;
                _longPressStage = LongPressStage.Checking;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CheckExitLongPress();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CheckExitLongPress();
        }

        private void CheckExitLongPress()
        {
            if (!checkLongPress) return;
            if (_longPressStage == LongPressStage.Checked)
            {
                if (onLongPressExit != null)
                {
                    onLongPressExit();
                }
            }
            _longPressStage = LongPressStage.Default;
        }

        private void Update()
        {
            if (checkLongPress && _longPressStage == LongPressStage.Checking)
            {
                if (Time.time - _longPressStart > longPressDuration)
                {
                    _longPressStage = LongPressStage.Checked;
                    if (onLongPress != null)
                    {
                        onLongPress();
                    }
                }
            }
        }

        
    }
}
