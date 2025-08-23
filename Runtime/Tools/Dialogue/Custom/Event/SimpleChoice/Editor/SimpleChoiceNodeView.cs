using Conversa.Editor;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleChoiceNodeView : BaseNodeView<SimpleChoiceNode>
{
    protected override string Title => "SimpleChoice";

    public SimpleChoiceNodeView(Conversation conversation) : base(new SimpleChoiceNode(), conversation) { }
    public SimpleChoiceNodeView(SimpleChoiceNode data, Conversation conversation) : base(data, conversation) { }

    protected override void SetBody()
    {
        string lang = EC.Localization.LocalizationSystem.ActiveLanguageEditor;

        VisualElement tag = new VisualElement();
        tag.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        tag.style.marginTop = 5;
        tag.style.marginRight = 5;
        tag.style.marginLeft = 5;
        Label tagLabel = new Label("Tag");
        tagLabel.style.width = 40;
        tag.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        tag.Add(tagLabel);
        TextField tagField = new TextField();
        tagField.style.width = 160;
        tagField.SetValueWithoutNotify(Data.tag);
        tagField.RegisterValueChangedCallback(e => Data.tag = e.newValue);
        tagField.isDelayed = true;
        tag.Add(tagField);
        bodyContainer.Add(tag);

        VisualElement actor = new VisualElement();
        actor.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        actor.style.marginRight = 5;
        actor.style.marginLeft = 5;
        Label actorLabel = new Label("Actor");
        actorLabel.style.width = 40;
        actor.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        actor.Add(actorLabel);
        TextField actorField = new TextField();
        actorField.style.width = 160;
        actorField.RegisterValueChangedCallback(e => Data.actor.SetValue(e.newValue, EC.Localization.LocalizationSystem.ActiveLanguageEditor));
        actorField.SetValueWithoutNotify(Data.actor.GetValue(lang));
        actorField.isDelayed = true;
        actor.Add(actorField);
        bodyContainer.Add(actor);

        VisualElement message = new VisualElement();
        message.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        message.style.marginRight = 5;
        message.style.marginLeft = 5;
        Label messageLabel = new Label("Message");
        message.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        message.Add(messageLabel);
        TextField messageField = new TextField();
        messageField.multiline = true;
        messageField.style.width = 200;
        messageField.RegisterValueChangedCallback(e => Data.message.SetValue(e.newValue, EC.Localization.LocalizationSystem.ActiveLanguageEditor));
        messageField.SetValueWithoutNotify(Data.message.GetValue(lang));
        messageField.isDelayed = true;
        message.Add(messageField);
        bodyContainer.Add(message);

        VisualElement options = new VisualElement();
        options.style.marginRight = 5;
        options.style.marginLeft = 5;

        EC.Localization.LocalizationSystem.SubscribeChangeEditor((value) =>
        {
            actorField.SetValueWithoutNotify(Data.actor.GetValue(value));
            messageField.SetValueWithoutNotify(Data.message.GetValue(value));
        });

        VisualElement optionsTitle = new VisualElement();
        optionsTitle.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        Label optionsLabel = new Label("Options");
        optionsLabel.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        optionsTitle.Add(optionsLabel);
        Dictionary<string, VisualElement> optionElementsDict = new Dictionary<string, VisualElement>();
        Button addBtn = new Button(() =>
        {
            var newOption = new SimplePortDefinition<BaseNode>(System.Guid.NewGuid().ToString(), new EC.Localization.LocalizationElement<string>());
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

        void AddOption(SimplePortDefinition<BaseNode> option)
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

            ChoiceOption optionElement = new ChoiceOption(option);
            optionElement.style.marginTop = -3;
            item.Add(optionElement);
            if (!GetPort(option.Guid, out Port port))
                RegisterPort(optionElement.port, option.Guid);

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
public class ChoiceOption : VisualElement
{
    public readonly SimplePortDefinition<BaseNode> portDefinition;
    public readonly Port port;
    private readonly Label label;

    public ChoiceOption(SimplePortDefinition<BaseNode> portDefinition)
    {
        this.portDefinition = portDefinition;

        port = General.OutputFlowPort();
        port.portName = "";
        Add(port);
    }

    public void Update()
    {
        label.text = portDefinition.Value.GetValue(EC.Localization.LocalizationSystem.ActiveLanguageEditor);
    }
}