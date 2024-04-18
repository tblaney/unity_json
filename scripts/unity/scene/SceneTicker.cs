namespace snorri
{
    public class SceneTicker : Ticker
    {
        [ReadOnly]
        public bool IsCompleted;

        protected Bag<ScenePass> passes;

        protected override void Setup()
        {
            base.Setup();

            IsCompleted = false;

            passes = Node.GetBagOActors<ScenePass>();
        }
        protected override void Launch()
        {
            base.Launch();

            GAME.Context = "loading";
        }

        public override void Tick()
        {
            if (IsCompleted)
                return;

            bool IsComplete = true;
            foreach (ScenePass pass in passes)
            {
                if (!pass.IsExecuted)
                {
                    IsComplete = false;
                    
                    // need to execute
                    if (pass.CanRun())
                    {
                        pass.Execute();
                    }
                    continue;
                }
                if (!pass.IsComplete)
                {
                    IsComplete = false;
                }
            }

            if (IsComplete)
            {
                Map delayMap = new Map();
                delayMap.Set<Task>("task", new Task(Complete));
                delayMap.Set<float>("time_delay", 1f);
                Node.Execute("delay", delayMap);

                IsCompleted = true;
            }
        }
        public override void TickPhysics()
        {

        }

        public void AddPass(ScenePass p)
        {
            passes.Append(p);
        }

        protected virtual void Complete()
        {
            GAME.Context = "run";

            IsCompleted = true;
        }
    }
}