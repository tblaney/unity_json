using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public abstract class Operation : Actor
    {
        protected Coroutine routine;

        protected override void Launch() {
            base.Launch();

            if (Vars.Get<bool>("is_execute_on_launch", false)) {
                Node.ExecuteOperation(this.Name, Vars.Get<Map>("launch_args", new Map()));
            }
        }

        protected override void WhenDisable()
        {
            base.WhenDisable();
            
            Stop();
        }

        public virtual void Execute(
           Map args = null)
        {
            Stop();
        }

        public void Stop()
        {
            if (routine != null)
                Node.Entity.StopCoroutine(routine);
            else 
                LOG.Console("operation stopped, no routine!");

            WhenStop();
        }
        protected virtual void WhenStop()
        {
            // base cleanup
        }
    }
}