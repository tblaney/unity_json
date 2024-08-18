using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationFitbit : Operation
    {

        protected override void Setup()
        {
            base.Setup();
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            if (args == null)
                args = new Map();

            bool val = false;
            if (args.Has("is_fade_in")) {
                val = args.Get<bool>("is_fade_in", true);
            } else if (args.Has("is_click_in")) {
                val = args.Get<bool>("is_click_in", true);
            } else if (args.Has("is_hover_in")){ 
                val = args.Get<bool>("is_hover_in", true);
            }

            if (!val) return;

            Map argsNew = new Map();
            argsNew.Set<bool>("is_hover_in", false);
            this.Node.ExecuteOperation("fade", argsNew);

            FitbitModule fitbit = null;
            switch (Vars.Get<string>("type", "authenticate")) {
                case "authenticate":
                    fitbit = GAME.Vars.Get<FitbitModule>("fitbit:module", null);
                    if (fitbit != null) {
                        fitbit.StartAuthorization();
                    }
                    break;
                case "get_codes":
                    fitbit = GAME.Vars.Get<FitbitModule>("fitbit:module", null);
                    if (fitbit != null) {
                        fitbit.StartGetCodes();
                    }
                    break;
                case "get_data":
                    fitbit = GAME.Vars.Get<FitbitModule>("fitbit:module", null);
                    if (fitbit != null) {
                        fitbit.StartGetData();
                    }
                    break;
            }
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}