using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationTextLerp : Operation
    {
        LayoutTextModule text;
        string textOut = "";
        bool isLooping = false;

        bool isRunning = false;

        TriggerListener listenerToContext;

        protected override void Setup()
        {
            base.Setup();

            text = Node.GetActor<LayoutTextModule>();
            textOut = Vars.Get<string>("text", "");
            isLooping = Vars.Get<bool>("is_looping", false);
        }
        protected override void Launch() {
            base.Launch();

            listenerToContext = new TriggerListener();
            listenerToContext.Listen(Trigger.WhenContextChange, new Task(WhenContextChange));
        }

        void WhenContextChange() {
            if (GAME.Context == Vars.Get<string>("context", "running")) {
                if (!isRunning) {
                    Map args = new Map();
                    args.Set<bool>("is_fade_in", true);
                    this.Node.ExecuteOperation(this.Name, args);
                }
            } else {
                isRunning = false;
            }
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            if (args == null)
                args = new Map();

            Task<string> taskCallback = new Task<string>(
                (string s) =>
                {
                    text.Text = s;
                }
            );

            bool isVal = args.Get<bool>("is_fade_in", false);

            if (isVal) {

                LOG.Console("layout operation text lerp! " + isVal.ToString());

                isRunning = true;

                Map argsLerp = new Map();
                argsLerp.Set<string>("lerp_type", "text");
                argsLerp.Set<string>("val_in", "");
                argsLerp.Set<string>("val_out", textOut);

                argsLerp.Set<float>("time", Vars.Get<float>("time", 0.5f));

                argsLerp.Set<Task<string>>("task_when_callback", taskCallback);

                Task taskWhenExecute = args.Get<Task>("task_when_end", null);
                if (isLooping) {
                    taskWhenExecute = new Task(
                        () => {
                            if (isRunning)
                                this.Node.ExecuteOperation(this.Name, args);
                        }
                    );
                    argsLerp.Set<Task>("task_when_end", taskWhenExecute);
                }

                routine = Node.Execute<string>("lerp", argsLerp);
            } else {
                isRunning = false;
            }
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}