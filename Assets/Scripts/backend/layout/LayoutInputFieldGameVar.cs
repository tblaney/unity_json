namespace snorri
{
    public class LayoutInputFieldGameVar : Actor {
        LayoutInputFieldModule module;
        TriggerListener trigger;
        string gameVar;
        protected override void Setup() {
            base.Setup();

            gameVar = Vars.Get<string>("game_var", "");

            module = this.Node.GetActor<LayoutInputFieldModule>();

            trigger = new TriggerListener(GAME.Vars.Trigger);
            trigger.Listen("change", new Task(TriggerCallback));
        }
        protected override void Launch() {
            base.Launch();
        }

        void TriggerCallback()
        {   
            //update input text
            string text = GAME.Vars.Get<string>(gameVar, "");
            //LOG.Console("layout input field game state trigger! " + text);
            module.Text = text;
        }
    }
}