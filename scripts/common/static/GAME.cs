
using UnityEngine;
namespace snorri
{
    public static class GAME
    {
        public static Map Vars {get; set;}

        public static void Init()
        {
            Vars = Map.FromJson("game");

            Context = "loading";

            UnityEngine.Physics.gravity = new Vector3(0f, Vars.Get<float>("gravity", -15f), 0f);
        }

        public static string Stage
        {
            get
            {
                if (Vars == null)
                    return "start";

                return Vars.Get<string>("stage_current", "start");   
            }
            set
            {
                Vars.Set<string>("stage_current", value);
                new Trigger(Trigger.WhenStageChange);
            }
        }
        public static string Context
        {
            get
            {
                if (Vars == null)
                    return "loading";

                return Vars.Get<string>("context_current", "loading");   
            }
            set
            {
                Vars.Set<string>("context_current", value);
                new Trigger(Trigger.WhenContextChange);
            }
        }
    }
}