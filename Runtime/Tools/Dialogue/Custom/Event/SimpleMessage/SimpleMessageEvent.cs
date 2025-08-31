using System;
using Conversa.Runtime.Interfaces;

public class SimpleMessageEvent : IConversationEvent
{
    public string[] Tags { get; }
    public EC.Localization.LocalizationElement<string> Actor { get; }
    public string Emotion { get; }
    public EC.Localization.LocalizationElement<string> Message { get; }
    public Action Advance { get; }

    public SimpleMessageEvent(string[] tags, EC.Localization.LocalizationElement<string> actor, string emotion, EC.Localization.LocalizationElement<string> message, Action advance)
    {
        Tags = tags;
        Actor = actor;
        Emotion = emotion;
        Message = message;
        Advance = advance;
    }
}