using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIB.Triggers
{
    public class TriggerCore { }

    /// <summary>
    /// An interface for calling events GIB.Triggers.
    /// </summary>
    public interface ITrigger
    {
        void Trigger();
    }
}
