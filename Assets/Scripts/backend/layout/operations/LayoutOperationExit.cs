namespace snorri
{
    using UnityEngine;
    public class LayoutOperationExit : Operation {
        protected override void Setup() {
            base.Setup();
        }
        public override void Execute(
            Map args = null)
        {
            base.Stop();

            Application.Quit();
        }
    }
}