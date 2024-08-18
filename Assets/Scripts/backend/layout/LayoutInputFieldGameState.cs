namespace snorri
{
    public class LayoutInputFieldGameState : Actor {
        LayoutInputFieldModule module;
        TriggerListener trigger;
        string gameVar;
        protected override void Setup() {
            base.Setup();

            gameVar = Vars.Get<string>("game_variable", "");

            module = this.Node.GetActor<LayoutInputFieldModule>();

            trigger = new TriggerListener();
            trigger.Listen(Trigger.WhenStateChange, new Task(TriggerCallback));
        }
        protected override void Launch() {
            base.Launch();
        }

        void TriggerCallback()
        {   
            //update input text
            string text = GAME.Vars.Get<string>(gameVar, "");
            LOG.Console("layout input field game state trigger! " + text);
            module.Text = text;
        }
    }
}