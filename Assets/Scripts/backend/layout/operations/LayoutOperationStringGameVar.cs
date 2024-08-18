using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationStringGameVar : Operation
    {
        string gameVar;
        string gameVal;

        protected override void Setup()
        {
            base.Setup();

            gameVar = Vars.Get<string>("game_var", "");
            gameVal = Vars.Get<string>("value", "");
        }
        protected override void Launch() {
            base.Launch();
        }

        public override void Execute(
            Map args = null)
        {
            LOG.Console("layout operation string game var execute! 01");

            base.Stop();
            if (args == null) args = new Map();
            if (gameVar == "") return;
            bool val = args.Get<bool>("is_click_in", false);
            if (!val) return;

            LOG.Console("layout operation string game var execute! 02, " + gameVar + " - " + gameVal);

            GAME.Vars.Set<string>(gameVar, gameVal);
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}