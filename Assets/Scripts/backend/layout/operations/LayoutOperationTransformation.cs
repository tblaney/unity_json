using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationTransformation : Operation
    {
        PointState stateOut;
        PointState stateIn;

        bool isDestroyOnTransform = false;

        protected override void Setup()
        {
            base.Setup();
            isDestroyOnTransform = Vars.Get<bool>("is_destroy_on_transform", false);
            stateOut = new PointState(Vars.Get<Map>("state_out", this.Node.PointLayout.State.ToMap()));
            stateIn = new PointState(Vars.Get<Map>("state_in", this.Node.PointLayout.State.ToMap()));
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            if (args == null)
                args = new Map();

            Task<PointState> taskCallback = new Task<PointState>(
                (PointState p) =>
                {
                    Node.PointLayout.State = p;
                }
            );

            LOG.Console("layout operation transformation start! ");

            Map argsLerp = new Map();
            bool val = false;
            if (args.Has("is_fade_in")) {
                val = args.Get<bool>("is_fade_in", true);
            } else if (args.Has("is_click_in")) {
                val = args.Get<bool>("is_click_in", true);
            } else if (args.Has("is_hover_in")){ 
                val = args.Get<bool>("is_hover_in", true);
            }

            argsLerp.Set<string>("lerp_type", "point");
            argsLerp.Set<PointState>("val_in", Node.PointLayout.State);

            if (val)
                argsLerp.Set<PointState>("val_out", stateIn);
            else
                argsLerp.Set<PointState>("val_out", stateOut);

            argsLerp.Set<float>("time", Vars.Get<float>("time", 2.0f));
            argsLerp.Set<Task<PointState>>("task_when_callback", taskCallback);

            Task taskWhenExecute = args.Get<Task>("task_when_end", null);
            if (taskWhenExecute == null) {
                taskWhenExecute = new Task(() => {
                    if (isDestroyOnTransform) {
                        this.Node.ExecuteOperation("terminate", new Map());
                    }
                });
            }
            argsLerp.Set<Task>("task_when_end", taskWhenExecute);


            LOG.Console("layout operation transformation, " + val.ToString() + " - rotation: "
                + stateIn.Rotation.Text()
            );
            routine = Node.Execute<PointState>("lerp", argsLerp);
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}