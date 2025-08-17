using UnityEngine;
using Sirenix.OdinInspector;
using Conversa.Runtime;
using Conversa.Runtime.Interfaces;
using Conversa.Runtime.Events;

namespace EC.Dialogue
{
    public class MonoDialogueBox : MonoBehaviour
    {
        [SerializeField, HideLabel] private DialogueBox _box = new();

        public virtual void Awake() => Initialize();
        public virtual void Initialize() => _box.Initialize();
        public virtual void StartDialogue() => _box.StartDialogue();
        public virtual void StopDialogue() => _box.StopDialogue();

        public virtual void HandleDialogueEvent(IConversationEvent e) => _box.HandleDialogueEvent(e);

        public virtual void OnMessageEvent(MessageEvent message) => _box.OnMessageEvent(message);
        public virtual void OnMessageClick(MessageEvent message) => _box.OnMessageClick(message);

        public virtual void OnChoiceEvent(ChoiceEvent choice) => _box.OnChoiceEvent(choice);
        public virtual void OnChoiceClick(ChoiceEvent choice, int optionIndex) => _box.OnChoiceClick(choice, optionIndex);

        public virtual void OnUserEvent(UserEvent user) => _box.OnUserEvent(user);
        public virtual void OnUserEventClick(UserEvent user) => _box.OnUserEventClick(user);
    }
    [System.Serializable]
    public class DialogueBox
    {
        [SerializeField] private Conversation _conversation;
        private ConversationRunner _runner;

        public virtual void Initialize()
        {
            _runner = new ConversationRunner(_conversation);
            _runner.OnConversationEvent.AddListener(HandleDialogueEvent);
        }
        public virtual void StartDialogue()
        {
            _runner.Begin();
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

        public virtual void OnMessageEvent(MessageEvent message) { }
        public virtual void OnMessageClick(MessageEvent message)
        {
            message.Advance();
        }

        public virtual void OnChoiceEvent(ChoiceEvent choice) { }
        public virtual void OnChoiceClick(ChoiceEvent choice, int optionIndex)
        {
            if (optionIndex < 0 || optionIndex >= choice.Options.Count) return;
            var option = choice.Options[optionIndex];
            option.Advance();
        }

        public virtual void OnUserEvent(UserEvent user) { }
        public virtual void OnUserEventClick(UserEvent user)
        {
            user.Advance();
        }
    }
}
