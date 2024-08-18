namespace snorri
{
    public class AbilityStateOperation : Ability
    {
        protected override void Setup() {
            base.Setup();

            if (Vars.Get<bool>("is_deactivate", true)) {
                this.Node.Enable(false);
            }
        }
        public override bool CanRun()
        {
            return GAME.State == Vars.Get<string>("state", "loading");
        }
        public override void TickAbility() {
            base.TickAbility();

            if (!CanRun()){
                Stop();
            }
        }
        public override void Begin() 
        { 
            base.Begin();

            this.Node.Enable(true);

            string operationName = Vars.Get<string>("operation_name", "");
            Map args = new Map();
            args.Set<bool>("is_fade_in", true);
            if (operationName != "")
                Node.ExecuteOperation(operationName, args);

            LOG.Console("ability state operation occured with state: " + GAME.State);
        }
        public override void End() 
        { 
            base.End();
            string operationName = Vars.Get<string>("operation_name", "");
            Map args = new Map();
            args.Set<bool>("is_fade_in", false);
            if (Vars.Get<bool>("is_deactivate", true)) {
                args.Set<Task>("task_when_end", new Task(
                    () => {
                        this.Node.Enable(false);
                    }
                ));
            }
            if (operationName != "")
                Node.ExecuteOperation(operationName, args);

            LOG.Console("ability state operation stopped with state: " + GAME.State);
        }
    }
}