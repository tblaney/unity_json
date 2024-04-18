namespace snorri
{
    public class CastTicker : Ticker
    {
        public Cast this[string name]
        {
            get 
            {
                bool isUpdate = Vars.Get<bool>(name + ":is_update", false);
                if (!isUpdate)
                {
                    ProcessRequest(name);
                }
                return Vars.Get<Cast>(name + ":cast", null);
            }
        }
        public override void Tick()
        {
            base.Tick();

            int i = 0;


            foreach (string requestName in Vars.Elements.Keys)
            {
                Map request = Vars.Get<Map>(requestName, new Map());

                bool isUpdate = request.Get<bool>("is_update", false);
                if (!isUpdate)
                    continue;

                ProcessRequest(requestName);

                //LOG.Console($"cast ticker has cast: {requestName}");

                i++;
            }    

            //LOG.Console($"cast ticker has {i} casts!");
            //Vars.Log();
        }

        void ProcessRequest(string name)
        {
            Map request = Vars.Get<Map>(name, new Map());

            Map argsBase = request.Get<Map>("args", new Map());

            Map args = new Map();
            args.Sync(argsBase);

            if (args.Get<bool>("is_origin_local", false))
            {
                Bag<float> originCurrent = argsBase.Get<Bag<float>>("origin", new Bag<float>(0,0,0));
                Bag<float> originNew = new Bag<float>(originCurrent.All());
                originNew[0] = Node.Point.North.x*originNew[0] + Node.Point.Position.x;
                originNew[1] = originNew[1] + Node.Point.Position.y;
                originNew[2] = Node.Point.North.z*originNew[2] + Node.Point.Position.z;
                args.Set<Bag<float>>("origin", originNew);
            }

            string directionName = args.Get<string>("direction_local", "");
            if (directionName != "")
            {
                switch (directionName)
                {
                    case "forward":
                        Vec north = Node.Point.North;
                        north.y = 0f;
                        args.Set<Bag<float>>("direction", north.AsBag());
                        break;
                    case "south":
                        args.Set<Bag<float>>("direction", Node.Point.South.AsBag());
                        break;
                    case "east":
                        args.Set<Bag<float>>("direction", Node.Point.East.AsBag());
                        break;
                    case "west":
                        args.Set<Bag<float>>("direction", Node.Point.West.AsBag());
                        break;
                }
            }

            request.Set<Cast>("cast", new Cast(args));

        }
    }
}