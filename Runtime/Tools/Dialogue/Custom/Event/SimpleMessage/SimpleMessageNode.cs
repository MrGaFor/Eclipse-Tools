using System;
using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using UnityEngine;

[Serializable]
[Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
[Port("Next", "next", typeof(BaseNode), Flow.Out, Capacity.One)]
public class SimpleMessageNode : BaseNode, IEventNode
{
    public string[] tags = new string[] { };
    public EC.Localization.LocalizationElement<string> actor = new();
    public string emotion;
    public EC.Localization.LocalizationElement<string> message = new();

    public SimpleMessageNode() { }

    public void Process(Conversation conversation, ConversationEvents conversationEvents)
    {
        void Advance()
        {
            var nextNode = conversation.GetOppositeNodes(GetNodePort("next")).FirstOrDefault();
            conversation.Process(nextNode, conversationEvents);
        }

        var e = new SimpleMessageEvent(tags, actor, emotion, message, Advance);
        conversationEvents.OnConversationEvent.Invoke(e);
    }
}
