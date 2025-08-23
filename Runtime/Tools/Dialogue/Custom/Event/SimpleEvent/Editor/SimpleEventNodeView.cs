using Conversa.Editor;
using Conversa.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleEventNodeView : BaseNodeView<SimpleEventNode>
{
    protected override string Title => "SimpleEvent";

    // Constructors

    public SimpleEventNodeView(Conversation conversation) : base(new SimpleEventNode(), conversation) { }

    public SimpleEventNodeView(SimpleEventNode data, Conversation conversation) : base(data, conversation) { }

    protected override void SetBody()    
    {
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
    }

}