namespace snorri
{
    public class TimedOperation : Actor
    {
        float time;
        string opName;
        Map args;
        protected override void Setup() {
            base.Setup();

            time = Vars.Get<float>("time_delay", 10f);
            opName = Vars.Get<string>("operation_name", "");
            args = Vars.Get<Map>("args", new Map());
        }
        protected override void Launch() {
            base.Launch();

            Map delayMap = new Map();
            delayMap.Set<Task>("task", new Task(
                ()=> {
                    this.Node.ExecuteOperation(opName, args);
                }
            ));
            delayMap.Set<float>("time_delay", time);
            this.Node.Execute("delay", delayMap);
        }
    }
}