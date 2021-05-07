using UnityEngine;
using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class ControlPlayableBehaviour : PlayableBehaviour, IPlayableTarget
    {
        public GameObject Target { get; set; }

        public ControlPlayableCommand command;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Target == null ||  command == null) return;
            command.DoCommand(Target);
        }

    }

}