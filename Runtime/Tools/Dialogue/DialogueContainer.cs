using System;
using System.Collections.Generic;
using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using Cysharp.Threading.Tasks;
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

        public virtual async UniTask StartDialogue()
        {
            if (Runner == null) Initialize();
            IsActive = true;
            await OnStarted();
            Runner.Begin();
        }

        public virtual async UniTask StopDialogue()
        {
            if (Runner != null) Runner.OnConversationEvent.RemoveListener(HandleEvent);
            Runner = null;
            IsActive = false;
            _messages.Clear();
            _choices.Clear();
            _users.Clear();
            await OnStopped();
        }

        protected virtual void HandleEvent(IConversationEvent e)
        {
            switch (e)
            {
                case SimpleMessageEvent m:
                    _messages.Enqueue(m);
                    OnMessageQueued(m).Forget();
                    break;

                case SimpleChoiceEvent c:
                    _choices.Enqueue(c);
                    OnChoiceQueued(c).Forget();
                    break;

                case SimpleEventEvent u:
                    _users.Enqueue(u);
                    OnUserEventQueued(u).Forget();
                    break;

                case EndEvent:
                    StopDialogue().ContinueWith(() => OnEnd()).Forget();
                    break;
            }
        }

        public void ProcessNextMessage() => OnProcessNextMessage().Forget();
        public async UniTask OnProcessNextMessage()
        {
            if (_messages.Count == 0) return;
            var m = _messages.Dequeue();
            await OnMessageUse(m);
            m.Advance();
        }

        public void ProcessAllMessages() => OnProcessAllMessages().Forget();
        public async UniTask OnProcessAllMessages()
        {
            while (_messages.Count > 0) 
            { 
                await OnProcessNextMessage(); 
                await UniTask.WaitForEndOfFrame(); 
            }
        }

        public void Choose(int optionIndex) => OnChoose(optionIndex).Forget();
        public async UniTask OnChoose(int optionIndex)
        {
            if (_choices.Count == 0) return;
            var c = _choices.Dequeue();
            await OnChoiceUse(c, optionIndex);
            c.Options[optionIndex].Advance();
        }

        public void ProcessNextUserEvent() => OnProcessNextUserEvent().Forget();
        public async UniTask OnProcessNextUserEvent()
        {
            if (_users.Count == 0) return;
            var u = _users.Dequeue();
            await OnUserEventUse(u);
            u.Advance();
        }

#pragma warning disable CS1998
        // Process
        protected virtual async UniTask OnStarted() { }
        protected virtual async UniTask OnStopped() { }
        protected virtual async UniTask OnEnd() { }

        // Add Block
        protected virtual async UniTask OnMessageQueued(SimpleMessageEvent e) { }
        protected virtual async UniTask OnChoiceQueued(SimpleChoiceEvent e) { }
        protected virtual async UniTask OnUserEventQueued(SimpleEventEvent e) { }

        // Use Block
        protected virtual async UniTask OnMessageUse(SimpleMessageEvent e) { }
        protected virtual async UniTask OnChoiceUse(SimpleChoiceEvent e, int optionIndex) { }
        protected virtual async UniTask OnUserEventUse(SimpleEventEvent e) { }
#pragma warning restore CS1998
    }
}
