namespace snorri
{
    public class LayoutTextGameVar : Actor {
        LayoutTextModule module;
        string gameVar;
        TriggerListener listener;
        protected override void Setup() {
            base.Setup();

            gameVar = Vars.Get<string>("game_var", "");
            module = this.Node.GetActor<LayoutTextModule>();

            listener = new TriggerListener(GAME.Vars.Trigger);
            listener.Listen("change", new Task(TriggerCallback));
        }
        protected override void Launch() {
            base.Launch();
            module.Text = GAME.Vars.Get<string>(gameVar, "");
        }

        void TriggerCallback()
        {   
            module.Text = GAME.Vars.Get<string>(gameVar, "");
        }
    }
}