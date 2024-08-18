using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace snorri
{
    public class LayoutRawImageModule : Module
    {
        RawImage image;

        protected override void AddClasses()
        {
            image = ComponentCheck<RawImage>();
        }
        protected override void Setup()
        {
            base.Setup();

            Configure();
        }
        protected override void Launch() {
            base.Launch();
            
        }
        public Color Color {
            get { 
                return image.color;
            }
            set {
                image.color = value;
            }
        }

        public override void Tick() {
            base.Tick();
        }

        void Configure()
        {
            if (Vars.Has("texture")) {
                string textureName = Vars.Get<string>("texture", "");
                Texture2D tex = RESOURCES.GetTexture(textureName);
                image.texture = tex;
            }
            
            image.raycastTarget = Vars.Get<bool>("raycasts", false);

            string colorHex = Vars.Get<string>("color", "#000000");
            Color colorHexU;
            if (ColorUtility.TryParseHtmlString(colorHex, out colorHexU))
            {
                colorHexU.a = Vars.Get<float>("alpha", 1f);
                image.color = colorHexU;
            }
        }
    }
}