using System;
using Conversa.Runtime.Attributes;
using UnityEngine.Scripting.APIUpdating;

namespace Conversa.Runtime.Properties
{
    [MovedFrom(true, null, "Assembly-CSharp")]
    [Serializable]
    [ConversationProperty("Int", 200, 80, 80)]
    public class IntProperty : ValueProperty<int>
    {
        public IntProperty(string name) : base(name) { }
    }
}