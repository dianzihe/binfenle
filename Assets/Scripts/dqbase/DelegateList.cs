using System;
using System.Collections.Generic;
using UnityEngine;


public class DelegateAction
{
    public Action action { get; private set; }
    public bool oneshot { get; private set; }

    public DelegateAction(Action action, bool oneshot)
    {
        this.action = action;
        this.oneshot = oneshot;
    }
}


public class DelegateList
{
    private List<DelegateAction> list = new List<DelegateAction>();

    public void Append(Action value)
    {
        Append(value, false, false);
    }

    public void Append(Action value, bool oneshot, bool overlap)
    {
        Append(new DelegateAction(value, oneshot), overlap);
    }

    public void Append(DelegateAction del, bool overlap)
    {
        if (del.action == null)
            return;

        if (!overlap)
            Remove(del.action);

        list.Add(del);
    }

    public void Remove(DelegateAction del)
    {
        list.Remove(del);
    }

    public void Remove(Action value)
    {
        list.RemoveAll(x => x.action == value);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Invoke()
    {
        DelegateAction[] copy = list.ToArray();

        foreach (var c in copy)
        {
            c.action();

            if (c.oneshot)
            {
                list.Remove(c);
            }
        }
    }
}


public class EventSubscription : IDisposable
{
    private DelegateList source;
    private DelegateAction del;
    private bool disposed = false;

    public EventSubscription(DelegateList source, Action value, bool oneshot, bool overlap)
    {
        this.source = source;
        this.del = new DelegateAction(value, oneshot);

        source.Append(del, overlap);
    }

    public void Dispose()
    {
        if (!disposed)
        {
            if (source != null)
            {
                source.Remove(del);
            }

            System.GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}


public class DelegateAction<T>
{
    public Action<T> action { get; private set; }
    public bool oneshot { get; private set; }

    public DelegateAction(Action<T> action, bool oneshot)
    {
        this.action = action;
        this.oneshot = oneshot;
    }
}


public class DelegateList<T>
{
    private List<DelegateAction<T>> list = new List<DelegateAction<T>>();

    public void Append(Action<T> value)
    {
        Append(value, false, false);
    }

    public void Append(Action<T> value, bool oneshot, bool overlap)
    {
        Append(new DelegateAction<T>(value, oneshot), overlap);
    }

    public void Append(DelegateAction<T> del, bool overlap)
    {
        if (del.action == null)
            return;

        if (!overlap)
            Remove(del.action);

        list.Add(del);
    }

    public void Remove(DelegateAction<T> del)
    {
        list.Remove(del);
    }

    public void Remove(Action<T> value)
    {
        list.RemoveAll(x => x.action == value);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Invoke(T arg)
    {
        DelegateAction<T>[] copy = list.ToArray();

        foreach (var c in copy)
        {
            c.action(arg);

            if (c.oneshot)
            {
                list.Remove(c);
            }
        }
    }
}


public class EventSubscription<T> : IDisposable
{
    private DelegateList<T> source;
    private DelegateAction<T> del;
    private bool disposed = false;

    public EventSubscription(DelegateList<T> source, Action<T> value, bool oneshot, bool overlap)
    {
        this.source = source;
        this.del = new DelegateAction<T>(value, oneshot);

        source.Append(del, overlap);
    }

    public void Dispose()
    {
        if (!disposed)
        {
            if (source != null)
            {
                source.Remove(del);
            }

            System.GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}


public class DelegateAction<T1, T2>
{
    public Action<T1, T2> action { get; private set; }
    public bool oneshot { get; private set; }

    public DelegateAction(Action<T1, T2> action, bool oneshot)
    {
        this.action = action;
        this.oneshot = oneshot;
    }
}


public class DelegateList<T1, T2>
{
    private List<DelegateAction<T1, T2>> list = new List<DelegateAction<T1, T2>>();

    public void Append(Action<T1, T2> value)
    {
        Append(value, false, false);
    }

    public void Append(Action<T1, T2> value, bool oneshot, bool overlap)
    {
        Append(new DelegateAction<T1, T2>(value, oneshot), overlap);
    }

    public void Append(DelegateAction<T1, T2> del, bool overlap)
    {
        if (del.action == null)
            return;

        if (!overlap)
            Remove(del.action);

        list.Add(del);
    }

    public void Remove(DelegateAction<T1, T2> del)
    {
        list.Remove(del);
    }

    public void Remove(Action<T1, T2> value)
    {
        list.RemoveAll(x => x.action == value);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Invoke(T1 arg1, T2 arg2)
    {
        DelegateAction<T1, T2>[] copy = list.ToArray();

        foreach (var c in copy)
        {
            c.action(arg1, arg2);

            if (c.oneshot)
            {
                list.Remove(c);
            }
        }
    }
}


public class EventSubscription<T1, T2> : IDisposable
{
    private DelegateList<T1, T2> source;
    private DelegateAction<T1, T2> del;
    private bool disposed = false;

    public EventSubscription(DelegateList<T1, T2> source, Action<T1, T2> value, bool oneshot, bool overlap)
    {
        this.source = source;
        this.del = new DelegateAction<T1, T2>(value, oneshot);

        source.Append(del, overlap);
    }

    public void Dispose()
    {
        if (!disposed)
        {
            if (source != null)
            {
                source.Remove(del);
            }

            System.GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}


public class DelegateAction<T1, T2, T3>
{
    public Action<T1, T2, T3> action { get; private set; }
    public bool oneshot { get; private set; }

    public DelegateAction(Action<T1, T2, T3> action, bool oneshot)
    {
        this.action = action;
        this.oneshot = oneshot;
    }
}


public class DelegateList<T1, T2, T3>
{
    private List<DelegateAction<T1, T2, T3>> list = new List<DelegateAction<T1, T2, T3>>();

    public void Append(Action<T1, T2, T3> value)
    {
        Append(value, false, false);
    }

    public void Append(Action<T1, T2, T3> value, bool oneshot, bool overlap)
    {
        Append(new DelegateAction<T1, T2, T3>(value, oneshot), overlap);
    }

    public void Append(DelegateAction<T1, T2, T3> del, bool overlap)
    {
        if (del.action == null)
            return;

        if (!overlap)
            Remove(del.action);

        list.Add(del);
    }

    public void Remove(DelegateAction<T1, T2, T3> del)
    {
        list.Remove(del);
    }

    public void Remove(Action<T1, T2, T3> value)
    {
        list.RemoveAll(x => x.action == value);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Invoke(T1 arg1, T2 arg2, T3 arg3)
    {
        DelegateAction<T1, T2, T3>[] copy = list.ToArray();

        foreach (var c in copy)
        {
            c.action(arg1, arg2, arg3);

            if (c.oneshot)
            {
                list.Remove(c);
            }
        }
    }
}


public class EventSubscription<T1, T2, T3> : IDisposable
{
    private DelegateList<T1, T2, T3> source;
    private DelegateAction<T1, T2, T3> del;
    private bool disposed = false;

    public EventSubscription(DelegateList<T1, T2, T3> source, Action<T1, T2, T3> value, bool oneshot, bool overlap)
    {
        this.source = source;
        this.del = new DelegateAction<T1, T2, T3>(value, oneshot);

        source.Append(del, overlap);
    }

    public void Dispose()
    {
        if (!disposed)
        {
            if (source != null)
            {
                source.Remove(del);
            }

            System.GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}


public class EventSubscriber : IDisposable
{
    private List<IDisposable> subscriptions = new List<IDisposable>();
    private bool disposed = false;

    public void Append(DelegateList source, Action value)
    {
        Append(source, value, false, false);
    }

    public void Append(DelegateList source, Action value, bool oneshot, bool overlap)
    {
        var s = new EventSubscription(source, value, oneshot, overlap);
        subscriptions.Add(s);
    }

    public void Append<T>(DelegateList<T> source, Action<T> value)
    {
        Append<T>(source, value, false, false);
    }

    public void Append<T>(DelegateList<T> source, Action<T> value, bool oneshot, bool overlap)
    {
        var s = new EventSubscription<T>(source, value, oneshot, overlap);
        subscriptions.Add(s);
    }

    public void Append<T1, T2>(DelegateList<T1, T2> source, Action<T1, T2> value)
    {
        Append<T1, T2>(source, value, false, false);
    }

    public void Append<T1, T2>(DelegateList<T1, T2> source, Action<T1, T2> value, bool oneshot, bool overlap)
    {
        var s = new EventSubscription<T1, T2>(source, value, oneshot, overlap);
        subscriptions.Add(s);
    }

    public void Append<T1, T2, T3>(DelegateList<T1, T2, T3> source, Action<T1, T2, T3> value)
    {
        Append<T1, T2, T3>(source, value, false, false);
    }

    public void Append<T1, T2, T3>(DelegateList<T1, T2, T3> source, Action<T1, T2, T3> value, bool oneshot, bool overlap)
    {
        var s = new EventSubscription<T1, T2, T3>(source, value, oneshot, overlap);
        subscriptions.Add(s);
    }

    public void CancleAll()
    {
        foreach (var s in subscriptions)
        {
            s.Dispose();
        }

        subscriptions.Clear();
    }

    public void Dispose()
    {
        if (!disposed)
        {
            CancleAll();
            System.GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}


public static class EventSubscriberExtensions
{
    public static EventSubscriber GetSubscriber(this GameObject target)
    {
        return SubscriberComponent.GetSubscriber(target);
    }

    public static void AppendDelegate(this GameObject target, DelegateList source, Action value)
    {
        SubscriberComponent.GetSubscriber(target).Append(source, value, false, false);
    }

    public static void AppendDelegate(this GameObject target, DelegateList source, Action value, bool oneshot, bool overlap)
    {
        SubscriberComponent.GetSubscriber(target).Append(source, value, oneshot, overlap);
    }

    public static void AppendDelegate<T>(this GameObject target, DelegateList<T> source, Action<T> value)
    {
        SubscriberComponent.GetSubscriber(target).Append<T>(source, value, false, false);
    }

    public static void AppendDelegate<T>(this GameObject target, DelegateList<T> source, Action<T> value, bool oneshot, bool overlap)
    {
        SubscriberComponent.GetSubscriber(target).Append<T>(source, value, oneshot, overlap);
    }

    public static void AppendDelegate<T1, T2>(this GameObject target, DelegateList<T1, T2> source, Action<T1, T2> value)
    {
        SubscriberComponent.GetSubscriber(target).Append<T1, T2>(source, value, false, false);
    }

    public static void AppendDelegate<T1, T2>(this GameObject target, DelegateList<T1, T2> source, Action<T1, T2> value, bool oneshot, bool overlap)
    {
        SubscriberComponent.GetSubscriber(target).Append<T1, T2>(source, value, oneshot, overlap);
    }

    public static void AppendDelegate<T1, T2, T3>(this GameObject target, DelegateList<T1, T2, T3> source, Action<T1, T2, T3> value)
    {
        SubscriberComponent.GetSubscriber(target).Append<T1, T2, T3>(source, value, false, false);
    }

    public static void AppendDelegate<T1, T2, T3>(this GameObject target, DelegateList<T1, T2, T3> source, Action<T1, T2, T3> value, bool oneshot, bool overlap)
    {
        SubscriberComponent.GetSubscriber(target).Append<T1, T2, T3>(source, value, oneshot, overlap);
    }

    public static void CancleAllDelegates(this GameObject target)
    {
        SubscriberComponent.CancleAllDelegates(target);
    }

    public static bool GetCancleAllDelegatesOnDisable(this GameObject target)
    {
        return SubscriberComponent.GetCancleAllDelegatesOnDisable(target);
    }

    public static void SetCancleAllDelegatesOnDisable(this GameObject target, bool yes)
    {
        SubscriberComponent.SetCancleAllDelegatesOnDisable(target, yes);
    }
}