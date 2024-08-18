using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
namespace snorri
{
    public class LayoutTextModule : Module
    {
        TextMeshProUGUI text;

        public string Text {
            get { return text.text; }
            set { text.text = value; }
        }
        public Color Color {
            get { return text.color; }
            set { text.color = value; }
        }

        protected override void AddClasses()
        {
            base.AddClasses();
            text = ComponentCheck<TextMeshProUGUI>();
        }
        protected override void Setup()
        {
            base.Setup();

            LOG.Console("### layout text module ###");

            Vars.Log();

            TMP_FontAsset fontAsset = RESOURCES.Load<TMP_FontAsset>("fonts/" + Vars.Get<string>("font_name", "comic_land"));
            text.font = fontAsset;

            text.text = Vars.Get<string>("text", "no text found");
            if (!Vars.Has("size")) {
                LOG.Console("layout text module doesnt have size: " + this.Node.Name);
            }
            text.fontSize = Vars.Get<int>("size", 12);

            switch (Vars.Get<string>("alignment", "center"))
            {
                case "center":
                    text.alignment = TextAlignmentOptions.Center;
                    break;
                case "left":
                    text.alignment = TextAlignmentOptions.Left;
                    break;
                case "right":
                    text.alignment = TextAlignmentOptions.Right;
                    break;
                case "justified":
                    text.alignment = TextAlignmentOptions.Justified;
                    break;
                default:
                    text.alignment = TextAlignmentOptions.Left;
                    break;
            }

            text.raycastTarget = Vars.Get<bool>("raycasts", false);

            if (Vars.Has("color")) {
                text.color = UTIL.GetColorFromHex(Vars.Get<string>("color"));
            }

            text.enableAutoSizing = Vars.Get<bool>("is_auto_size", false);
            if (text.enableAutoSizing) {
                text.fontSizeMin = Vars.Get<int>("text_size_min", 14);
                text.fontSizeMax = Vars.Get<int>("text_size_min", 22);
            }

            switch (Vars.Get<string>("overflow", "truncate")) {
                case "truncate":
                
                    text.overflowMode = TextOverflowModes.Truncate;

                    break;
            }
        }
        protected override void Launch() 
        {
            base.Launch();
        }
    }
}