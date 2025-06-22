using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EC.Pool
{
    public class ComponentPool<T> where T : Component
    {
        public ComponentPool(T prefab, Transform parent, int initCount = 0, int maxCount = 150)
        {
            Prefab = prefab;
            Parent = parent;
            Max = maxCount;
            Pool = new List<T> { };
            if (initCount > 0)
                for (int i = 0; i < initCount; i++)
                    CreateOne();
        }

        private readonly T Prefab;
        private readonly Transform Parent;
        private readonly List<T> Pool;
        private int Max;

        public int AllCount => Pool.Count;
        public int ActiveCount => Pool.Count(obj => obj.gameObject.activeSelf);
        public int MaxCount => Max;

        #region Funcs
        public T GetOne(bool needState = true)
        {
            if (Pool.Count > 0)
                for (int i = 0; i < Pool.Count; i++)
                {
                    var go = Pool[i].gameObject;
                    if (!go.activeSelf)
                    {
                        go.SetActive(needState);
                        return Pool[i];
                    }
                }
            return CreateOne(needState);
        }
        public void ReturnOne(T obj)
        {
            if (obj != null)
                obj.gameObject.SetActive(false);
        }
        private T CreateOne(bool needState = false)
        {
            if (AllCount == Max)
                throw new System.Exception($"ComponentPool<{typeof(T).Name}>: MaxCount reached");
            var obj = Object.Instantiate(Prefab, Parent);
            obj.gameObject.SetActive(needState);
            Pool.Add(obj);
            return obj;
        }

        public List<T> GetAll() => Pool;
        public void ReturnAll()
        {
            if (Pool.Count > 0)
                for (int i = 0; i < Pool.Count; i++)
                    if (Pool[i].gameObject.activeSelf)
                        Pool[i].gameObject.SetActive(false);
        }
        public void RemoveAll()
        {
            foreach (var obj in Pool)
                if (obj != null)
                    if (Application.isPlaying)
                        Object.Destroy(obj.gameObject);
                    else
                        Object.DestroyImmediate(obj.gameObject);
            Pool.Clear();
        }

        public void ChangeMaxCount(int maxCount) => Max = maxCount;
        #endregion
    }
}