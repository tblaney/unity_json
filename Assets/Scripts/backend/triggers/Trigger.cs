using UnityEngine;
using System;

namespace snorri
{
    [System.Serializable]
    public class Trigger
    {
        public static event EventHandler<Map> e;

        public Trigger(string name)
        {
            Map m = new Map(name);
            e?.Invoke(null, m);
        }
        public Trigger(string name, Map elem)
        {
            elem.Name = name;
            e?.Invoke(null, elem);
        }

             // triggers
        public static readonly string WhenStageChange = "trigger_stage_change";
        public static readonly string WhenContextChange = "trigger_context_change";
        public static readonly string WhenStateChange = "trigger_state_change";
        public static readonly string WhenSave = "trigger_save";
        public static readonly string WhenCellUpdate = "trigger_cell_update";
    }
}