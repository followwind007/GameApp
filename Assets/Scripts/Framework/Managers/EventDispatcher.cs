using System;
using System.Collections.Generic;
using LuaInterface;

public class EventDispatcher
{
    private static EventDispatcher _instance;
    public static EventDispatcher Instance
    {
        get { return _instance ?? (_instance = new EventDispatcher()); }
    }

    private EventDispatcher() { }

    private readonly Dictionary<string, Delegate> _listeners = new Dictionary<string, Delegate>();

    public void AddListener<T1, T2, T3, T4>(string evt, Action<T1, T2, T3, T4> callback)
    {
        AddListener(evt, (Delegate)callback);
    }

    public void AddListener<T1, T2, T3>(string evt, Action<T1, T2, T3> callback)
    {
        AddListener(evt, (Delegate)callback);
    }

    public void AddListener<T1, T2>(string evt, Action<T1, T2> callback)
    {
        AddListener(evt, (Delegate)callback);
    }

    public void AddListener<T>(string evt, Action<T> callback)
    {
        AddListener(evt, (Delegate)callback);
    }

    public void AddListener(string evt, Action callback)
    {
        AddListener(evt, (Delegate)callback);
    }

    public void AddListener(string evt, Delegate callback)
    {
        if (_listeners.TryGetValue(evt, out var listener))
        {
            _listeners[evt] = Delegate.Combine(listener, callback);
        }
        else
        {
            _listeners[evt] = callback;
        }
    }

    public void RemoveListener<T1, T2, T3, T4>(string evt, Action<T1, T2, T3, T4> callback)
    {
        RemoveListener(evt, (Delegate)callback);
    }

    public void RemoveListener<T1, T2, T3>(string evt, Action<T1, T2, T3> callback)
    {
        RemoveListener(evt, (Delegate)callback);
    }

    public void RemoveListener<T1, T2>(string evt, Action<T1, T2> callback)
    {
        RemoveListener(evt, (Delegate)callback);
    }

    public void RemoveListener<T>(string evt, Action<T> callback)
    {
        RemoveListener(evt, (Delegate)callback);
    }

    public void RemoveListener(string evt, Action callback)
    {
        RemoveListener(evt, (Delegate)callback);
    }

    private void RemoveListener(string evt, Delegate callback)
    {
        if (_listeners.TryGetValue(evt, out var listener))
        {
            listener = Delegate.Remove(listener, callback);
            if (listener == null)
            {
                _listeners.Remove(evt);
            }
            else
            {
                _listeners[evt] = listener;
            }
        }
    }

    public void Dispatch<T1, T2, T3, T4>(string evt, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        var methods = GetMethods(evt);
        if (methods != null)
        {
            foreach (var m in methods)
            {
                try
                {
                    if (m.Target != null) ((Action<T1, T2, T3, T4>)m)(arg1, arg2, arg3, arg4);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch<T1, T2, T3>(string evt, T1 arg1, T2 arg2, T3 arg3)
    {
        var methods = GetMethods(evt);
        if (methods != null)
        {
            foreach (var m in methods)
            {
                try
                {
                    if (m.Target != null) ((Action<T1, T2, T3>)m)(arg1, arg2, arg3);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch<T1, T2>(string evt, T1 arg1, T2 arg2)
    {
        var methods = GetMethods(evt);
        if (methods != null)
        {
            foreach (var m in methods)
            {
                try
                {
                    if (m.Target != null) ((Action<T1, T2>)m)(arg1, arg2);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch<T>(string evt, T arg)
    {
        var methods = GetMethods(evt);
        if (methods != null)
        {
            foreach (var m in methods)
            {
                try
                {
                    if (m.Target != null) ((Action<T>)m)(arg);
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch(string evt)
    {
        var methods = GetMethods(evt);
        if (methods != null)
        {
            foreach (var m in methods)
            {
                try
                {
                    if (m.Target != null) ((Action)m)();
                }
                catch (Exception e) { LogError(e); }
            }
        }
    }

    public void Dispatch(string evt, LuaTable tbl)
    {
        Dispatch<LuaTable>(evt, tbl);
    }

    public void Dispatch(string evt, string msg)
    {
        Dispatch<string>(evt, msg);
    }

    public void DispatchLua(string evt, string msg = null)
    {
#if TEMPLATE_MODE
        var state = Framework.Manager.LuaManager.Instance.State;
#else
        var state = LuaManager.Instance ? LuaManager.Instance.State : null;
        #endif
        if (state != null)
        {
            state.Call("dispatcher.onMessage", evt, msg, true);
        }
    }

    private Delegate[] GetMethods(string evt)
    {
        return !_listeners.TryGetValue(evt, out var listener) ? null : listener.GetInvocationList();
    }

    private static void LogError(Exception e)
    {
        UnityEngine.Debug.LogError(e);
    }

}