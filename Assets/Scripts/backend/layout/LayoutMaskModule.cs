namespace snorri
{
    using UnityEngine;
    using UnityEngine.UI;
    public class LayoutMaskModule : Module {
        RectMask2D mask;

        protected override void AddClasses() {
            base.AddClasses();
            mask = this.ComponentCheck<RectMask2D>();
        }
        protected override void Setup() {
            base.Setup();
        }
        protected override void Launch() {
            base.Launch();
        }
    }
}