using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace snorri
{
    public class LayoutButtonModule : Module
    {
        public bool IsClickable;
        int index;
        public int ID {
            get { return index; }
        }

        protected override void Setup()
        {
            base.Setup();

            IsClickable = Vars.Get<bool>("is_clickable", true); 
            index = Vars.Get<int>("index", 0); 
        }
        protected override void Launch() {
            base.Launch();
        }
        protected override void WhenDestroy() {
            base.WhenDestroy();
        }

        protected override void WhenEnable() {
            base.WhenEnable();

            if (Vars.Has("index")) {
                index = Vars.Get<int>("index", 0); 
                GAME.Vars.Set<LayoutButtonModule>($"layout:buttons:{index}", this);

                LOG.Console($"actor for {this.Node.Name}, has button index: {index}");
            }
        }
        protected override void WhenDisable() {
            base.WhenDisable();

            Map buttons = GAME.Vars.Get<Map>("layout:buttons", new Map());
            LayoutButtonModule mod = buttons.Get<LayoutButtonModule>(index.ToString(), null);
            if (mod == this)
                buttons.Remove(this.index.ToString());
        }
        
        bool isSelected = false;
        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                if (!IsClickable) {
                    isSelected = false;
                    return;
                }
                isSelected = value;
            }
        }
        public void InformState(LayoutState state) {
            switch (state) {
                case LayoutState.HoverIn:
                    CallbackCheck("hover_in");
                    break;
                case LayoutState.HoverOut:
                    if (!IsSelected)
                        CallbackCheck("hover_out");
                    break;
                case LayoutState.ClickIn:
                    CallbackCheck("click_in");
                    if (!IsClickable) {
                        InformState(LayoutState.HoverOut);
                    }
                    break;
                case LayoutState.ClickOut:
                    CallbackCheck("click_out");
                    CallbackCheck("hover_out");
                    break;
            }
        }

        void CallbackCheck(string typeName)
        {
            foreach (Task task in Vars.Get<Bag<Task>>(typeName, new Bag<Task>()))
            {
                task.Execute();
            }

            // operations:
            Map args = new Map();
            
            switch (typeName)
            {
                case "hover_in":
                    args.Set<bool>("is_hover_in", true);
                    args.Set<bool>("is_fade_in", true);
                    Node.ExecuteOperation("hover", args);
                    break;
                case "hover_out":
                    args.Set<bool>("is_hover_in", false);
                    args.Set<bool>("is_fade_in", false);
                    Node.ExecuteOperation("hover", args);
                    break;
                case "click_in":
                    args.Set<bool>("is_click_in", true);
                    Node.ExecuteOperation("click", args);
                    break;
                case "click_out":
                    args.Set<bool>("is_click_in", false);
                    Node.ExecuteOperation("click", args);
                    break;
            }
        }
        public void Listen(string typeName, Task task_callback)
        {
            Bag<Task> tasks = Vars.Get<Bag<Task>>(typeName, new Bag<Task>());
            tasks.Append(task_callback);

            Vars.Set<Bag<Task>>(typeName, tasks);
        }
    }
}