namespace snorri
{
    public class TriggerListener
    {
        Bag<Element> callbacks;
        public TriggerListener()
        {
            callbacks = new Bag<Element>();
            Trigger.e += WhenTrigger;
        }
        public TriggerListener(TriggerLocal triggerLocal) {
            callbacks = new Bag<Element>();
            triggerLocal.e += WhenTrigger;
        }
        void WhenTrigger(object sender, Map args)
        {
            Execute(args.Name, args);
        }
        public void Execute(string name, Map args) {
            foreach (Element e in callbacks)
            {
                // LOG.Console("trigger actor when trigger consider elem: " + e.Name);

                if (e.Name == args.Name)
                {
                    // match
                    Task taskBase = e as Task;
                    if (taskBase != null)
                    {
                        taskBase.Execute();
                        continue;
                    }

                    Task<Map> taskElement = e as Task<Map>;
                    if (taskElement != null)
                    {
                        taskElement.Execute(args);
                        continue;
                    }
                }
            }
        }
        public void Listen(string name, Task taskCallback)
        {
            taskCallback.Name = name;

            callbacks.Append(taskCallback as Element);
        }
        public void Listen(string name, Task<Map> taskCallback)
        {
            taskCallback.Name = name;

            callbacks.Append(taskCallback as Element);
        }

   
    }
}