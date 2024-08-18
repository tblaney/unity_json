namespace snorri
{
    public class LayoutInput : Ticker {
        InputActor input;

        protected override void Setup() {
            base.Setup();
        }
        protected override void Launch() {
            base.Launch();
            
            input = Node.GetActor<InputActor>();

            LOG.Console("layout input launch!");
            GAME.Vars.Log();
        }
        public override void Tick() {
            base.Tick();

            if (input.IsClicked("tab")) {

                LayoutButtonModule buttonPrevious = GAME.Vars.Get<LayoutButtonModule>("layout:active_button", null);
                LayoutButtonModule buttonPreviousClicked = GAME.Vars.Get<LayoutButtonModule>("layout:clicked_button", null);

                // go to next input field
                LayoutButtonModule moduleCurrent = GAME.Vars.Get<LayoutButtonModule>("layout:clicked_button", null);
                if (moduleCurrent == null) return;

                int idCurrent = moduleCurrent.ID;

                ClearButtons(buttonPrevious, buttonPreviousClicked);

                int idNew = idCurrent + 1;

                LayoutButtonModule moduleNew = GAME.Vars.Get<LayoutButtonModule>($"layout:buttons:{idNew}", null);
                if (moduleNew == null) {
                    moduleNew = GAME.Vars.Get<LayoutButtonModule>($"layout:buttons:{0}", null);
                } 

                if (moduleNew != null) {
                    moduleNew.InformState(LayoutState.HoverIn);
                    moduleNew.InformState(LayoutState.ClickIn);
                    moduleNew.IsSelected = true;
                    GAME.Vars.Set<LayoutButtonModule>("layout:clicked_button", moduleNew);
                }
            }
        }

        void ClearButtons(LayoutButtonModule buttonPrevious, LayoutButtonModule buttonPreviousClicked) {

            GAME.Vars.Set<LayoutButtonModule>("layout:active_button", null);
            if (buttonPrevious != null) {
                buttonPrevious.InformState(LayoutState.HoverOut);
            }

            if (buttonPreviousClicked != null) {
                buttonPreviousClicked.InformState(LayoutState.ClickOut);
                buttonPreviousClicked.IsSelected = false;
            }
            GAME.Vars.Set<LayoutButtonModule>("layout:clicked_button", null);
            
        }
    }
}