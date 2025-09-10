using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIB
{
    /// <summary>
    /// Represents a thread-safe Singleton MonoBehaviour in Unity.
    /// </summary>
    /// <typeparam name="T">Type of the MonoBehaviour that needs to be a Singleton.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();

        /// <summary>
        /// Gets the instance of the Singleton. If it doesn't exist, it creates one.
        /// </summary>
        /// <returns>Instance of the Singleton.</returns>
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var instances = FindObjectsOfType<T>();

                        if (instances.Length > 1)
                        {
                            Debug.LogError($"Multiple instances of {typeof(T)}. Deleting extras.");

                            // Destroy all but keep one
                            for (int i = 1; i < instances.Length; i++)
                            {
                                Destroy(instances[i].gameObject);
                            }
                            _instance = instances[0];
                            return _instance;
                        }

                        if (instances.Length == 1)
                        {
                            return instances[0];
                        }

                        if (instances.Length == 0)
                        {
                            Debug.LogWarning($"An object attempted to get the instance of {typeof(T)} but it was not found." +
                                " A new instance has been created.");

                            GameObject newSingleton = new GameObject();
                            _instance = newSingleton.AddComponent<T>();
                            newSingleton.name = $"{typeof(T)} Singleton";
                        }
                    }
                    return _instance;
                }
            }
        }
    }
}
