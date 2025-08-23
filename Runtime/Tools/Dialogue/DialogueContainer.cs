using System;
using System.Collections.Generic;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EC.Dialogue
{
    public class DialogueContainer : MonoBehaviour
    {
        public Conversation Dialogue;
        [HideInEditorMode, ReadOnly] public ConversationRunner Runner;

        protected readonly Queue<SimpleMessageEvent> _messages = new();
        protected readonly Queue<SimpleChoiceEvent> _choices = new();
        protected readonly Queue<SimpleEventEvent> _users = new();

        public bool IsActive { get; private set; }

        public virtual void Initialize(Conversation dialogue)
        {
            Dialogue = dialogue;
            Initialize();
        }

        public virtual void Initialize()
        {
            if (Dialogue == null) throw new InvalidOperationException("Dialogue is null");
            Runner = new ConversationRunner(Dialogue);
            Runner.OnConversationEvent.AddListener(HandleEvent);
        }

        public virtual void StartDialogue()
        {
            if (Runner == null) Initialize();
            IsActive = true;
            OnStarted();
            Runner.Begin();
        }

        public virtual void StopDialogue()
        {
            if (Runner != null) Runner.OnConversationEvent.RemoveListener(HandleEvent);
            IsActive = false;
            _messages.Clear();
            _choices.Clear();
            _users.Clear();
            OnStopped();
        }

        protected virtual void HandleEvent(IConversationEvent e)
        {
            switch (e)
            {
                case SimpleMessageEvent m:
                    _messages.Enqueue(m);
                    OnMessageQueued(m);
                    break;

                case SimpleChoiceEvent c:
                    _choices.Enqueue(c);
                    OnChoiceQueued(c);
                    break;

                case SimpleEventEvent u:
                    _users.Enqueue(u);
                    OnUserEventQueued(u);
                    break;

                case EndEvent:
                    OnEnd();
                    StopDialogue();
                    break;
            }
        }

        public void ProcessNextMessage()
        {
            if (_messages.Count == 0) return;
            var m = _messages.Dequeue();
            OnMessage(m);
            m.Advance();
        }

        public void ProcessAllMessages()
        {
            while (_messages.Count > 0) ProcessNextMessage();
        }

        public void Choose(int optionIndex)
        {
            if (_choices.Count == 0) return;
            var c = _choices.Dequeue();
            OnChoice(c, optionIndex);
            c.Options[optionIndex].Advance();
        }

        public void ProcessNextUserEvent()
        {
            if (_users.Count == 0) return;
            var u = _users.Dequeue();
            OnUserEvent(u);
            u.Advance();
        }

        protected virtual void OnMessage(SimpleMessageEvent e) { }
        protected virtual void OnChoice(SimpleChoiceEvent e, int optionIndex) { }
        protected virtual void OnUserEvent(SimpleEventEvent e) { }

        protected virtual void OnMessageQueued(SimpleMessageEvent e) { }
        protected virtual void OnChoiceQueued(SimpleChoiceEvent e) { }
        protected virtual void OnUserEventQueued(SimpleEventEvent e) { }
        protected virtual void OnStarted() { }
        protected virtual void OnStopped() { }
        protected virtual void OnEnd() { }
    }
}
