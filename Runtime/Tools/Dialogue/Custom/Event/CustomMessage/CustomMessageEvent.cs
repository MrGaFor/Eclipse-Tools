using System;
using Conversa.Runtime.Interfaces;

public class CustomMessageEvent : IConversationEvent
{
    public string[] Tags { get; }
    public EC.Localization.LocalizationElement<string> Actor { get; }
    public string Emotion { get; }
    public Action Advance { get; }

    public CustomMessageEvent(string[] tags, EC.Localization.LocalizationElement<string> actor, string emotion, Action advance)
    {
        Tags = tags;
        Actor = actor;
        Emotion = emotion;
        Advance = advance;
    }
}