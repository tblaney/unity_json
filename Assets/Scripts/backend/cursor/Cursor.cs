namespace snorri
{
    using UnityEngine;
    public class Cursor : Ticker {
        Vector2 hotspot;
        bool isVisible;
        string cursorTexName;

        public bool Visible {
            get {
                return isVisible;
            }
            set  {
                isVisible = value;
            }
        }
        public Map Tooltip {
            get {
                return Vars.Get<Map>("tooltip", new Map());
            }
            set {
                Vars.Set<Map>("tooltip", value);
            }
        }

        protected override void Setup() {
            base.Setup();

            Bag<float> hotspotBag = Vars.Get<Bag<float>>("hotspot", new Bag<float>(0.25f, 0.25f));
            hotspot = new Vector2(
                hotspotBag[0], hotspotBag[1]
            );

            cursorTexName = Vars.Get<string>("cursor_name", "tex_ui_cursor_01");

            Visible = Vars.Get<bool>("is_visible", false);
        }
        protected override void Launch() {
            base.Launch();

            GAME.Vars.Set<Cursor>("cursor", this);

            Refresh();
        }
        void Refresh() {
            Texture2D tex = RESOURCES.GetTexture(cursorTexName);

            UnityEngine.Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
        }

        public override void Tick() {
            base.Tick();

           UnityEngine.Cursor.visible = isVisible;
        }
    }
}