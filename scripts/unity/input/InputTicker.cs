using UnityEngine;

namespace snorri
{
    public class InputTicker : Ticker
    {
        protected override void Setup()
        {
            base.Setup();

            ConfigureVars();
        }

        void ConfigureVars()
        {
            GAME.Vars.Set<Map>("input:keys", this.Vars.Get<Map>("keys", new Map()));

            GAME.Vars.Get<Map>("input:keys", new Map()).Log();
        }

        public override void Tick()
        {
            base.Tick();

            GAME.Vars.Set<Vec>("input:mouse_delta", new Vec(Input.GetAxis("mouse x"), Input.GetAxis("mouse y")));
            GAME.Vars.Set<Vec>("input:direction", new Vec(Input.GetAxis("horizontal"), Input.GetAxis("vertical")));
            GAME.Vars.Set<Vec>("input:mouse_position", new Vec(Input.mousePosition));
            GAME.Vars.Set<float>("input:mouse_scroll", Input.mouseScrollDelta.y);
        }
    }
}