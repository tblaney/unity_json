using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationGameState : Operation
    {
        string gameStateIn;

        protected override void Setup()
        {
            base.Setup();

            gameStateIn = Vars.Get<string>("game_state", "step_01");
        }
        protected override void Launch() {
            base.Launch();
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();
            if (args == null) return;
            bool val = args.Get<bool>("is_click_in", false);
            if (!val) return;
            if (GAME.State != gameStateIn)
                GAME.State = gameStateIn;
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}