using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class OperationInvokeChild : Operation
    {

        protected override void Setup()
        {
            base.Setup();
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            foreach (string childName in Vars.Get<Bag<string>>("childs", new Bag<string>())) {
                bool hasChild = this.Node.FindChild(childName, out Node childNode);
                if (hasChild) {
                    childNode.ExecuteOperation(this.Name, args);
                }
            }
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}