using UnityEngine;

namespace snorri
{
    public abstract class ColliderModule : Module
    {
        protected Collider collider;

        protected override void AddClasses()
        {
            base.AddClasses();
        }
        protected override void Setup()
        {
            base.Setup();

            string pmName = Vars.Get<string>("material", "pm_smooth");
            PhysicMaterial mat = RESOURCES.Load<PhysicMaterial>(pmName);
            this.collider.material = mat;
        }
    }
}