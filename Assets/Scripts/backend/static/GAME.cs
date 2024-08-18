
using UnityEngine;
namespace snorri
{
    public static class GAME
    {
        public static Map Vars {get; set;}

        public static float TimeDayLength {
            get {
                return Vars.Get<float>("time_day_length", 360f);
            }
        }

        public static void Init()
        {
            Vars = Map.FromJson("game");

            LoadDefaults();

            LoadCheck();

            Context = "loading";

            UnityEngine.Physics.gravity = new Vector3(0f, Vars.Get<float>("gravity", -15f), 0f);
            UnityEngine.Application.targetFrameRate = 60;
        }
        private static void LoadDefaults() {
            Vars.Set<string>("fitbit:config:folder_path_config", JSON.FilePathSave + "data/");
            Vars.Set<string>("fitbit:config:folder_path_data", JSON.FilePathSave + "data/");
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
        public static string State
        {
            get
            {
                if (Vars == null)
                    return "";

                return Vars.Get<string>("state_current", "");   
            }
            set
            {
                Vars.Set<string>("state_current", value);
                new Trigger(Trigger.WhenStateChange);
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
        public static Map Locations
        {
            get {
                if (Vars == null)
                    return null;

                return Vars.Get<Map>("locations", new Map());
            }
        }
        public static string Layout {
            get {
                if (Vars == null) return "";

                return Vars.Get<string>("layout_state", "basic");
            }
        }
        public static Node GetLocation(string name)
        {
            Map locationMap = Locations;
            return locationMap.Get<Node>(name, null);
        }

        public static void LoadCheck(bool isCompress = false) {
            // checks for existing save files
            foreach (var kvp in Vars.Get<Map>("saves", new Map()).Elements) {
                Map m = Map.FromJsonSave(kvp.Value.ToString(), isCompress);
                if (m == null) m = new Map();

                //Vars.Set<Map>(kvp.Key, m);
                Map mCurrent = Vars.Get<Map>(kvp.Key, new Map());
                mCurrent.Sync(m);
                Vars.Set<Map>(kvp.Key, mCurrent);
            }

            LOG.Console("### GAME LOAD ###");
            Vars.Log();
        }
        public static void SaveCheck(bool isCompress = false) {
            // determines what to save
            foreach (var kvp in Vars.Get<Map>("saves", new Map()).Elements) {
                Map m = Vars.Get<Map>(kvp.Key, new Map());
                m.Write(kvp.Value.ToString(), isCompress, false);
            }
        }

        public static void Notify(Map args) {
            Bag<Map> bagOfNotifications = Vars.Get<Bag<Map>>("notifications", new Bag<Map>());
            bagOfNotifications.Append(args);
            Vars.Set<Bag<Map>>("notifications", bagOfNotifications);
        }
        public static Bag<Map> Notifications {
            get {
                return Vars.Get<Bag<Map>>("notifications", new Bag<Map>());
            }
            set {
                Vars.Set<Bag<Map>>("notifications", value);
            }
        }
    }
}