using Conversa.Editor;
using Conversa.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomMessageNodeView : BaseNodeView<CustomMessageNode>
{
    protected override string Title => "CustomMessage";

    // Constructors

    public CustomMessageNodeView(Conversation conversation) : base(new CustomMessageNode(), conversation) { }

    public CustomMessageNodeView(CustomMessageNode data, Conversation conversation) : base(data, conversation) { }

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

        EC.Localization.LocalizationSystem.SubscribeChangeEditor((value) =>
        {
            actorField.SetValueWithoutNotify(Data.actor.GetValue(value));
        });
    }

}