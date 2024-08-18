namespace snorri
{
    public class AbilityContextOperation : Ability
    {
        public override bool CanRun()
        {
            return GAME.Context == Vars.Get<string>("context", "loading");
        }
        public override void Begin() 
        { 
            base.Begin();
            string operationName = Vars.Get<string>("operation_name", "");
            Map args = new Map();
            args.Set<bool>("is_fade_in", true);
            if (operationName != "")
                Node.ExecuteOperation(operationName, args);

            LOG.Console("ability context operation occured with context: " + GAME.Context);
        }
        public override void End() 
        { 
            base.End();
            string operationName = Vars.Get<string>("operation_name", "");
            Map args = new Map();
            args.Set<bool>("is_fade_in", false);
            if (operationName != "")
                Node.ExecuteOperation(operationName, args);

            LOG.Console("ability context operation stopped with context: " + GAME.Context);
        }
    }
}