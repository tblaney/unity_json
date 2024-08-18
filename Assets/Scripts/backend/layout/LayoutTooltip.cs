namespace snorri
{
    public class LayoutTooltip : Ticker
    {
        Cursor cursor;
        bool isVisible = true;

        LayoutTextModule text;
        LayoutTextModule textAlt;

        Node container;

        float offset = 50f;

        protected override void Setup()
        {
            base.Setup();
            isVisible = true;

            offset = Vars.Get<float>("offset", 50.0f);
        }
        protected override void Launch() {
            base.Launch();

            Bag<LayoutTextModule> mods = this.Node.GetBagOChildActors<LayoutTextModule>();
            text = mods[0];
            if (mods.Length > 1)
                textAlt = mods[1];

            bool hasContainer = this.Node.FindChild("tooltip_container", out container);
        }
        public override void Tick() {
            base.Tick();

            if (cursor == null) {
                cursor = GAME.Vars.Get<Cursor>("cursor");
                return;
            }

            if (!cursor.Visible) {
                if (isVisible) {
                    Map args = new Map();
                    args.Set<bool>("is_fade_in", false);
                    Node.ExecuteOperation("fade", args);
                    isVisible = false;
                }
                return;
            }

            if (!isVisible) {
                // fade in
                Map args = new Map();
                args.Set<bool>("is_fade_in", true);
                Node.ExecuteOperation("fade", args);
                isVisible = true;
            }

            Vec mousePos = GAME.Vars.Get<Vec>("input:mouse_position", new Vec(0,0));
            int mouseQuadrant = GAME.Vars.Get<int>("input:mouse_quadrant", 0);

            LOG.Console("mouse position: " + mousePos.vec2.ToString());

            mousePos.x = (mousePos.x / (float)UnityEngine.Screen.width)*1920;
            mousePos.y = (mousePos.y / (float)UnityEngine.Screen.height)*1080;

            if (container != null) {
                Vec pLPos = new Vec(0,0);
                Vec pivot = new Vec(0,0);
                Vec anchor = new Vec(0,0);
                switch (mouseQuadrant) {
                    case 0:
                        pLPos = new Vec(offset, -offset);
                        pivot = new Vec(0, 1);
                        anchor = new Vec(0, 1);
                        break;
                    case 1:
                        pLPos = new Vec(-offset, -offset);
                        pivot = new Vec(1, 1);
                        anchor = new Vec(1, 1);
                        break;
                    case 2:
                        pLPos = new Vec(-offset, offset);
                        pivot = new Vec(1, 0);
                        anchor = new Vec(1, 0);
                        break;
                    case 3:
                        pLPos = new Vec(offset, offset);
                        pivot = new Vec(0, 0);
                        anchor = new Vec(0, 0);
                        break;
                }

                PointLayout pL = (container.Point as PointLayout);
                pL.Position = pLPos;
                pL.Anchor = anchor;
                pL.Pivot = pivot;
            }

            // now we need to  put our position there
            this.Node.Point.Position = mousePos;

            text.Text = cursor.Tooltip.Get<string>("text", "");
            if (textAlt != null)
                textAlt.Text = "";
        }
    }
}