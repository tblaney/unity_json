namespace snorri
{
    public class AbilitiesState : Abilities
    {
        TriggerListener trigger;

        protected override void Setup()
        {
            base.Setup();

            trigger = new TriggerListener();
            trigger.Listen(Trigger.WhenStateChange, new Task(TriggerCallback));
        }
        protected override void Launch()
        {
            base.Launch();
        }

        void TriggerCallback()
        {   
            LOG.Console("abilities state trigger callback!");
            // evaluate from scratch
            Ability ab = Node.Vars.Get<Ability>("ability_current", null);
            if (ab != null)
            {
                ab.End();
            }
            Node.Vars.Remove("ability_current");
            Choose();
        }
    }
}