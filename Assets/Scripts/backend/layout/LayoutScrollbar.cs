using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace snorri
{
    public class LayoutScrollbar : Ticker
    {
        float speed;
        string childName;
        Node child;
        PointLayout childPoint;
        protected override void Setup() {
            base.Setup();
            childName = Vars.Get<string>("child", "");
            speed = Vars.Get<float>("speed", 1f);
        }
        protected override void Launch() {
            base.Launch();
            bool hasNode = this.Node.FindChild(childName, out child);
            if (hasNode) {
                childPoint = child.Point as PointLayout;
            }
        }
        public override void Tick() {
            base.Tick();

            if (child == null) return;

            PointLayout pL = this.Node.Point as PointLayout;
            if (pL == null) {
                //LOG.Console("layout scrollbar point layout failed!");
                //LOG.Console("layout scrollbar: " + this.Node.Point.gameObject.name);
                return;
            }

            bool isOver = pL.IsMouseOver;
            //LOG.Console("layout scrollbar tick, is mouse over: " + isOver.ToString());

            if (isOver) {
                RefreshPositions();
            }
        }
        void RefreshPositions() {
            float scrollDelta = GAME.Vars.Get<float>("input:mouse_scroll", Input.mouseScrollDelta.y);
            
            Vec pos = childPoint.Position;

            //LOG.Console("layout scroll current pos: " + pos.vec2.ToString());
            if (scrollDelta > 0f) {
                pos.y -= TIME.Delta*speed*childPoint.Height;
            } else if (scrollDelta < 0f) {
                pos.y += TIME.Delta*speed*childPoint.Height;
            }

            if (pos.y < 0f) {
                pos.y = 0f;
            }

            if (pos.y > childPoint.Height) {
                pos.y = childPoint.Height;
            }

            //LOG.Console("layout scroll new pos: " + pos.vec2.ToString());

            childPoint.Position = pos;
        }
    }
}