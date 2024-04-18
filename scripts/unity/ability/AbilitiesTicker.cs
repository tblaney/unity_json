namespace snorri
{
    public class AbilitiesTicker : Abilities
    {
        public override void Tick()
        {
            base.Tick();

            if (Current == null)
            {
                Choose();
                return;
            } else
            {
                Current.TickAbility();

                Choose();
            }
        }
        public override void TickPhysics()
        {
            base.TickPhysics();

            if (Current != null)
                Current.TickPhysicsAbility();
        }
    }
}