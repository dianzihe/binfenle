using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Utilities.Routines
{
    public sealed class Wait : RoutineBlock
    {
        public float duration { get; private set; }
        public float destTime { get; private set; }

        public Wait(float duration)
        {
            this.duration = Mathf.Max(0f, duration);
        }

        protected override void OnStart()
        {
            destTime = Time.time + duration + 0.001f;
        }

        protected override bool OnBlock()
        {
            return Time.time < destTime;
        }
    }


    public sealed class WaitFrames : RoutineBlock
    {
        public float count { get; private set; }
        public float destFrameCount { get; private set; }

        public WaitFrames(int count)
        {
            this.count = count;
        }

        protected override void OnStart()
        {
            destFrameCount = Time.frameCount + count;
        }

        protected override bool OnBlock()
        {
            return Time.frameCount < destFrameCount;
        }
    }


    public sealed class WaitFor : RoutineBlock
    {
        private Func<bool> condition;

        public WaitFor(Func<bool> condition)
        {
            this.condition = condition;
        }

        protected override bool OnBlock()
        {
            return condition != null && !condition();
        }
    }


    public sealed class Protecter : Routine
    {
        private int frame;

        public Protecter()
        {
            this.frame = Time.frameCount;
            Bind(DoProtecter());
        }

        private IEnumerator DoProtecter()
        {
            if (Time.frameCount == frame)
            {
                yield return null;
            }
        }
    }


    public sealed class Act : Routine
    {
        public Act(Action action)
        {
            Bind(DoAct(action));
        }

        private IEnumerator DoAct(Action action)
        {
            if (action != null)
            {
                action();
            }

            yield break;
        }
    }


    public abstract class YieldRoutine : Routine
    {
        public YieldRoutine()
        {
            Bind(DoYield());
        }

        protected abstract IEnumerator DoYield();
    }


    public sealed class Sequencer : Routine
    {
        public Sequencer(params IEnumerator[] routines)
        {
            Bind(DoSequencer(routines));
        }

        private IEnumerator DoSequencer(IEnumerator[] routines)
        {
            foreach (var r in routines)
            {
                yield return r;
            }
        }
    }


    public sealed class Parallel : RoutineBlock
    {
        private IRoutine[] routines;

        public Parallel(params IEnumerator[] routines)
        {
            int length = routines.Length;
            this.routines = new IRoutine[length];

            for (int i = 0; i < length; ++i)
            {
                this.routines[i] = Routine.Enclosure(routines[i]);
            }
        }

        protected override bool OnBlock()
        {
            bool finished = true;

            foreach (var r in routines)
            {
                finished &= !r.MoveNext();
            }

            if (finished)
                BreakAll();

            return !finished;
        }

        protected override void OnBreak()
        {
            BreakAll();
        }

        private void BreakAll()
        {
            foreach (var r in routines)
            {
                r.Break();
            }
        }
    }


    public sealed class Selector : RoutineBlock
    {
        private IRoutine[] routines;

        public Selector(params IEnumerator[] routines)
        {
            int length = routines.Length;
            this.routines = new IRoutine[length];

            for (int i = 0; i < length; ++i)
            {
                this.routines[i] = Routine.Enclosure(routines[i]);
            }
        }

        protected override bool OnBlock()
        {
            bool finished = routines.Length == 0;

            foreach (var r in routines)
            {
                finished |= !r.MoveNext();
            }

            if (finished)
                BreakAll();

            return !finished;
        }

        protected override void OnBreak()
        {
            BreakAll();
        }

        private void BreakAll()
        {
            foreach (var r in routines)
            {
                r.Break();
            }
        }
    }


    public sealed class Predicater : RoutineBlock
    {
        private IRoutine main;
        private Func<bool> predicate;

        public Predicater(IEnumerator main, Func<bool> predicate)
        {
            this.main = Routine.Enclosure(main);
            this.predicate = predicate;
        }

        protected override bool OnBlock()
        {
            if (predicate == null || predicate())
            {
                return main.MoveNext();
            }
            else 
            {
                main.Break();
                return false;
            }
        }

        protected override void OnBreak()
        {
            main.Break();
        }
    }

    
    public sealed class WaitForRoutineFinish : Routine
    {
        public WaitForRoutineFinish(IRoutine r)
            : this(r, true)
        { }

        public WaitForRoutineFinish(IRoutine r, bool skipIfNotStarted)
        {
            Bind(DoWaitForRoutineFinish(r, skipIfNotStarted));
        }

        private IEnumerator DoWaitForRoutineFinish(IRoutine r, bool skipIfNotStarted)
        {
            yield return new WaitFor(() => r.isBroken || r.isDone || (skipIfNotStarted && !r.hasStarted));
        }
    }

    public class SimpleListener : RoutineBlock
    {
        public bool hasTriggered { get; private set; }
        public string triggerMsg { get; private set; }
        public object triggerArg { get; private set; }
        public float triggerTime { get; private set; }
        public int triggerFrameCount { get; private set; }

        public float timeSinceTrigger
        {
            get { return hasTriggered ? Time.time - triggerTime : -1f; }
        }

        public float frameCountSinceTrigger
        {
            get { return hasTriggered ? Time.frameCount - triggerFrameCount : -1; }
        }

        private string msg;
        private Action<object> callback;
        private bool blocked = true;

        public SimpleListener(string msg)
            : this(msg, null)
        { }

        public SimpleListener(string msg, Action<object> onTrigger)
        {
            this.msg = msg;
            this.callback = onTrigger;
            this.hasTriggered = false;
            this.triggerMsg = null;
            this.triggerArg = null;
            this.triggerTime = -1f;
            this.triggerFrameCount = -1;
        }

        protected override bool OnBlock()
        {
            return blocked;
        }

        protected override void OnMessage(string msg, object arg)
        {
            if (this.msg == msg)
            {
                hasTriggered = true;
                triggerMsg = msg;
                triggerArg = arg;
                triggerTime = Time.time;
                triggerFrameCount = Time.frameCount;

                if (callback != null)
                {
                    callback(arg);
                }

                blocked = false;
            }
            else
            {
                base.OnMessage(msg, arg);
            }
        }
    }


    public sealed class Listener : RoutineBlock
    {
        public bool finishOnTrigger { get; set; }
        public bool hasTriggered { get; private set; }
        public string triggerMsg { get; private set; }
        public object triggerArg { get; private set; }
        public float triggerTime { get; private set; }
        public int triggerFrameCount { get; private set; }

        public float timeSinceTrigger
        {
            get { return hasTriggered ? Time.time - triggerTime : -1f; }
        }

        public float frameCountSinceTrigger
        {
            get { return hasTriggered ? Time.frameCount - triggerFrameCount : -1; }
        }

        private Dictionary<string, Action<object>> callbacks = new Dictionary<string, Action<object>>();
        private bool blocked = true;

        public Listener(bool finishOnTrigger)
        {
            this.finishOnTrigger = finishOnTrigger;
            ClearLastTrigger();
        }

        public Listener(string msg, Action<object> onTrigger, bool finishOnTrigger)
        {
            this.finishOnTrigger = finishOnTrigger;
            ClearLastTrigger();
            Listen(msg, onTrigger);
        }

        public void ClearLastTrigger()
        {
            this.hasTriggered = false;
            this.triggerMsg = null;
            this.triggerArg = null;
            this.triggerTime = -1f;
            this.triggerFrameCount = -1;
        }

        public void Listen(string msg, Action<object> onTrigger)
        {
            Action<object> callback = null;

            if (callbacks.TryGetValue(msg, out callback))
            {
                if (callback != null)
                {
                    callback += onTrigger;
                }
                else
                {
                    callback = onTrigger;
                }

                callbacks[msg] = callback;
            }
            else
            {
                callbacks.Add(msg, onTrigger);
            }
        }

        public void ClearListen()
        {
            callbacks.Clear();
        }

        protected override bool OnBlock()
        {
            return blocked;
        }

        protected override void OnMessage(string msg, object arg)
        {
            Action<object> callback = null;

            if (callbacks.TryGetValue(msg, out callback))
            {
                hasTriggered = true;
                triggerMsg = msg;
                triggerArg = arg;
                triggerTime = Time.time;
                triggerFrameCount = Time.frameCount;

                if (callback != null)
                {
                    callback(arg);
                }

                if (finishOnTrigger)
                {
                    blocked = false;
                }
            }
            else
            {
                base.OnMessage(msg, arg);
            }
        }
    }
}