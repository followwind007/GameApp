using UnityEngine;

namespace GameApp.AnimationRigging
{
    public static class AxisUtil
    {
        public enum Axis
        {
            Forward,
            Back,
            Up,
            Down,
            Left,
            Right
        }
        
        public static Vector3 GetAxisVector(Axis axis)
        {
            switch (axis)
            {
                case Axis.Forward:
                    return Vector3.forward;
                case Axis.Back:
                    return Vector3.back;
                case Axis.Up:
                    return Vector3.up;
                case Axis.Down:
                    return Vector3.down;
                case Axis.Left:
                    return Vector3.left;
                case Axis.Right:
                    return Vector3.right;
            }

            return Vector3.forward;
        }

        public static Vector3 GetClampedRotation(Vector3 rot)
        {
            var r = rot;
            if (r.x < -180) r.x = r.x + 360;
            if (r.x > 180) r.x = r.x - 360;
            
            if (r.y < -180) r.y = r.y + 360;
            if (r.y > 180) r.y = r.y - 360;
            
            if (r.z < -180) r.z = r.z + 360;
            if (r.z > 180) r.z = r.z - 360;

            return r;
        }
        
    }
}