using System;
using Conversa.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Conversa.Editor
{
    public class ActorField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ActorField, UxmlTraits> { }

        private Action<string> changeEventCallback;

        private readonly TextField actorTextField;
        
        public ActorField()
        {
            AddStyles();
                        
            actorTextField = new TextField("Actor");
            actorTextField.RegisterValueChangedCallback(evt => HandleChangeData());
            actorTextField.isDelayed = true;

            Add(actorTextField);
        }

        private void HandleChangeData()
        {
            UpdateVisibility();
            FireChangeCallback();
        }

        private void FireChangeCallback() =>
            changeEventCallback.Invoke(actorTextField.value);

        private void AddStyles() => styleSheets.Add(Resources.Load<StyleSheet>("ActorField"));

        private void UpdateVisibility()
        {
            ShowIf(actorTextField, true);
        }
        
        // public

        public void OnChange(Action<string> callback) => changeEventCallback = callback;

        public void SetValueWithoutNotify(string staticActor)
        {
            actorTextField.SetValueWithoutNotify(staticActor);
            UpdateVisibility();
        }
        
        // Static
        
        private static DisplayStyle GetDisplayStyle(bool visible) => visible ? DisplayStyle.Flex : DisplayStyle.None;

        private static void ShowIf(VisualElement el, bool visible) =>
            el.style.display = new StyleEnum<DisplayStyle>(GetDisplayStyle(visible));


    }
}