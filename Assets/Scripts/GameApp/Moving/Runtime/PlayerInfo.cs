using UnityEngine;

namespace GameApp.Moving
{
    [System.Serializable]
    public class PlayerInfo
    {
        public Transform transform;
        public Transform neck;
        public Transform rightHand;
        public Transform leftHand;
        public Transform rightFoot;
        public Transform leftFoot;

        public float walkSpeed = 15f;
        public float runSpeed = 25f;
        public float rotateSpeed = 4f;
    }
}