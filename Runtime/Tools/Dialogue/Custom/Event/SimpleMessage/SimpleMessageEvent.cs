using System;
using Conversa.Runtime.Interfaces;

public class SimpleMessageEvent : IConversationEvent
{
    public string Tag { get; }
    public EC.Localization.LocalizationElement<string> Actor { get; }
    public EC.Localization.LocalizationElement<string> Message { get; }
    public Action Advance { get; }

    public SimpleMessageEvent(string tag, EC.Localization.LocalizationElement<string> actor, EC.Localization.LocalizationElement<string> message, Action advance)
    {
        Tag = tag;
        Actor = actor;
        Message = message;
        Advance = advance;
    }
}