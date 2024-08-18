using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class PointOperation : Operation
    {
        /*
            VARS:
                --> point_state_in
                --> point_state_out
                --> time
        */

        PointState stateIn;
        PointState stateOut;

        protected override void Setup()
        {
            base.Setup();

            stateIn = new PointState(Vars.Get<Map>("point_state_in", new Map()));
            stateOut = new PointState(Vars.Get<Map>("point_state_out", new Map()));
        }

        public override void Execute(Map args = null)
        {
            base.Execute(args);

            if (args == null)
                args = new Map();

            Task<PointState> taskCallback = new Task<PointState>(
                (PointState p) =>
                {
                    Node.Point.State = p;
                }
            );

            Map argsLerp = new Map();

            bool val = args.Get<bool>("is_fade_in", true);

            argsLerp.Set<string>("lerp_type", "point");
            argsLerp.Set<PointState>("val_in", Node.Point.State);
            if (val)
                argsLerp.Set<PointState>("val_out", stateIn);
            else
                argsLerp.Set<PointState>("val_out", stateOut);

            argsLerp.Set<float>("time", Vars.Get<float>("time", 0.5f));
            argsLerp.Set<Task<PointState>>("task_when_callback", taskCallback);
            //argsLerp.Set<Task>("task_when_end", taskWhenExecute);

            Node.Execute<PointState>("lerp", argsLerp);
        }
    }
}