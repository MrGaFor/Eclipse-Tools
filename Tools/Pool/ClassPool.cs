using System.Collections.Generic;
using System.Linq;

namespace EC.Pool
{
    public interface IClassPool
    {
        public bool IsActive { get; }
        public void SetActive(bool active);
    }
    public class ClassPool : IClassPool
    {
        public bool IsActive { get; private set; }
        public void SetActive(bool active)
        {
            IsActive = active;
        }
    }
    public class ClassPool<T> where T : IClassPool, new()
    {
        public ClassPool(int initCount = 0, int maxCount = 150)
        {
            Max = maxCount;
            Pool = new List<T> { };
            if (initCount > 0)
                for (int i = 0; i < initCount; i++)
                    CreateOne();
        }

        private readonly List<T> Pool;
        private int Max;

        public int AllCount => Pool.Count;
        public int ActiveCount => Pool.Count(obj => obj.IsActive);
        public int MaxCount => Max;

        #region Funcs
        public T GetOne()
        {
            if (Pool.Count > 0)
                for (int i = 0; i < Pool.Count; i++)
                {
                    var go = Pool[i];
                    if (!go.IsActive)
                    {
                        go.SetActive(true);
                        return Pool[i];
                    }
                }
            return CreateOne();
        }
        public void ReturnOne(T obj)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        private T CreateOne()
        {
            if (AllCount == Max)
                throw new System.Exception($"ClassPool<{typeof(T).Name}>: MaxCount reached");
            T obj = new T();
            obj.SetActive(true);
            Pool.Add(obj);
            return obj;
        }

        public List<T> GetAll() => Pool;
        public void ReturnAll()
        {
            if (Pool.Count > 0)
                for (int i = 0; i < Pool.Count; i++)
                    if (Pool[i].IsActive)
                        Pool[i].SetActive(false);
        }
        public void RemoveAll()
        {
            Pool.Clear();
        }

        public void ChangeMaxCount(int maxCount) => Max = maxCount;
        #endregion
    }
}