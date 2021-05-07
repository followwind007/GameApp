using UnityEngine;
using System.Collections;

public class OverrideControllerUnit
{
    public AnimatorOverrideController controller;

    public OverrideControllerUnit()
    {
        controller = new AnimatorOverrideController();
        AnimationClip clip = new AnimationClip();
        controller[""] = clip;
    }

    public AnimationClip GetClip(string name)
    {
        Debug.Log(string.Format("[{0}] get clip: {1}", Time.time, name));
        return null;
    }

}
