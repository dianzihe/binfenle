using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Utilities.Routines
{
    public class RoutineComponent : MonoBehaviour
    {
        private List<IRoutine> main = new List<IRoutine>();
        private List<IRoutine> buffer = new List<IRoutine>();

        public void StartRoutine(IRoutine r)
        {
            if (!this.gameObject.activeInHierarchy || !this.enabled)
            {
                Debug.LogError("Can't start routine in an inactive gameObject in hierarchy or disabled RoutineComponent.");
                return;
            }

            if (r.hasStarted)
            {
                Debug.LogError("Can't start routine because it has already started.");
                return;
            }

            if (r.MoveNext())
            {
                buffer.Add(r);
            }
        }

        public void BreakAllRoutines()
        {
            foreach (var r in main)
            {
                if (r != null)
                {
                    r.Break();
                }
            }

            foreach (var r in buffer)
            {
                if (r != null)
                {
                    r.Break();
                }
            }
        }

        public void Broadcast(string msg, object arg)
        {
            foreach (var r in main)
            {
                if (r != null)
                {
                    r.SendMessage(msg, arg);
                }
            }

            foreach (var r in buffer)
            {
                if (r != null)
                {
                    r.SendMessage(msg, arg);
                }
            }
        }

        private void LateUpdate()
        {
            foreach (var r in buffer)
            {
                if (r != null)
                {
                    main.Add(r);
                }
            }

            buffer.Clear();

            int count = main.Count;

            for (int i = 0; i < count; ++i)
            {
                if (main[i] != null && !main[i].MoveNext())
                {
                    main[i] = null;
                }
            }

            main.RemoveAll(r => r == null);
        }

        private void OnDisable()
        {
            BreakAllRoutines();
        }
    }
}