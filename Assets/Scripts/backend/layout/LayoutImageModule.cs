using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace snorri
{
    public class LayoutImageModule : Module
    {
        Image image;

        int refreshImageState = 0;
        bool hasSprite = false;

        protected override void AddClasses()
        {
            image = ComponentCheck<Image>();
        }
        protected override void Setup()
        {
            base.Setup();

            Configure();
        }
        protected override void Launch() {
            base.Launch();
            
        }

        public void SetSprite(Sprite s) {
            image.sprite = s;
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

            if (!hasSprite) return;

            return;

            if (refreshImageState == 0) {

                image.sprite = null;

                refreshImageState++;
            } else if (refreshImageState == 1) {

                string spriteName = Vars.Get<string>("sprite", "");
                if (spriteName != "") {
                    string imageName = Vars.Get<string>("image", "");
                    Sprite sprite = RESOURCES.GetSprite(imageName, spriteName);
                    if (sprite != null)
                    {
                        image.sprite = sprite;
                    }
                }

                refreshImageState = 0;;
            }
        }

        void Configure()
        {
            
            string spriteName = Vars.Get<string>("sprite", "");
            if (spriteName != "") {
                string imageName = Vars.Get<string>("image", "");
                Sprite sprite = RESOURCES.GetSprite(imageName, spriteName);
                if (sprite != null) image.sprite = sprite;

                image.pixelsPerUnitMultiplier = Vars.Get<float>("ppu_multiplier", 2.0f);
                switch (Vars.Get<string>("type", "sliced")) {
                    case "sliced":
                        image.type = Image.Type.Sliced;
                        break;
                    case "simple":
                        image.type = Image.Type.Simple;
                        break;
                }
                image.fillCenter = Vars.Get<bool>("fill_center", true);
                
                hasSprite = true;
            } else {
                // setup texture if possible
                string textureName = Vars.Get<string>("texture", "");
                string textureType = Vars.Get<string>("texture_type", "");
                Bag<float> pivot = Vars.Get<Bag<float>>("texture_pivot", new Bag<float>(0.5f, 0.5f));
                Vec pivotVec = new Vec(pivot);

                if (textureName != "")
                {
                    Texture2D tex = RESOURCES.GetTexture(textureName, textureType);
                    if (tex != null)
                    {
                        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivotVec.vec2, 100.0f);
                        image.sprite = sprite;
                    }
                }
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