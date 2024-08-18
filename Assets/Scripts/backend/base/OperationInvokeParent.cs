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

            bool hasParent = this.Node.Parent != null;
            if (hasParent) {
                Node p = this.Node.Parent;
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