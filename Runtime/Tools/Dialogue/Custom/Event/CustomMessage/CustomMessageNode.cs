using System;
using System.Linq;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using UnityEngine;

[Serializable]
[Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
[Port("Message", "message", typeof(EC.Localization.LocalizationElement<string>), Flow.In, Capacity.One)]
[Port("Next", "next", typeof(BaseNode), Flow.Out, Capacity.One)]
public class CustomMessageNode : BaseNode, IEventNode
{
    public string[] tags = new string[] { };
    public EC.Localization.LocalizationElement<string> actor = new();
    public string emotion = string.Empty;

    public CustomMessageNode() { }

    public void Process(Conversation conversation, ConversationEvents conversationEvents)
    {
        void Advance()
        {
            var nextNode = conversation.GetOppositeNodes(GetNodePort("next")).FirstOrDefault();
            conversation.Process(nextNode, conversationEvents);
        }

        var e = new SimpleMessageEvent(tags, actor, emotion, ProcessPort(conversation, "message", new EC.Localization.LocalizationElement<string>()), Advance);
        conversationEvents.OnConversationEvent.Invoke(e);
    }
}
