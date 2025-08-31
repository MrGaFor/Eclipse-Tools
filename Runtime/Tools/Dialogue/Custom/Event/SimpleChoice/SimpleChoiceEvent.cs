using Conversa.Runtime.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Conversa.Runtime.Events
{
    public class SimpleChoiceEvent : IConversationEvent
    {
        public string Tag { get; }
        public EC.Localization.LocalizationElement<string> Actor { get; }
        public string Emotion { get; }
        public EC.Localization.LocalizationElement<string> Message { get; }
        public List<SimpleOption> Options { get; }

        public SimpleChoiceEvent(string tag, EC.Localization.LocalizationElement<string> actor, string emotion, 
            EC.Localization.LocalizationElement<string> message, List<SimpleOption> options)
        {
            Tag = tag;
            Actor = actor;
            Emotion = emotion;
            Message = message;
            Options = options;
        }

    }
    public class SimpleOption
    {
        public EC.Localization.LocalizationElement<string> Message { get; }
        public Action Advance { get; }

        public SimpleOption(EC.Localization.LocalizationElement<string> message, Action advance)
        {
            Message = message;
            Advance = advance;
        }
    }
    [MovedFrom(true, null, "Assembly-CSharp")]
    [Serializable]
    public class SimplePortDefinition<T>
    {
        public string Guid;
        public EC.Localization.LocalizationElement<string> Value;

        public SimplePortDefinition(string guid, EC.Localization.LocalizationElement<string> value)
        {
            Guid = guid;
            Value = value;
        }
    }
}