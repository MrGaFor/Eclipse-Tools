using Sirenix.OdinInspector;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using Conversa.Runtime.Events;
using Conversa.Runtime.Nodes;

namespace EC.Dialogue
{
    [System.Serializable]
    public class DialogueBox
    {
        public Conversation Dialogue;
        [HideInEditorMode, ReadOnly] public ConversationRunner Runner;

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

        #region MessageEvent
        public MessageEvent CurrentMessageEvent;
        public virtual void OnMessageEvent(MessageEvent message) 
        {
            CurrentMessageEvent = message;
        }
        public virtual void OnMessageUse()
        {
            CurrentMessageEvent?.Advance();
            CurrentMessageEvent = null;
        }
        #endregion

        #region ChoiceEvent
        public ChoiceEvent CurrentChoiceEvent;
        public virtual void OnChoiceEvent(ChoiceEvent choice) 
        {
            CurrentChoiceEvent = choice;
        }
        public virtual void OnChoiceUse(int optionIndex)
        {
            CurrentChoiceEvent?.Options[optionIndex].Advance();
            CurrentChoiceEvent = null;
        }
        #endregion

        #region UserEvent
        public UserEvent CurrentUserEvent;
        public virtual void OnUserEvent(UserEvent user)
        {
            CurrentUserEvent = user;
        }
        public virtual void OnUserEventUse()
        {
            CurrentUserEvent?.Advance();
            CurrentUserEvent = null;
        }
        #endregion
    }
}
