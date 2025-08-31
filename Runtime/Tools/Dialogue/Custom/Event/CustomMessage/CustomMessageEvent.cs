using System;
using Conversa.Runtime.Interfaces;

public class CustomMessageEvent : IConversationEvent
{
    public string Tag { get; }
    public EC.Localization.LocalizationElement<string> Actor { get; }
    public string Emotion { get; }
    public Action Advance { get; }

    public CustomMessageEvent(string tag, EC.Localization.LocalizationElement<string> actor, string emotion, Action advance)
    {
        Tag = tag;
        Actor = actor;
        Emotion = emotion;
        Advance = advance;
    }
}