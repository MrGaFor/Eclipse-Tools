using Conversa.Editor;
using Conversa.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleMessageNodeView : BaseNodeView<SimpleMessageNode>
{
    protected override string Title => "SimpleMessage";

    // Constructors

    public SimpleMessageNodeView(Conversation conversation) : base(new SimpleMessageNode(), conversation) { }

    public SimpleMessageNodeView(SimpleMessageNode data, Conversation conversation) : base(data, conversation) { }

    protected override void SetBody()    
    {
        string lang = EC.Localization.LocalizationSystem.ActiveLanguageEditor;

        VisualElement tags = new VisualElement();
        tags.style.flexDirection = FlexDirection.Column;
        tags.style.marginTop = 5;
        tags.style.marginRight = 5;
        tags.style.marginLeft = 5;
        VisualElement tagsContainer = new VisualElement();
        tagsContainer.style.flexDirection = FlexDirection.Column;
        tags.style.marginBottom = 5;

        VisualElement tagsField = new VisualElement();
        tagsField.style.flexDirection = FlexDirection.Row;
        Label tagsLabel = new Label("Tags");
        tagsLabel.style.width = 70;
        TextField tagField = new TextField();
        tagField.style.width = 110;
        tagField.isDelayed = true;
        tagField.RegisterCallback<KeyDownEvent>(e =>
        {
            if (e.keyCode == KeyCode.Return && !string.IsNullOrEmpty(tagField.value))
            {
                var list = Data.tags.ToList();
                list.Add(tagField.value);
                Data.tags = list.ToArray();
                tagField.value = "";
                RefreshTags();
            }
        });
        var tagButton = new Button();
        tagButton.text = "...";
        tagButton.style.marginTop = 0;
        tagButton.style.height = 20;
        tagButton.style.width = 20;
        tagButton.clicked += () =>
        {
            GenericDropdownMenu tagsMenu = new GenericDropdownMenu();
            foreach (var t in EC.Dialogue.TagConfig.Tags)
            {
                tagsMenu.AddItem(t, Data.tags.Contains(t), () =>
                {
                    var list = Data.tags.ToList();
                    if (!list.Contains(t))
                        list.Add(t);
                    else
                        list.Remove(t);
                    Data.tags = list.ToArray();
                    RefreshTags();
                });
            }
            tagsMenu.DropDown(tagButton.worldBound, tagButton, false);
        };
        tagsField.Add(tagsLabel);
        tagsField.Add(tagField);
        tagsField.Add(tagButton);
        tags.Add(tagsField);
        tags.Add(tagsContainer);

        bodyContainer.Add(tags);
        RefreshTags();
        void RefreshTags()
        {
            tagsContainer.Clear();
            foreach (var t in Data.tags)
            {
                var tagChip = new VisualElement();
                tagChip.style.flexDirection = FlexDirection.Row;
                tagChip.style.marginRight = 4;
                tagChip.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
                tagChip.style.paddingLeft = 4;
                tagChip.style.paddingRight = 2;
                tagChip.style.alignItems = Align.Center;

                var label = new Label(t);
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.flexGrow = 1;

                var removeBtn = new Button(() =>
                {
                    Data.tags = Data.tags.Where(x => x != t).ToArray();
                    RefreshTags();
                })
                {
                    text = "x"
                };
                removeBtn.style.width = 16;
                removeBtn.style.height = 16;

                tagChip.Add(label);
                tagChip.Add(removeBtn);
                tagsContainer.Add(tagChip);
            }
        }

        VisualElement actor = new VisualElement();
        actor.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        actor.style.marginRight = 5;
        actor.style.marginLeft = 5;
        Label actorLabel = new Label("Actor");
        actorLabel.style.width = 70;
        actor.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        actor.Add(actorLabel);
        List<string> actors = new();
        Dictionary<string, EC.Localization.LocalizationElement<string>> actorsData = new();
        foreach (var actorElement in EC.Dialogue.ActorConfig.Actors)
        {
            actors.Add(actorElement.GetValue(lang));
            actorsData.Add(actorElement.GetValue(lang), actorElement);
        }
        DropdownField actorField = new DropdownField("", actors, 0);
        actorField.style.width = 130;
        actorField.RegisterValueChangedCallback(e => Data.actor = actorsData[e.newValue]);
        actorField.SetValueWithoutNotify(Data.actor.GetValue(lang));
        actor.Add(actorField);
        bodyContainer.Add(actor);

        VisualElement emotion = new VisualElement();
        emotion.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        emotion.style.marginRight = 5;
        emotion.style.marginLeft = 5;
        Label emotionLabel = new Label("Emotion");
        emotionLabel.style.width = 70;
        emotion.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        emotion.Add(emotionLabel);
        DropdownField emotionField = new DropdownField("", EC.Dialogue.EmotionConfig.Emotions.ToList(), 0);
        emotionField.style.width = 130;
        emotionField.RegisterValueChangedCallback(e => Data.emotion = e.newValue);
        emotionField.SetValueWithoutNotify(Data.emotion);
        emotion.Add(emotionField);
        bodyContainer.Add(emotion);

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

        EC.Localization.LocalizationSystem.SubscribeChangeEditor((value) => 
        {
            actors = new();
            actorsData = new();
            foreach (var actorElement in EC.Dialogue.ActorConfig.Actors)
            {
                actors.Add(actorElement.GetValue(value));
                actorsData.Add(actorElement.GetValue(value), actorElement);
            }
            actorField.choices = actors;
            actorField.SetValueWithoutNotify(Data.actor.GetValue(value));
            messageField.SetValueWithoutNotify(Data.message.GetValue(value));
        });
    }

}