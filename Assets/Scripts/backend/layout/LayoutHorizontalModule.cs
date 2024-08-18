using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace snorri
{
    using UnityEngine.UI;

    public class LayoutHorizontalModule : Module
    {
        HorizontalLayoutGroup group;

        protected override void AddClasses()
        {
            group = ComponentCheck<HorizontalLayoutGroup>();
        }
        protected override void Setup()
        {
            base.Setup();

            Configure();
        }

        protected override void WhenFirstFrame() {
            base.WhenFirstFrame();
            group.enabled = true;
        }
        protected override void Launch() {
            base.Launch();
            group.enabled = false;
        }

        void Configure()
        {
            group.spacing = Vars.Get<float>("spacing", 0f);
            switch (Vars.Get<string>("alignment", "")) {
                case "":
                    break;
                case "upper_left":
                    group.childAlignment = TextAnchor.UpperLeft;
                    break;
                case "upper_center":
                    group.childAlignment = TextAnchor.UpperCenter;
                    break;
                case "upper_right":
                    group.childAlignment = TextAnchor.UpperRight;
                    break;
                case "middle_left":
                    group.childAlignment = TextAnchor.MiddleLeft;
                    break;
                case "middle_center":
                    group.childAlignment = TextAnchor.MiddleCenter;
                    break;
                case "middle_right":
                    group.childAlignment = TextAnchor.MiddleRight;
                    break;
                case "lower_left":
                    group.childAlignment = TextAnchor.LowerLeft;
                    break;
                case "lower_center":
                    group.childAlignment = TextAnchor.LowerCenter;
                    break;
                case "lower_right":
                    group.childAlignment = TextAnchor.LowerRight;
                    break;
            }

            group.reverseArrangement = Vars.Get<bool>("reverse", false);
            Bag<int> paddings = Vars.Get<Bag<int>>("paddings", new Bag<int>(0,0,0,0));
            group.padding = new RectOffset(paddings[0], paddings[1], paddings[2], paddings[3]); // l,r,t,b
            
            
            group.childControlWidth = Vars.Get<bool>("child_control_width", false);
            group.childControlHeight = Vars.Get<bool>("child_control_height", false);
            group.childScaleWidth = Vars.Get<bool>("child_scale_width", false);
            group.childScaleHeight = Vars.Get<bool>("child_scale_height", false);
            group.childForceExpandWidth = Vars.Get<bool>("child_force_expand_width", false);
            group.childForceExpandHeight = Vars.Get<bool>("child_force_expand_height", false);
        }
    }
}