using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class OperationTerminate : Operation
    {

        protected override void Setup()
        {
            base.Setup();
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            this.Node.Terminate();
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}