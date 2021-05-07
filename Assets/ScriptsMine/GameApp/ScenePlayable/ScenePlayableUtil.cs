using System;
using UnityEngine;

namespace GameApp.ScenePlayable
{
    public static class ScenePlayableUtil
    {
        public const string EVENT_OP_PLAY = "play";
        public const string EVENT_OP_PAUSE = "pause";
        public const string EVENT_OP_STOP = "stop";

        public const int RECEIVER_LAYER = 18;

        public const float DEFALUT_OVERLAP_RADIUS = 5f;

        public const float DEFAULT_TRIGGER_RADIUS = 0.5f;

        public static int ReceiverLayer { get { return 1 << RECEIVER_LAYER; } }
        public static int ReceiverLayerMask { get { return ~ReceiverLayer; } }

        public static int GetCurvedInt(float factor, AnimationCurve curve, float proc)
        {
            return (int)GetCurvedFloat(factor, curve, proc);
        }

        public static float GetCurvedFloat(float factor, AnimationCurve curve, float proc)
        {
            return factor * curve.Evaluate(proc);
        }

        public static Color GetCurvedColor(AnimationCurve[] curves, float proc)
        {
            Color color = new Color();
            if (curves != null && curves.Length >= 4)
            {
                color.r = curves[0].Evaluate(proc);
                color.g = curves[1].Evaluate(proc);
                color.b = curves[2].Evaluate(proc);
                color.a = curves[3].Evaluate(proc);
            }
            return color;
        }

        

    }

}