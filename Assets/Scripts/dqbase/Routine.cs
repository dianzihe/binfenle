//#define LOG_ROUTINE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Utilities.Routines
{
    public interface IRoutine : IEnumerator, IDisposable
    {
        bool hasStarted { get; }
        bool isBroken { get; }
        bool isDone { get; }
        void Break();
        void SendMessage(string evt, object arg);
        IRoutine Append(IEnumerator appendRoutine);
    }


    public class Routine : IRoutine
    {
        public static int LogLevel = 0;
        public static RoutineLogPartten LogPartten = RoutineLogPartten.All;

        private static GameObject globalTarget = null;

        public static IRoutine Start(IEnumerator routine)
        {
            if (globalTarget == null)
            {
                globalTarget = new GameObject("Global Routines");
                GameObject.DontDestroyOnLoad(globalTarget);
            }

            return globalTarget.StartRoutine(routine);
        }

        public static IRoutine Enclosure(IEnumerator routine)
        {
            IRoutine r = routine as IRoutine;
            return r == null ? new Routine(routine) : r;
        }

        public static void BreakAll()
        {
            if (globalTarget != null)
            {
                globalTarget.BreakAllRoutines();
            }
        }

        public static void Broadcast(string evt, object arg)
        {
            if (globalTarget != null)
            {
                globalTarget.BroadcastRoutines(evt, arg);
            }
        }

        public static bool IsActive(IRoutine r)
        {
            return r != null && r.hasStarted && !r.isBroken && !r.isDone;
        }

        private IEnumerator main;
        private IRoutine sub;
        private bool isDisposed = false;
        private GameObject aidObject = null;
        private List<IRoutine> prepareAidRoutines = null;

        public bool hasStarted { get; private set; }
        public bool isBroken { get; private set; }
        public bool isDone { get; private set; }

        public Routine(IEnumerator main)
        {
            this.main = main;
            this.sub = null;
            this.hasStarted = false;
            this.isBroken = false;
            this.isDone = false;

#if LOG_ROUTINE && UNITY_EDITOR
            RoutineLogAttribute logAttr = GetLogAttribute();

            if (logAttr == null || logAttr.level < Routine.LogLevel)
            {
                logable = false;
            }
            else 
            {
                logable = true;
                logTag = logAttr.tag;
                logLevel = logAttr.level;
                logRoutineName = this.GetType().Name;
            }
#endif
        }

        public Routine()
            : this(null)
        { }

        public void Bind(IEnumerator main)
        {
            if (!hasStarted)
            {
                this.main = main;
            }
            else
            {
                throw new InvalidOperationException("Can't bind routine if already start.");
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public object Current
        {
            get { return sub == null ? (main == null ? null : main.Current) : sub.Current; }
        }

        public bool MoveNext()
        {
            if (main == null || isBroken || isDone)
            {
                return false;
            }

            if (!hasStarted)
            {
#if LOG_ROUTINE && UNITY_EDITOR
                if (logable && (Routine.LogPartten & RoutineLogPartten.Start) > 0)
                {
                    Debug.Log(">> START: " + MakeLogInfo());
                }
#endif
                hasStarted = true;

                if (prepareAidRoutines != null)
                {
                    if (aidObject == null)
                    {
                        aidObject = new GameObject("Aid Routines");
                        GameObject.DontDestroyOnLoad(aidObject);
                    }

                    foreach (var aid in prepareAidRoutines)
                    {
                        aidObject.StartRoutine(aid);
                    }

                    prepareAidRoutines.Clear();
                }
            }

            while (true)
            {
                if (sub == null)
                {
                    if (!main.MoveNext())
                    {
                        if (aidObject != null)
                        {
                            GameObject.Destroy(aidObject);
                            aidObject = null;
                        }

                        isDone = true;

#if LOG_ROUTINE && UNITY_EDITOR
                        if (logable && (Routine.LogPartten & RoutineLogPartten.Done) > 0)
                        {
                            Debug.Log("<< DONE: " + MakeLogInfo());
                        }
#endif
                        return false;
                    }

                    if (main.Current == null || !(main.Current is IEnumerator))
                    {
                        return true;
                    }

                    sub = Routine.Enclosure((IEnumerator)main.Current);

                    if (sub.hasStarted)
                    {
                        Debug.LogWarning("Yielded subroutine has already started.");
                    }
                }

                if (sub.MoveNext())
                {
                    return true;
                }
                else
                {
                    sub = null;
                }
            }
        }

        public void Break()
        {
            if (hasStarted && !isBroken && !isDone)
            {
                OnBreak();

                if (aidObject != null)
                {
                    GameObject.Destroy(aidObject);
                    aidObject = null;
                }

                if (sub != null)
                {
                    sub.Break();
                }

                isBroken = true;

#if LOG_ROUTINE && UNITY_EDITOR
                if (logable && (Routine.LogPartten & RoutineLogPartten.Break) > 0)
                {
                    Debug.Log("! ! ! BREAK: " + MakeLogInfo());
                }
#endif
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                Break();
                isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public void SendMessage(string msg, object arg)
        {
            if (hasStarted && !isBroken && !isDone)
            {
#if LOG_ROUTINE && UNITY_EDITOR
                if (logable && (Routine.LogPartten & RoutineLogPartten.Message) > 0)
                {
                    Debug.Log("- - - MESSAGE (" + msg + "): " + MakeLogInfo());
                }
#endif
                OnMessage(msg, arg);
            }
        }

        public IRoutine Append(IEnumerator appendRoutine)
        {
            if (isBroken || isDone)
            {
                return null;
            }

            if (hasStarted)
            {
                if (aidObject == null)
                {
                    aidObject = new GameObject("Aid Routines");
                    GameObject.DontDestroyOnLoad(aidObject);
                }

                return aidObject.StartRoutine(appendRoutine);
            }
            else
            {
                if (prepareAidRoutines == null)
                {
                    prepareAidRoutines = new List<IRoutine>();
                }

                IRoutine r = Routine.Enclosure(appendRoutine);
                prepareAidRoutines.Add(r);
                return r;
            }
        }

        protected virtual void OnBreak() { }

        protected virtual void OnMessage(string msg, object arg)         
        {
            if (sub != null)
            {
                sub.SendMessage(msg, arg);
            }

            if (aidObject != null)
            {
                aidObject.BroadcastRoutines(msg, arg);
            }
        }

#if LOG_ROUTINE && UNITY_EDITOR
        private bool logable = false;
        private string logRoutineName = null;
        private string logTag = null;
        private int logLevel = 0;

        private RoutineLogAttribute GetLogAttribute()
        {
            RoutineLogAttribute[] attrs = this.GetType().GetCustomAttributes(typeof(RoutineLogAttribute), true) as RoutineLogAttribute[];
            return (attrs != null && attrs.Length > 0) ? attrs[0] : null;
        }

        private string MakeLogInfo()
        {
            string info = logRoutineName;
            info += " [" + Time.time;

            if (logTag != null)
            {
                info += ", " + logTag;
            }

            info += "]";
            return info;
        }
#endif
    }


    public class RoutineBlock : IRoutine
    {
        private bool isDisposed = false;
        private GameObject aidObject = null;
        private List<IRoutine> prepareAidRoutines = null;

        public RoutineBlock()
        {
            hasStarted = false;
            isBroken = false;
            isDone = false;

#if LOG_ROUTINE && UNITY_EDITOR
            RoutineLogAttribute logAttr = GetLogAttribute();

            if (logAttr == null || logAttr.level < Routine.LogLevel)
            {
                logable = false;
            }
            else
            {
                logable = true;
                logTag = logAttr.tag;
                logLevel = logAttr.level;
                logRoutineName = this.GetType().Name;
            }
#endif
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public bool hasStarted { get; private set; }
        public bool isBroken { get; private set; }
        public bool isDone { get; private set; }
        public object Current { get { return null; } }

        public bool MoveNext()
        {
            if (isBroken || isDone)
            {
                return false;
            }

            if (!hasStarted)
            {
#if LOG_ROUTINE && UNITY_EDITOR
                if (logable && (Routine.LogPartten & RoutineLogPartten.Start) > 0)
                {
                    Debug.Log(">> START: " + MakeLogInfo());
                }
#endif
                hasStarted = true;

                if (prepareAidRoutines != null)
                {
                    if (aidObject == null)
                    {
                        aidObject = new GameObject("Aid Routines");
                        GameObject.DontDestroyOnLoad(aidObject);
                    }

                    foreach (var aid in prepareAidRoutines)
                    {
                        aidObject.StartRoutine(aid);
                    }

                    prepareAidRoutines.Clear();
                }
               
                OnStart();               
            }        

            if (OnBlock())
            {
                return true;
            }
            else
            {
                if (aidObject != null)
                {
                    GameObject.Destroy(aidObject);
                    aidObject = null;
                }

                isDone = true;
#if LOG_ROUTINE && UNITY_EDITOR
                if (logable && (Routine.LogPartten & RoutineLogPartten.Done) > 0)
                {
                    Debug.Log("<< DONE: " + MakeLogInfo());
                }
#endif
                return false;
            }
        }

        public void Break()
        {
            if (hasStarted && !isBroken && !isDone)
            {
                OnBreak();

                if (aidObject != null)
                {
                    GameObject.Destroy(aidObject);
                    aidObject = null;
                }

                isBroken = true;

#if LOG_ROUTINE && UNITY_EDITOR
                if (logable && (Routine.LogPartten & RoutineLogPartten.Break) > 0)
                {
                    Debug.Log("! ! ! BREAK: " + MakeLogInfo());
                }
#endif
            }
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                Break();
                isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public void SendMessage(string msg, object arg)
        {
            if (hasStarted && !isBroken && !isDone)
            {
#if LOG_ROUTINE && UNITY_EDITOR
                if (logable && (Routine.LogPartten & RoutineLogPartten.Message) > 0)
                {
                    Debug.Log("- - - MESSAGE (" + msg + "): " + MakeLogInfo());
                }
#endif
                OnMessage(msg, arg);
            }
        }

        public IRoutine Append(IEnumerator appendRoutine)
        {
            if (isBroken || isDone)
            {
                return null;
            }

            if (hasStarted)
            {
                if (aidObject == null)
                {
                    aidObject = new GameObject("Aid Routines");
                    GameObject.DontDestroyOnLoad(aidObject);
                }

                return aidObject.StartRoutine(appendRoutine);
            }
            else 
            {
                if (prepareAidRoutines == null)
                {
                    prepareAidRoutines = new List<IRoutine>();
                }

                IRoutine r = Routine.Enclosure(appendRoutine);
                prepareAidRoutines.Add(r);
                return r;
            }
        }

        protected virtual bool OnBlock() { return true; }
        protected virtual void OnStart() { }
        protected virtual void OnBreak() { }
        protected virtual void OnMessage(string msg, object arg) 
        {
            if (aidObject != null)
            {
                aidObject.BroadcastRoutines(msg, arg);
            }
        }

#if LOG_ROUTINE && UNITY_EDITOR
        private bool logable = false;
        private string logRoutineName = null;
        private string logTag = null;
        private int logLevel = 0;

        private RoutineLogAttribute GetLogAttribute()
        {
            RoutineLogAttribute[] attrs = this.GetType().GetCustomAttributes(typeof(RoutineLogAttribute), true) as RoutineLogAttribute[];
            return (attrs != null && attrs.Length > 0) ? attrs[0] : null;
        }

        private string MakeLogInfo()
        {
            string info = logRoutineName;
            info += " [" + Time.time;

            if (logTag != null)
            {
                info += ", " + logTag;
            }

            info += "]";
            return info;
        }
#endif
    }


    public static class RoutineExtensions
    {
        public static IRoutine AppendWith(this IRoutine mainRoutine, IEnumerator appendRoutine)
        {
            mainRoutine.Append(appendRoutine);
            return mainRoutine;
        }

        public static IRoutine StartRoutine(this GameObject target, IEnumerator routine)
        {
            RoutineComponent c = target.GetComponent<RoutineComponent>();

            if (c == null)
            {
                c = target.AddComponent<RoutineComponent>();
            }

            IRoutine r = Routine.Enclosure(routine);
            c.StartRoutine(r);
            return r;
        }

        public static void BreakAllRoutines(this GameObject target)
        {
            RoutineComponent c = target.GetComponent<RoutineComponent>();

            if (c != null)
            {
                c.BreakAllRoutines();
            }
        }

        public static void BroadcastRoutines(this GameObject target, string msg, object arg)
        {
            RoutineComponent c = target.GetComponent<RoutineComponent>();

            if (c != null)
            {
                c.Broadcast(msg, arg);
            }
        }
    }


    [Flags]
    public enum RoutineLogPartten
    {
        None = 0,
        Start = 1,
        Done = 2,
        Break = 4,
        Message = 8,
        Default = 7,
        All = 15
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class RoutineLogAttribute : Attribute
    {
        public int level { get; set; }
        public string tag { get; set; }

        public RoutineLogAttribute()
            : this(0)
        { }

        public RoutineLogAttribute(int level)
            : this(0, null)
        { }

        public RoutineLogAttribute(int level, string tag)
        {
            this.level = level;
            this.tag = tag;
        }
    }


    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UnblockPossibleAttribute : Attribute
    { }
}