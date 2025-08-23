using System;
using Conversa.Runtime.Interfaces;

public class CustomMessageEvent : IConversationEvent
{
    public string Tag { get; }
    public EC.Localization.LocalizationElement<string> Actor { get; }
    public Action Advance { get; }

    public CustomMessageEvent(string tag, EC.Localization.LocalizationElement<string> actor, Action advance)
    {
        Tag = tag;
        Actor = actor;
        Advance = advance;
    }
}