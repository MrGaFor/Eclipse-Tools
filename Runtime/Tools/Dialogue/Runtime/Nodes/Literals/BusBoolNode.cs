/*using System;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Conversa.Runtime.Nodes.LogicalOperators
{
    [MovedFrom(true, null, "Assembly-CSharp")]
    [Serializable]
    [Port("Out", "out", typeof(bool), Flow.Out, Capacity.Many)]
    public class BusBoolNode : BaseNode, IValueNode
    {
        [SerializeField] private string key;

        public string Key
        {
            get => key;
            set => key = value;
        }

        public bool Value
        {
            get => EC.Bus.BusSystem.Get<bool>(key);
            set => EC.Bus.BusSystem.Set<bool>(key, value);
        }

        public override bool IsValid(Conversation conversation) => true;

        public T GetValue<T>(string portGuid, Conversation conversation)
        {
            return portGuid != "out" ? default : Converter.ConvertValue<bool, T>(Value);
        }
    }
}*/