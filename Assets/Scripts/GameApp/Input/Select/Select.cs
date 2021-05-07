
using Lean.Touch;
using UnityEngine;

namespace GameApp.Input
{
    public class Select : MonoBehaviour
    {
        public Camera customCamera;
        
        public Collider SelectedCollider { get; private set; }
        public ISelectable Selected { get; private set; }

        public void SelectCheck(LeanFinger finger)
        {
            Check(finger.ScreenPosition, true);
        }
        

        public void Check(LeanFinger finger)
        {
            Check(finger.ScreenPosition);
        }

        public void Check(Vector2 position, bool isSelect = false)
        {
            var cam = LeanTouch.GetCamera(customCamera);
            var ray = cam.ScreenPointToRay(position);
            #if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            #endif
            
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.collider != SelectedCollider)
                {
                    if (isSelect)
                    {
                        SelectedCollider = hit.collider;
                        Selected = SelectedCollider.gameObject.GetComponent<ISelectable>();
                        Selected?.Select();
                    }
                }
                else
                {
                    Selected?.Continue();
                }
            }
            else
            {
                Deselect();
            }
        }

        public void Deselect()
        {
            if (Selected != null)
            {
                Selected.Deselect();
                SelectedCollider = null;
                Selected = null;
            }
        }
        
        
    }
}