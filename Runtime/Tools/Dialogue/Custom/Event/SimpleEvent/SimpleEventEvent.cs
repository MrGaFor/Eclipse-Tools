using System;
using Conversa.Runtime.Interfaces;

public class SimpleEventEvent : IConversationEvent
{
    public string Tag { get; }
    public Action Advance { get; }

    public SimpleEventEvent(string tag, Action advance)
    {
        Tag = tag;
        Advance = advance;
    }
}