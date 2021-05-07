
using UnityEngine;
using System.Collections.Generic;
using GameApp.AnimatorBehaviour;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class TestAnimator : MonoBehaviour
{
    public Animator animator;

    public List<AnimationClip> clips = new List<AnimationClip>();

    public AnimatorBehaviour behaviour;

    private PlayableGraph _graph;
    private AnimationMixerPlayable _mixer;

    private float _proc;

    private void Awake()
    {
        _graph = PlayableGraph.Create("Test");
        _mixer = AnimationMixerPlayable.Create(_graph, clips.Count);
        var output = AnimationPlayableOutput.Create(_graph, "mixer output", animator);
        output.SetSourcePlayable(_mixer);
    }

    private void OnGUI()
    {
        DrawAnimator();
    }

    public void DrawPlayable()
    {
        for (var i = 0; i < clips.Count; i++)
        {
            if (GUILayout.Button($"clip {i}", GUILayout.Width(100), GUILayout.Height(50)))
            {
                var p = AnimationClipPlayable.Create(_graph, clips[i]);
                if (!_mixer.GetInput(i).IsNull()) _graph.Disconnect(_mixer, i);
                p.SetTime(_proc);
                _graph.Connect(p, 0, _mixer, i);
                _mixer.SetInputWeight(i, 1);
            }
        }

        if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _graph.Play();
        }
        if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(50)))
        {
            _graph.Stop();
        }
        
        _proc = GUILayout.HorizontalSlider(_proc, 0, 10, GUILayout.Width(100));
    }

    public void DrawAnimator()
    {
        if (GUILayout.Button("play Default", GUILayout.Width(100), GUILayout.Height(50)))
        {
            behaviour.Runner.Play("Default");
        }
        if (GUILayout.Button("play s1", GUILayout.Width(100), GUILayout.Height(50)))
        {
            behaviour.Runner.Play("s1");
        }
        if (GUILayout.Button("play s2", GUILayout.Width(100), GUILayout.Height(50)))
        {
            behaviour.Runner.Play("s2");
        }
        if (GUILayout.Button("set trigger t", GUILayout.Width(100), GUILayout.Height(50)))
        {
            behaviour.Runner.SetTrigger("t");
        }
        if (GUILayout.Button("play additive a1", GUILayout.Width(100), GUILayout.Height(50)))
        {
            behaviour.Runner.PlayAdditive("a1");
        }
        if (GUILayout.Button("stop additive a1", GUILayout.Width(100), GUILayout.Height(50)))
        {
            behaviour.Runner.StopAddtive("a1");
        }
    }

    private void OnDestroy()
    {
        _graph.Destroy();
    }
    
}
