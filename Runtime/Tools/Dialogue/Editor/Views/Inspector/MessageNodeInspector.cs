using Conversa.Runtime;
using Conversa.Runtime.Nodes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Conversa.Editor
{
    public class MessageNodeInspector : BaseNodeInspector<MessageNode>
    {
        public MessageNodeInspector(MessageNode data, Conversation conversation) : base(data, conversation) { }

        protected override void SetBody()
        {
            var template = Resources.Load<VisualTreeAsset>("Inspectors/MessageNode");
            template.CloneTree(this);

            var actorField = this.Q<ActorField>();
            actorField.OnChange(HandleActorChange);
            actorField.SetValueWithoutNotify(data.Actor);

            var inputMessage = this.Q<TextField>("body");
            inputMessage.RegisterValueChangedCallback(UpdateMessage);
            inputMessage.SetValueWithoutNotify(data.Message);
            inputMessage.isDelayed = true;
        }

        private void HandleActorChange(string staticActor)
        {
            data.Actor = staticActor;
        }

        private void UpdateMessage(ChangeEvent<string> evt)
        {
            RegisterUndoStep();
            data.Message = evt.newValue;
        }
    }
}