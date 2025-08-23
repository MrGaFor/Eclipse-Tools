using Conversa.Editor;
using Conversa.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Conversa.Runtime.Events;

public class SimpleMassiveNodeView : BaseNodeView<SimpleMassiveNode>
{
    protected override string Title => "SimpleMassive";

    // Constructors

    public SimpleMassiveNodeView(Conversation conversation) : base(new SimpleMassiveNode(), conversation) { }

    public SimpleMassiveNodeView(SimpleMassiveNode data, Conversation conversation) : base(data, conversation) { }

    protected override void SetBody()    
    {
        string lang = EC.Localization.LocalizationSystem.ActiveLanguageEditor;

        VisualElement options = new VisualElement();
        options.style.marginTop = 5;
        options.style.marginRight = 5;
        options.style.marginLeft = 5;

        VisualElement optionsTitle = new VisualElement();
        optionsTitle.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        Label optionsLabel = new Label("Options");
        optionsLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        optionsTitle.Add(optionsLabel);
        Dictionary<string, VisualElement> optionElementsDict = new Dictionary<string, VisualElement>();

        Button addBtn = new Button(() =>
        {
            var newOption = new SimplePortDefinition<EC.Localization.LocalizationElement<string>>(System.Guid.NewGuid().ToString(), new EC.Localization.LocalizationElement<string>());
            Data.options.Add(newOption);
            AddOption(newOption);
        })
        { text = "+" };
        addBtn.style.marginTop = -3;
        optionsTitle.Add(addBtn);
        options.Add(optionsTitle);

        foreach (var option in Data.options)
            AddOption(option);
        bodyContainer.Add(options);

        void AddOption(SimplePortDefinition<EC.Localization.LocalizationElement<string>> option)
        {
            if (optionElementsDict.ContainsKey(option.Guid))
                return;
            VisualElement item = new VisualElement();
            item.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
            item.style.alignItems = Align.Center;

            TextField itemField = new TextField();
            itemField.style.width = 160;
            itemField.RegisterValueChangedCallback(e => option.Value.SetValue(e.newValue, EC.Localization.LocalizationSystem.ActiveLanguageEditor));
            itemField.SetValueWithoutNotify(option.Value.GetValue(lang));
            itemField.isDelayed = true;
            item.Add(itemField);

            EC.Localization.LocalizationSystem.SubscribeChangeEditor((value) =>
            {
                itemField.SetValueWithoutNotify(option.Value.GetValue(value));
            });

            Button removeBtn = new Button(() =>
            {
                if (GetPort(option.Guid, out Port portToRemove))
                    GraphView.DeleteElements(portToRemove.connections.ToList());

                Data.options.Remove(option);
                options.Remove(item);
                optionElementsDict.Remove(option.Guid);
            })
            { text = "X" };
            removeBtn.style.marginTop = -3;
            item.Add(removeBtn);

            options.Add(item);
            optionElementsDict[option.Guid] = item;
        }
    }

    // We need to extend this method, so that when we delete the node, the edges attached to
    // each option are included in the list of elements to delete, for "graphViewChanged"
    public override void CollectElements(HashSet<GraphElement> collectedElementSet, Func<GraphElement, bool> conditionFunc)
    {
        base.CollectElements(collectedElementSet, conditionFunc);

        var choicePorts = bodyContainer.Query<Port>().ToList();
        collectedElementSet.UnionWith(choicePorts.SelectMany(port => port.connections));
    }

}