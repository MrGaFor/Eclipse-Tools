/*using Conversa.Runtime;
using Conversa.Runtime.Nodes.LogicalOperators;
using UnityEngine.UIElements;

namespace Conversa.Editor
{
    public class BusBoolNodeView : BaseNodeView<BusBoolNode>
    {
        protected override string Title => "BusBool";

        public BusBoolNodeView(Conversation conversation) : base(new BusBoolNode(), conversation) { }

        public BusBoolNodeView(BusBoolNode data, Conversation conversation) : base(data, conversation) { }

        private TextField keyField;

        protected override void SetBody()
        {
            keyField = new TextField();
            keyField.SetValueWithoutNotify(Data.Key);
            keyField.RegisterValueChangedCallback(UpdateValue);
            keyField.isDelayed = true;

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(keyField);

            bodyContainer.Add(wrapper);
        }

        private void UpdateValue(ChangeEvent<string> e)
        {
            OnBeforeChange.Invoke();
            Data.Key = e.newValue;
        }
    }
}*/