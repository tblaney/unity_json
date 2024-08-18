namespace snorri
{
    public class AbilityStateNode : Ability
    {
        string stateName;
        bool isParent;

        Node parentNode;

        protected override void Setup()
        {
            base.Setup();

            isParent = Vars.Get<bool>("is_parent", false);
            stateName = Vars.Get<string>("state", "");

            parentNode = NODE.Tree.Get<Node>(this.Node.Parent, null);
        }
        public override bool CanRun() {
            if (isParent && parentNode != null) {
                return parentNode.Vars.Get<string>("state", "") == stateName;
            } else {
                return this.Node.Vars.Get<string>("state", "") == stateName;
            }
        }
    }
}