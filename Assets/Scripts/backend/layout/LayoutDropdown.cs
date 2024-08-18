namespace snorri
{
    using UnityEngine;

    public class LayoutDropdown : Actor {
        Bag<string> options;
        string gameVar;

        int id = 0;

        protected override void Setup() {
            base.Setup();

            options = Vars.Get<Bag<string>>("options", new Bag<string>());
            gameVar = Vars.Get<string>("game_var", "fitbit:config:activity");
        }
        protected override void Launch() {
            base.Launch();

            // need to spawn in
            Generate();
        }
        void Generate() {
            LOG.Console("layout dropdown generate! " + gameVar + ", " + options.Length.ToString());

            string current = GAME.Vars.Get<string>(gameVar, "");
            id = 0;
            if (current != "") {
                int i = 0;
                foreach (string o in options) {
                    if (o == current) {
                        id = i;
                        break;
                    }
                    i++;
                }
            }

            int k = 0;
            foreach (string o in options) {
                if (k == id) {
                    k++;
                    continue;
                }
                Map nodeMap = GetNodeMap(o);
                this.Node.AddChild("dropdown_item_"+k.ToString(), nodeMap);
                k++;
            }
        }
        Map GetNodeMap(string option) {
            Map m = Vars.Get<Map>("node_map", new Map());
            m.Set<string>("actors:layout_operation_string_game_var:game_var", gameVar);
            m.Set<string>("actors:layout_operation_string_game_var:value", option);
            m.Set<string>("children:text:actors:layout_text_module:text", option);
            return m;
        }
        void Clear() {
            this.Node.Terminate();
        }
    }
}