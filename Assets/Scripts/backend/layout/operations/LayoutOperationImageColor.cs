using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationImageColor : Operation
    {
        LayoutImageModule image;

        protected override void Setup()
        {
            base.Setup();

            image = Node.GetActor<LayoutImageModule>();
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            if (args == null) return;

            Task<Vec> taskCallback = new Task<Vec>(
                (Vec f) =>
                {
                    image.Color = f.color;
                }
            );

            Map argsLerp = new Map();

            Color colorIn = UTIL.GetColorFromHex(Vars.Get<string>("color_in", ""));
            Color colorOut = UTIL.GetColorFromHex(Vars.Get<string>("color_out", ""));
        
            Vec vecIn = new Vec(colorIn);
            Vec vecOut = new Vec(colorOut);

            string lerpType = "vec";
            bool val = args.Get<bool>("is_fade_in", true);

            argsLerp.Set<string>("lerp_type", lerpType);
            argsLerp.Set<Vec>("val_in", new Vec(image.Color));
            if (val)
                argsLerp.Set<Vec>("val_out", vecIn);
            else
                argsLerp.Set<Vec>("val_out", vecOut);

            argsLerp.Set<float>("time", Vars.Get<float>("time", 0.5f));
            argsLerp.Set<Task<Vec>>("task_when_callback", taskCallback);

            Task taskWhenExecute = args.Get<Task>("task_when_end", null);
            if (taskWhenExecute != null)
                argsLerp.Set<Task>("task_when_end", taskWhenExecute);

            routine = Node.Execute<Vec>("lerp", argsLerp);
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}