using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace snorri
{
    public class LayoutActor : Actor
    {
        bool isActivateOnLaunch;

        protected override void Setup()
        {
            base.Setup();

            isActivateOnLaunch = Vars.Get<bool>("is_activate_on_launch", false);
        }
        protected override void Launch() {
            base.Launch();
             
            if (isActivateOnLaunch) {
                Activate(true);
            }
        }

        public virtual void Activate(bool isActive) {
            this.IsActive = isActive;

            WhenActivate();
        }
        protected virtual void WhenActivate() {
            Map args = new Map();
            if (IsActive) {
                args.Set<bool>("is_fade_in", true);
            } else {
                args.Set<bool>("is_fade_in", false);
                args.Set<Task>("task_when_end", new Task(Node.Terminate));
            }

            Node.ExecuteOperation("fade", args);
        }


    }
}