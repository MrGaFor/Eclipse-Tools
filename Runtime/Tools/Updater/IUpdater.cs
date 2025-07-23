using UnityEngine;
using Sirenix.OdinInspector;

namespace EC.Updater
{
    public interface IUpdaterObject
    {
        int Priority { get; }
        void CustomUpdate();
    }
    public interface IFixedUpdaterObject
    {
        int Priority { get; }
        void CustomFixedUpdate();
    }
    public interface ILateUpdaterObject
    {
        int Priority { get; }
        void CustomLateUpdate();
    }
}
