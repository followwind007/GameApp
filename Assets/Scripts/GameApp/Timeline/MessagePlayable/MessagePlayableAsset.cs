using UnityEngine;
using UnityEngine.Playables;
using GameApp.DataBinder;

namespace GameApp.Timeline
{
    [System.Serializable]
    public class MessagePlayableAsset : PlayableAsset
    {
        public string messageName;

        public BindableExposes exposes;
        public BindableValues values;

        public MessagePlayableBehaviour template = new MessagePlayableBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var command = new MessagePlayableCommand()
            {
                name = messageName,
            };

            foreach (var expose in exposes.List)
                command.body[expose.key] = expose.value.Resolve(graph.GetResolver());
            foreach (var val in values.wraps)
                command.body[val.name] = val.value;
            
            template.Command = command;

            return ScriptPlayable<MessagePlayableBehaviour>.Create(graph, template);
        }

    }

}