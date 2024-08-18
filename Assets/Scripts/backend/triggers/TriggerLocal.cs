using UnityEngine;
using System;

namespace snorri
{
    [System.Serializable]
    public class TriggerLocal
    {
        public event EventHandler<Map> e;
        public TriggerLocal() {

        }
        public void Trigger(string name, Map args) {
            args.Name = name;
            e?.Invoke(null, args);
        }
    }
}