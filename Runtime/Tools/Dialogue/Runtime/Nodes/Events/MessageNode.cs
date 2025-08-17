using System;
using System.Linq;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace Conversa.Runtime.Nodes
{
	[MovedFrom(true, null, "Assembly-CSharp")]
	[Serializable]
	[Port("Previous", "previous", typeof(BaseNode), Flow.In, Capacity.Many)]
	[Port("Next", "next", typeof(BaseNode), Flow.Out, Capacity.One)]
	public class MessageNode : BaseNode, IEventNode
	{
		public const string DefaultActor = "Actor name";
		public const string DefaultMessage = "Enter your message here";
		
		[SerializeField] private string actor = DefaultActor;
		[SerializeField] private string message = DefaultMessage;
		
		public string Actor
		{
			get => actor;
			set => actor = value;
		}

		public string Message
		{
			get => message;
			set => message = value;
		}
		
		public string ActorName => Actor?? "";

		public MessageNode() { }

		public void Process(Conversation conversation, ConversationEvents conversationEvents)
		{
			void Advance()
			{
				var nodePort = GetNodePort("next");
				var oppositeNodes = conversation.GetOppositeNodes(nodePort);
				var nextNode = oppositeNodes.FirstOrDefault();
				conversation.Process(nextNode, conversationEvents);
			}

			var e = new MessageEvent(actor, message, Advance);
			conversationEvents.OnMessage.Invoke(e); // Deprecated
			conversationEvents.OnConversationEvent.Invoke(e);
		}
	}
}