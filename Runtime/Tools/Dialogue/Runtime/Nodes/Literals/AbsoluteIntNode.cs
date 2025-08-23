using System;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Conversa.Runtime.Nodes.MathOperators
{
    [MovedFrom(true, null, "Assembly-CSharp")]
    [Serializable]
    [Port("Out", "out", typeof(int), Flow.Out, Capacity.Many)]
    public class AbsoluteIntNode : BaseNode, IValueNode
    {
        [SerializeField] private int value;
        public int Value
        {
            get => value;
            set => this.value = value;
        }

        public override bool IsValid(Conversation conversation) => true;

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            return portGuid != "out" ? default : Converter.ConvertValue<int, T>(value);
        }
    }
}