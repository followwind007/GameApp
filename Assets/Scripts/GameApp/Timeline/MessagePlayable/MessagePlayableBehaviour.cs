using UnityEngine.Playables;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class MessagePlayableBehaviour : PlayableBehaviour
    {
        public MessagePlayableCommand Command { get; set; }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Command != null)
            {
                Command.DoCommand();
            }
        }

    }

}