using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
namespace snorri
{
    public class LayoutTextBox : Ticker
    {
        LayoutTextModule module;

        protected override void Setup()
        {
            base.Setup();
        }
        protected override void Launch() 
        {
            base.Launch();

            module = Node.GetBagOChildActors<LayoutTextModule>()[0];
        }

        public override void Tick() {
            base.Tick();

        }
    }
}