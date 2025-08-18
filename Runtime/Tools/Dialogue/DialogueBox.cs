using Sirenix.OdinInspector;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using Conversa.Runtime.Events;
using Conversa.Runtime.Nodes;
using System;
using System.Collections.Generic;

namespace EC.Dialogue
{
    [System.Serializable]
    public class DialogueBox
    {
        public Conversation Dialogue;
        [HideInEditorMode, ReadOnly] public ConversationRunner Runner;

        public List<MessageEvent> MessageEvents = new();
        public List<ChoiceEvent> ChoiceEvents = new();
        public List<UserEvent> UserEvents = new();

        public virtual void Initialize()
        {
            Runner = new ConversationRunner(Dialogue);
            Runner.OnConversationEvent.AddListener(HandleDialogueEvent);
        }

        public virtual void StartDialogue()
        {
            Runner.Begin();
        }
        public virtual void StopDialogue() { }
        public virtual void HandleDialogueEvent(IConversationEvent e)
        {
            switch (e)
            {
                case MessageEvent messageEvent:
                    OnMessageEvent(messageEvent);
                    break;
                case ChoiceEvent choiceEvent:
                    OnChoiceEvent(choiceEvent);
                    break;
                case UserEvent userEvent:
                    OnUserEvent(userEvent);
                    break;
                case EndEvent endEvent:
                    StopDialogue();
                    break;
            }
        }

        public virtual void UseMessage()
        {
            if (MessageEvents.Count > 0)
                for (int i = MessageEvents.Count - 1; i >= 0; i--)
                {
                    MessageEvents[i].Advance();
                    MessageEvents.RemoveAt(i);
                }
        }
        private void OnMessageEvent(MessageEvent message)
        {
            MessageEvents.Add(message);
        }

        public virtual void UseChoice(int optionIndex)
        {
            if (ChoiceEvents.Count > 0)
                for (int i = ChoiceEvents.Count - 1; i >= 0; i--)
                {
                    ChoiceEvents[i].Options[optionIndex].Advance();
                    ChoiceEvents.RemoveAt(i);
                }
        }
        private void OnChoiceEvent(ChoiceEvent choice)
        {
            ChoiceEvents.Add(choice);
        }

        public virtual void UseUserEvent()
        {
            if (UserEvents.Count > 0)
                for (int i = UserEvents.Count - 1; i >= 0; i--)
                {
                    UserEvents[i].Advance();
                    UserEvents.RemoveAt(i);
                }
        }
        private void OnUserEvent(UserEvent user)
        {
            UserEvents.Add(user);
        }
    }
}
