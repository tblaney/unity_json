using UnityEngine;
namespace snorri
{
    public static class LOG 
    {
        private static Map vars;
        private static Map Vars {
            get {
                if (vars == null)
                    vars = Map.FromJson("settings");

                return vars;
            }
        }
        public static void Console(string name)
        {
            if (Vars.Get<bool>("is_log", true))
                Debug.Log(name);
        }
    }
}