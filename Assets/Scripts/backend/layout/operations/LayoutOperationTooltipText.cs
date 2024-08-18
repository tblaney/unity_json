using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationTooltipText : Operation
    {
        protected override void Setup()
        {
            base.Setup();
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            Cursor cursor = GAME.Vars.Get<Cursor>("cursor", null);
            if (cursor == null) return;

            Map tooltipMap = new Map();

            if (args.Get<bool>("is_hover_in", false)) {
                tooltipMap.Set<string>("text", Vars.Get<string>("text", "this is a atestYYY!"));
                cursor.Tooltip = tooltipMap;
            } else {
                tooltipMap.Set<string>("text", "");
                cursor.Tooltip = tooltipMap;
            }
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}