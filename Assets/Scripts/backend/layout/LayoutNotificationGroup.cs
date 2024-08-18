namespace snorri
{
    using UnityEngine;

    public class LayoutNotificationGroup : Ticker {
        // just pull from global notificaiton list and spawn
        Bag<Map> activeNotifications = new Bag<Map>();
        string nodeName;
        string nodeNameLarge;
        int maxCharNumber = 100;

        int counter = 0;

        protected override void Setup() 
        {
            base.Setup();

            JSON.IsNotify = true;

            nodeName = Vars.Get<string>("node_notification", "layout_notification");
            nodeNameLarge = Vars.Get<string>("node_notification_large", "layout_notification_large");

            maxCharNumber = Vars.Get<int>("max_char", 100);
        }
        public override void Tick() {
            base.Tick();

            Bag<Map> notifications = GAME.Notifications;
            if (notifications.Length > 0) {
                ProcessNotification(notifications.Pop());
                GAME.Notifications = notifications;
            }
        }
        void ProcessNotification(Map args) {
            Map newNodeMap = new Map();
            bool isError = args.Get<bool>("is_error", false);
            string text = args.Get<string>("text", "");
            char[] charArray = text.ToCharArray();
            string nodeNameToSpawn = nodeName;
            LOG.Console("layout notification group: " + charArray.Length.ToString());
            if (charArray.Length < maxCharNumber) {
                //newNodeMap.Set<string>("inherit_from", nodeName);
            } else {
                nodeNameToSpawn = nodeNameLarge;
                //newNodeMap.Set<string>("inherit_from", nodeNameLarge);
            }
            newNodeMap.Set<string>("inherit_from", nodeNameToSpawn);
            Map m = new Map();
            m.Set<string>("text", text);
            newNodeMap.Set<Map>(
                "children:group:children:text_box:children:text:actors:layout_text_module",
                m
            );
            if (isError) {
                newNodeMap.Set<string>("children:group:children:text_box:actors:layout_image_module:color", "#ba3811");
            }
            LOG.Console("layout notification group new notification! with base node: " + nodeNameToSpawn);
            newNodeMap.Log();
            Node childNode = this.Node.AddChild("notification_"+counter.ToString(), newNodeMap);
            counter++;
        }
    }
}