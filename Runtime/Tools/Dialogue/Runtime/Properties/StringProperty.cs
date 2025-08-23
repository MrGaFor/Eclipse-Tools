using System;
using Conversa.Runtime.Attributes;

namespace Conversa.Runtime.Properties
{
    [Serializable]
    [ConversationProperty("String", 170, 90, 200)]
    public class StringProperty : ValueProperty<string>
    {
        public StringProperty(string name) : base(name) { }
    }
}