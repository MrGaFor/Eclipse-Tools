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

        VisualElement tag = new VisualElement();
        tag.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
        tag.style.marginTop = 5;
        tag.style.marginRight = 5;
        tag.style.marginLeft = 5;
        Label tagLabel = new Label("Tag");
        tagLabel.style.width = 70;
        tag.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
        tag.Add(tagLabel);
        TextField tagField = new TextField();
        tagField.style.width = 130;
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