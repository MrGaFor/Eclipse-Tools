using System;
using System.Collections.Generic;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;

[Serializable]
[Port("Index", "index", typeof(int), Flow.In, Capacity.One)]
[Port("Out", "out", typeof(EC.Localization.LocalizationElement<string>), Flow.Out, Capacity.Many)]
public class SimpleMassiveNode : BaseNode, IValueNode
{

    public List<SimplePortDefinition<EC.Localization.LocalizationElement<string>>> options = new List<SimplePortDefinition<EC.Localization.LocalizationElement<string>>>();

    public T GetValue<T>(string portGuid, Conversation conversation)
    {
        if (portGuid != "out") return default;

        var value2 = conversation.IsConnected(Guid, "index")
            ? conversation.GetConnectedValueTo<EC.Localization.LocalizationElement<string>>(this, "index")
            : new();

        int index = conversation.IsConnected(Guid, "index") ? conversation.GetConnectedValueTo<int>(this, "index") : 0;
        EC.Localization.LocalizationElement<string> option = options[index].Value;

        return Converter.ConvertValue<EC.Localization.LocalizationElement<string>, T>(option);
    }
}
