using System;
using System.Linq;
using Conversa.Runtime.Events;
using UnityEngine;

namespace Conversa.Runtime.StackBlocks
{
    [Serializable]
    public class MessageBlockNode : BaseBlockNode
    {
        [SerializeField] private string message = "Test";
        
        [Slot("Message", "message", Flow.In, Capacity.One)]
        public string Message
        {
            get => message;
            set => message = value;
        }

        public override void Process(Conversation conversation, ConversationEvents conversationEvents)
        {
            void Advance()
            {
                var blockNode = conversation.GetNextBlockNode(Guid);
                conversation.Process(blockNode, conversationEvents);
            }


            var processedMessage = GetProcessedMessage(conversation);

            var e3 = new MessageEvent("", processedMessage, Advance);
            conversationEvents.OnMessage.Invoke(e3); // Deprecated
            conversationEvents.OnConversationEvent.Invoke(e3);
        }

        private string GetProcessedMessage(Conversation conversation) =>
            conversation.IsConnected(Guid, "message")
                ? conversation.GetConnectedValueTo<string>(this, "message")
                : Message;
    }
}