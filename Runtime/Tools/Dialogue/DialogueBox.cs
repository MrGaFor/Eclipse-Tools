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

        public virtual void InitializeRunner() => _box.InitializeRunner();
        public virtual void StartDialogue() => _box.StartDialogue();

        public virtual void HandleDialogueEvent(IConversationEvent e) => _box.HandleDialogueEvent(e);

        public virtual void OnMessageEvent(MessageEvent message) => _box.OnMessageEvent(message);
        public virtual void OnChoiceEvent(ChoiceEvent choice) => _box.OnChoiceEvent(choice);
        public virtual void OnUserEvent(UserEvent user) => _box.OnUserEvent(user);
        public virtual void StopDialogue() => _box.StopDialogue();
    }
    [System.Serializable]
    public class DialogueBox
    {
        [SerializeField] private Conversation _conversation;
        private ConversationRunner _runner;

        public virtual void InitializeRunner()
        {
            _runner = new ConversationRunner(_conversation);
            _runner.OnConversationEvent.AddListener(HandleDialogueEvent);
        }
        public virtual void StartDialogue()
        {
            _runner.Begin();
        }

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
        public virtual void OnChoiceEvent(ChoiceEvent choice) { }
        public virtual void OnUserEvent(UserEvent user) { }
        public virtual void StopDialogue() { }
    }
}
