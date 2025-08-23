using Conversa.Runtime;
using Conversa.Runtime.Nodes.MathOperators;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Conversa.Editor
{
    public class AbsoluteIntNodeView : BaseNodeView<AbsoluteIntNode>
    {
        protected override string Title => "Int";

        public AbsoluteIntNodeView(Conversation conversation) : base(new AbsoluteIntNode(), conversation) { }

        public AbsoluteIntNodeView(AbsoluteIntNode data, Conversation conversation) : base(data, conversation) { }

        private IntegerField intField;

        protected override void SetBody()
        {
            intField = new IntegerField();
            intField.SetValueWithoutNotify(Data.Value);
            intField.RegisterValueChangedCallback(UpdateValue);
            intField.isDelayed = true;

            var wrapper = new VisualElement();
            wrapper.AddToClassList("p-5");
            wrapper.Add(intField);

            bodyContainer.Add(wrapper);
        }

        private void UpdateValue(ChangeEvent<int> e)
        {
            OnBeforeChange.Invoke();
            Data.Value = e.newValue;
        }
    }
}