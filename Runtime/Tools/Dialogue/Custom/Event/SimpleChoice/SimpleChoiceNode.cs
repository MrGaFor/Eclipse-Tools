using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
[Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
public class SimpleChoiceNode : BaseNode, IEventNode
{
    public string[] tags = new string[] { };
    public EC.Localization.LocalizationElement<string> actor = new();
    public string emotion;
    public EC.Localization.LocalizationElement<string> message = new();
    public List<SimplePortDefinition<BaseNode>> options = new List<SimplePortDefinition<BaseNode>>
        {
            new SimplePortDefinition<BaseNode>("yes", new EC.Localization.LocalizationElement<string>()),
            new SimplePortDefinition<BaseNode>("no", new EC.Localization.LocalizationElement<string>())
        };


    public SimpleChoiceNode() { }

    public void Process(Conversation conversation, ConversationEvents conversationEvents)
    {
        void HandleClickOption(SimplePortDefinition<BaseNode> portDefinition)
        {
            var nextNode = conversation.GetOppositeNodes(GetNodePort(portDefinition.Guid)).FirstOrDefault();
            conversation.Process(nextNode, conversationEvents);
        }
        ;

        SimpleOption NodePortToOption(SimplePortDefinition<BaseNode> portDefinition) => new SimpleOption(portDefinition.Value, () => HandleClickOption(portDefinition));

        var choiceEvent = new SimpleChoiceEvent(tags, actor, emotion, message, options.Select(NodePortToOption).ToList());
        conversationEvents.OnConversationEvent.Invoke(choiceEvent);
    }

    public override bool ContainsPort(string portId, Flow flow)
    {
        if (base.ContainsPort(portId, flow)) return true;
        return flow == Flow.Out && options.Any(option => option.Guid == portId);
    }
}
