using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    // starts a dropdown

    public class LayoutOperationDropdown : Operation
    {
        string gameVar;
        Bag<string> options;

        Node dropdownNode;

        protected override void Setup()
        {
            base.Setup();

            gameVar = Vars.Get<string>("game_var", "");
            options = Vars.Get<Bag<string>>("options", new Bag<string>());
            if (Vars.Has("options_game_var")) {
                options = GAME.Vars.Get<Bag<string>>(Vars.Get<string>("options_game_var"));
            }
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            if (args == null)
                args = new Map();

            bool val = false;
            if (args.Has("is_click_in")) {
                val = args.Get<bool>("is_click_in", true);
            }

            if (val) {
                // need to open up a new dropdown
                Map m = new Map();
                m.Set<string>("inherit_from", "layout_dropdown");
                m.Set<string>("children:dropdown_rect:actors:layout_dropdown:game_var", this.gameVar);
                m.Set<Bag<string>>("children:dropdown_rect:actors:layout_dropdown:options", this.options);
                dropdownNode = this.Node.AddChild("dropdown", m);
            } else {
                Map delayMap = new Map();
                delayMap.Set<Task>("task", new Task(
                    ()=> {
                        if (dropdownNode != null) {
                            dropdownNode.Terminate();
                        }
                    }
                ));
                delayMap.Set<float>("time_delay", 0.1f);
                this.Node.Execute("delay", delayMap);
            }
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}