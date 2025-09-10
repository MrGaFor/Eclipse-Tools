using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using GIB;

namespace UnityEngine.Events
{
    public delegate void UnityAction<T0, T1, T2, T3, T4>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    public delegate void UnityAction<T0, T1, T2, T3, T4, T5>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate void UnityAction<T0, T1, T2, T3, T4, T5, T6>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    public delegate void UnityAction<T0, T1, T2, T3, T4, T5, T6, T7>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    public delegate void UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    public delegate void UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

    /// <summary>
    /// Events Pro: An extension of UnityEvents
    /// </summary>
    [Serializable]
    public class EventPro : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[0];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction call) => AddCall(GetDelegate(call));

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction call) => RemoveListener(call.Target, call.Method);

        protected override MethodInfo FindMethod_Impl(object targetObj, string name) => GetValidMethodInfo(targetObj, name, new Type[0]);

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction) => new EP_InvokableCall(target, theFunction);

        private static EP_BaseInvokableCall GetDelegate(UnityAction action) => new EP_InvokableCall(action);

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        public void Invoke()
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke();
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall;
                if (curCall != null)
                    curCall.Invoke();
                else
                    calls[i].Invoke(m_InvokeArray);
            }
        }

        internal void AddPersistentListener(UnityAction call) => AddPersistentListener(call, EventProCallState.RuntimeOnly);

        internal void AddPersistentListener(UnityAction call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }

    /// <summary>
	/// Events Pro: one generic type
	/// </summary>
	[Serializable]
    public abstract class EventPro<T0> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[1];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0> call) => AddCall(GetDelegate(call));

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0> call) => RemoveListener(call.Target, call.Method);

        protected override MethodInfo FindMethod_Impl(object targetObj, string name) => GetValidMethodInfo(targetObj, name, new Type[1] { typeof(T0) });

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction) => new EP_InvokableCall<T0>(target, theFunction);

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0> action) => new EP_InvokableCall<T0>(action);

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        public void Invoke(T0 arg1)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0>;
                if (curCall != null)
                    curCall.Invoke(arg1);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }

    /// <summary>
	/// Events Pro with two generic types
	/// </summary>
	[Serializable]
    public abstract class EventPro<T0, T1> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[2];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[2] { typeof(T0), typeof(T1) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1> action)
        {
            return new EP_InvokableCall<T0, T1>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        public void Invoke(T0 arg1, T1 arg2)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }

    /// <summary>
    /// Events Pro with three generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[3];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[3] { typeof(T0), typeof(T1), typeof(T2) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2> action)
        {
            return new EP_InvokableCall<T0, T1, T2>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }

    /// <summary>
    /// Events Pro with four generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2, T3> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[4];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2, T3> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2, T3> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[4] { typeof(T0), typeof(T1), typeof(T2), typeof(T3) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2, T3>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3> action)
        {
            return new EP_InvokableCall<T0, T1, T2, T3>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        /// <param name="arg4">Dynamic argument 4</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3, T3 arg4)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3, arg4);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2, T3>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3, arg4);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        m_InvokeArray[3] = arg4;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }



    /// <summary>
    /// Events Pro with five generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2, T3, T4> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[5];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2, T3, T4> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2, T3, T4> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[5] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3, T4> action)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        /// <param name="arg4">Dynamic argument 4</param>
        /// <param name="arg5">Dynamic argument 5</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3, arg4, arg5);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2, T3, T4>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3, arg4, arg5);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        m_InvokeArray[3] = arg4;
                        m_InvokeArray[4] = arg5;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3, T4> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }



    /// <summary>
    /// Events Pro with six generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2, T3, T4, T5> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[6];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2, T3, T4, T5> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2, T3, T4, T5> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[6] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3, T4, T5> action)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        /// <param name="arg4">Dynamic argument 4</param>
        /// <param name="arg5">Dynamic argument 5</param>
        /// <param name="arg6">Dynamic argument 6</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, T5 arg6)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3, arg4, arg5, arg6);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2, T3, T4, T5>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        m_InvokeArray[3] = arg4;
                        m_InvokeArray[4] = arg5;
                        m_InvokeArray[5] = arg6;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3, T4, T5> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }



    /// <summary>
    /// Events Pro with seven generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2, T3, T4, T5, T6> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[7];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2, T3, T4, T5, T6> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2, T3, T4, T5, T6> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[7] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3, T4, T5, T6> action)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        /// <param name="arg4">Dynamic argument 4</param>
        /// <param name="arg5">Dynamic argument 5</param>
        /// <param name="arg6">Dynamic argument 6</param>
        /// <param name="arg7">Dynamic argument 7</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, T5 arg6, T6 arg7)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        m_InvokeArray[3] = arg4;
                        m_InvokeArray[4] = arg5;
                        m_InvokeArray[5] = arg6;
                        m_InvokeArray[6] = arg7;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3, T4, T5, T6> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }



    /// <summary>
    /// Events Pro with eight generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2, T3, T4, T5, T6, T7> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[8];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[8] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7> action)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        /// <param name="arg4">Dynamic argument 4</param>
        /// <param name="arg5">Dynamic argument 5</param>
        /// <param name="arg6">Dynamic argument 6</param>
        /// <param name="arg7">Dynamic argument 7</param>
        /// <param name="arg8">Dynamic argument 8</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, T5 arg6, T6 arg7, T7 arg8)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        m_InvokeArray[3] = arg4;
                        m_InvokeArray[4] = arg5;
                        m_InvokeArray[5] = arg6;
                        m_InvokeArray[6] = arg7;
                        m_InvokeArray[7] = arg8;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3, T4, T5, T6, T7> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }



    /// <summary>
    /// Events Pro with nine generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2, T3, T4, T5, T6, T7, T8> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[9];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[9] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7, T8>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7, T8>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        /// <param name="arg4">Dynamic argument 4</param>
        /// <param name="arg5">Dynamic argument 5</param>
        /// <param name="arg6">Dynamic argument 6</param>
        /// <param name="arg7">Dynamic argument 7</param>
        /// <param name="arg8">Dynamic argument 8</param>
        /// <param name="arg9">Dynamic argument 9</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, T5 arg6, T6 arg7, T7 arg8, T8 arg9)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7, T8>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        m_InvokeArray[3] = arg4;
                        m_InvokeArray[4] = arg5;
                        m_InvokeArray[5] = arg6;
                        m_InvokeArray[6] = arg7;
                        m_InvokeArray[7] = arg8;
                        m_InvokeArray[8] = arg9;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> call)
        {
            if (call == null)
                Debug.LogWarning("Registering a Listener requires an action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }



    /// <summary>
    /// Events Pro with ten generic types
    /// </summary>
    [Serializable]
    public abstract class EventPro<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : EventProBase
    {
        private readonly object[] m_InvokeArray = new object[10];

        /// <summary>
        ///   <para>Constructor.</para>
        /// </summary>
        public EventPro() { }

        /// <summary>
        /// Add a non persistent listener to the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void AddListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> call)
        {
            AddCall(GetDelegate(call));
        }

        /// <summary>
        /// Remove a non persistent listener from the UnityEvent.
        /// </summary>
        /// <param name="call">Callback function.</param>
        public void RemoveListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> call)
        {
            RemoveListener(call.Target, call.Method);
        }

        protected override MethodInfo FindMethod_Impl(object targetObj, string name)
        {
            return GetValidMethodInfo(targetObj, name, new Type[10] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) });
        }

        internal override EP_BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(target, theFunction);
        }

        private static EP_BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            return new EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(action);
        }

        /// <summary>
        /// Invoke all registered callbacks (runtime and persistent).
        /// </summary>
        /// <param name="arg1">Dynamic argument 1</param>
        /// <param name="arg2">Dynamic argument 2</param>
        /// <param name="arg3">Dynamic argument 3</param>
        /// <param name="arg4">Dynamic argument 4</param>
        /// <param name="arg5">Dynamic argument 5</param>
        /// <param name="arg6">Dynamic argument 6</param>
        /// <param name="arg7">Dynamic argument 7</param>
        /// <param name="arg8">Dynamic argument 8</param>
        /// <param name="arg9">Dynamic argument 9</param>
        /// <param name="arg10">Dynamic argument 10</param>
        public void Invoke(T0 arg1, T1 arg2, T2 arg3, T3 arg4, T4 arg5, T5 arg6, T6 arg7, T7 arg8, T8 arg9, T9 arg10)
        {
            List<EP_BaseInvokableCall> calls = PrepareInvoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            for (var i = 0; i < calls.Count; i++)
            {
                var curCall = calls[i] as EP_InvokableCall<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>;
                if (curCall != null)
                    curCall.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
                else
                {
                    var staticCurCall = calls[i] as EP_InvokableCall;
                    if (staticCurCall != null)
                        staticCurCall.Invoke();
                    else
                    {
                        m_InvokeArray[0] = arg1;
                        m_InvokeArray[1] = arg2;
                        m_InvokeArray[2] = arg3;
                        m_InvokeArray[3] = arg4;
                        m_InvokeArray[4] = arg5;
                        m_InvokeArray[5] = arg6;
                        m_InvokeArray[6] = arg7;
                        m_InvokeArray[7] = arg8;
                        m_InvokeArray[8] = arg9;
                        m_InvokeArray[9] = arg10;
                        calls[i].Invoke(m_InvokeArray);
                    }
                }
            }
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> call)
        {
            AddPersistentListener(call, EventProCallState.RuntimeOnly);
        }

        internal void AddPersistentListener(UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> call, EventProCallState callState)
        {
            int persistentEventCount = GetPersistentEventCount();
            AddPersistentListener();
            RegisterPersistentListener(persistentEventCount, call);
            SetPersistentListenerState(persistentEventCount, callState);
        }

        internal void RegisterPersistentListener(int index, UnityAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> call)
        {
            if (call == null)
                Debug.LogWarning("Attempted to register listener without an Action");
            else
                RegisterPersistentListener(index, call.Target, call.Method);
        }
    }
}
