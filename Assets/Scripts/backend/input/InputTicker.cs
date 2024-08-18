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

            GAME.Vars.Set<Vec>("input:mouse_delta", new Vec(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
            GAME.Vars.Set<Vec>("input:direction", new Vec(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
            GAME.Vars.Set<Vec>("input:mouse_position", new Vec(Input.mousePosition));
            GAME.Vars.Set<float>("input:mouse_scroll", Input.mouseScrollDelta.y);
            GAME.Vars.Set<int>("input:mouse_quadrant", GetMouseQuadrant(Input.mousePosition));
        }

        private int GetMouseQuadrant(Vector3 mousePosition)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            if (mousePosition.x < screenWidth / 2 && mousePosition.y > screenHeight / 2)
                return 0; // Top Left
            else if (mousePosition.x >= screenWidth / 2 && mousePosition.y > screenHeight / 2)
                return 1; // Top Right
            else if (mousePosition.x >= screenWidth / 2 && mousePosition.y <= screenHeight / 2)
                return 2; // Bottom Right
            else
                return 3; // Bottom Left
        }
    }
}