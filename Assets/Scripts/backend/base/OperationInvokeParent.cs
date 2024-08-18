using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class OperationInvokeParent : Operation
    {

        protected override void Setup()
        {
            base.Setup();
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            bool hasParent = this.Node.Parent != "";
            if (hasParent) {
                Node p = NODE.Tree.Get<Node>(this.Node.Parent, null);
                if (p != null) {
                    string opName = this.Name;
                    if (Vars.Has("name_parent")) {
                        opName = Vars.Get<string>("name_parent");
                    }
                    p.ExecuteOperation(opName, args);
                }
            }
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}