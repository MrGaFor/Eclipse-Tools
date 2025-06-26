using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EC.Inputer
{
    [HideMonoScript]
    public static class InputKeyController
    {
        public static void Init()
        {
            SystemExemplier instance = new GameObject("[Eclipse Input]").AddComponent<SystemExemplier>();
            GameObject.DontDestroyOnLoad(instance.gameObject);
            Updater.UpdaterCore.AddUpdater(instance);
        }

        private readonly static Dictionary<KeyCode, List<Action>> SubscribersDown = new();
        private readonly static Dictionary<KeyCode, List<Action>> SubscribersUp = new();

        #region --- SUBSCRIBE ---
        public static void Subscribe(KeyCode key, Action action, bool isDown = true)
        {
            if (isDown)
            {
                if (SubscribersDown.ContainsKey(key))
                    SubscribersDown[key].Add(action);
                else
                    SubscribersDown.Add(key, new List<Action>() { action });
            }
            else
            {
                if (SubscribersUp.ContainsKey(key))
                    SubscribersUp[key].Add(action);
                else
                    SubscribersUp.Add(key, new List<Action>() { action });
            }
        }
        #endregion

        #region --- UNSUBSCRIBE ---
        public static void Unsubscribe(KeyCode key, Action action, bool isDown = true)
        {
            if (isDown)
            {
                if (SubscribersDown.ContainsKey(key) && SubscribersDown[key].Contains(action))
                    SubscribersDown[key].Remove(action);
                if (SubscribersDown.ContainsKey(key) && SubscribersDown[key].Count == 0)
                    SubscribersDown.Remove(key);
            }
            else
            {
                if (SubscribersUp.ContainsKey(key) && SubscribersUp[key].Contains(action))
                    SubscribersUp[key].Remove(action);
                if (SubscribersUp[key].Count == 0)
                    SubscribersUp.Remove(key);
            }
        }
        #endregion

        public static void CheckInput()
        {
            if (SubscribersDown.Count > 0)
                for (int i = SubscribersDown.Count - 1; i >= 0; i--)
                {
                    if (i >= SubscribersDown.Count)
                        continue;
                    KeyValuePair<KeyCode, List<Action>> pair = SubscribersDown.ElementAt(i);
                    if (UnityEngine.Input.GetKeyDown(pair.Key))
                        foreach (Action action in pair.Value)
                            action?.Invoke();
                }
            if (SubscribersUp.Count > 0)
                for (int i = SubscribersUp.Count - 1; i >= 0; i--)
                {
                    if (i >= SubscribersUp.Count)
                        continue;
                    KeyValuePair<KeyCode, List<Action>> pair = SubscribersUp.ElementAt(i);
                    if (UnityEngine.Input.GetKeyUp(pair.Key))
                        foreach (Action action in pair.Value)
                            action?.Invoke();
                }
        }
    }
    public class SystemExemplier : MonoBehaviour, Updater.IUpdaterObject
    {
        public int Priority => 1000;
        public void CustomUpdate()
        {
            InputKeyController.CheckInput();
        }
    }
}