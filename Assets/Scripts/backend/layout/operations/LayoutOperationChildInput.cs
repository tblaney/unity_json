using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;


namespace snorri
{
    public class LayoutOperationChildInput : Operation
    {
        Node childNode;
        LayoutInputFieldModule text;

        protected override void Setup()
        {
            base.Setup();
        }
        protected override void Launch() {
            base.Launch();

            bool hasChildNode = this.Node.FindChild(Vars.Get<string>("child", "text"), out childNode);
            if (hasChildNode) {
                text = childNode.GetActor<LayoutInputFieldModule>();
            } else {
                LOG.Console("layout operation child input couldnt find input field module");
            }
        }

        public override void Execute(
            Map args = null)
        {
            base.Stop();

            LOG.Console("layout operation child input executing! " + args.Get<bool>("is_click_in", false).ToString());

            if (childNode == null) return;
            if (text == null) return;

            bool isVal = args.Get<bool>("is_click_in", false);

            text.ActivateInputField(isVal);
        }

        protected override void WhenStop()
        {
            // base cleanup
        }
    }
}