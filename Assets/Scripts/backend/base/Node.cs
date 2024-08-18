using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
namespace snorri
{
    public class Node : IMap, INameable
    {
        public Map Vars {get; set;}
        public Map VarsResource {get; set;} // should be discarded on link - its a one time use thing
        
         // -- properties -- //
        public string Name  {
            get {
                return Vars.Get<string>("name", "");
            } 
            set {   
                Vars.Set<string>("name", value);
            }
        }
        public Node Parent { 
            get {
                return Vars.Get<Node>("parent", null);
            }
            set {
                Vars.Set<Node>("parent", value);
            }
        }
        public Bag<Node> Children { 
            get {
                Bag<Node> bagOfChildren = Vars.Get<Bag<Node>>("children", null);
                if (bagOfChildren == null)
                {
                    bagOfChildren = new Bag<Node>();
                    Vars.Set<Bag<Node>>("children", bagOfChildren);
                }
                return bagOfChildren;
            }
            set {
                Vars.Set<Bag<Node>>("children", value);
            }
        }
        public NodeEntity Entity { 
            get {
                return Vars.Get<NodeEntity>("entity", null);
            }
            set {
                Vars.Set<NodeEntity>("entity", value);
            }
        }
        public Point Point {
            get {
                NodeEntity entity = this.Entity;
                if (entity != null)
                    return entity.Point;

                return null;
            }
        }
        public PointLayout PointLayout {
            get {
                NodeEntity entity = this.Entity;
                if (entity != null) {
                    return entity.Point as PointLayout;
                }

                return null;
            }
        }
        public Transform transform {
            get {
                NodeEntity entity = this.Entity;
                if (entity != null)
                    return entity.transform;

                return null;
            }
        }
        public Body Body {
            get {
                return Vars.Get<Body>("body", null);
            } 
            set {
                Vars.Set<Body>("body", value);
            }
        }
        public bool IsBuilt { 
            get {
                return Vars.Get<bool>("is_built", false);
            }
            set {
                Vars.Set<bool>("is_built", value);
            }
        }
        public Bag<IActor> Actors {
            get {
                Bag<IActor> bagOfActors = Vars.Get<Bag<IActor>>("actors", null);
                if (bagOfActors == null) {
                    bagOfActors = new Bag<IActor>();
                    Vars.Set<Bag<IActor>>("actors", bagOfActors);
                }
                return bagOfActors;
            }
            private set {
                Vars.Set<Bag<IActor>>("actors", value);
            }
        }
        public Bag<Ticker> Tickers {
            get {
                Bag<Ticker> bagOfTickers = Vars.Get<Bag<Ticker>>("tickers", null);
                if (bagOfTickers == null) {
                    bagOfTickers = new Bag<Ticker>();
                    Vars.Set<Bag<Ticker>>("tickers", bagOfTickers);
                }
                return bagOfTickers;
            }
            private set {
                Vars.Set<Bag<Ticker>>("tickers", value);
            }
        }
        

        // -- constructors -- //
        public Node(string nodeName, string resourceFile, Node parentNode = null) {
            Vars = new Map();

            this.Name = nodeName;

            Map m = JSON.GetResourceMap(resourceFile, "nodes");
            this.VarsResource = m;

            if (parentNode != null) this.Parent = parentNode;

            Setup();
        }
        public Node(string nodeName, Map vars, Node parentNode = null) {
            Vars = new Map();

            this.Name = nodeName;

            this.VarsResource = vars;

            if (parentNode != null) this.Parent = parentNode;

            Setup();
        }

        // -- setup -- //
        void Setup() {
            InheritSetup();
            ChildrenSetup();
        }
        void InheritSetup() {
            LOG.Console("node inherit setup: " + this.Name);
            string inheritName = VarsResource.Get<string>("inherit_from", "");
            if (inheritName != "") {
                Map m = JSON.GetResourceMap(inheritName, "nodes");
                m.Sync(this.VarsResource);
                this.VarsResource = m;
            }   
        }
        void ChildrenSetup() {
            Map childMap = VarsResource.Get<Map>("children", new Map());
            foreach (string childKey in childMap.Elements.Keys) {
                Map childSetupMap = childMap.Get<Map>(childKey, new Map());

                if (this.VarsResource.Get<bool>("is_pass_layer_on", false))
                {
                    childSetupMap.Set<int>("layer", this.VarsResource.Get<int>("layer", 0));
                    childSetupMap.Set<bool>("is_pass_layer_on", true);
                }

                string childName = childKey;
                
                Node childNode = new Node(childName, childSetupMap, this);
                Children.Append(childNode);
            } 
        }

        // -- terminate -- //
        public void Terminate() {
            LOG.Console($"node terminated! {this.Name}");

            if (Parent != null)
            {
                Parent.Children.Remove(this);
            }

            this.Actors = new Bag<IActor>();
            this.Tickers = new Bag<Ticker>();

            // delete children first from tree
            TerminateChildren();

            if (IsBuilt)
            {
                Entity.Terminate();
            }
        }
        public void TerminateChildren() {
            foreach (Node child in this.Children) {
                if (child == null) continue;
                child.Terminate();
            }
        }

        // -- build -- //
        public void Build() {
            if (IsBuilt)
                return;
            
            NodeEntity entityOut = null;
            GameObject obj = null;
            bool isPrefab = VarsResource.Get<bool>("is_prefab", false);
            bool isAssignParent = true;

            if (isPrefab)
            {
                string prefabName = VarsResource.Get<string>("prefab_name", Name);
                obj = Node.NewPrefab(prefabName); 
                obj.name = Name;
            } else
            {
                if (Parent != null)
                {
                    bool hasObj = Parent.Entity.FindChildGameObject(Name, out obj);
                    if (!hasObj)
                    {
                        obj = Node.New(Name);
                    } else
                    {
                        isAssignParent = false;
                    }

                    obj.layer = VarsResource.Get<int>("layer", 0);

                } else
                {
                    obj = Node.New(Name);

                    obj.layer = VarsResource.Get<int>("layer", 0);
                }
            }
            
            PopulateActors(obj);

            // entity should be last thing we add
            if (Parent != null && isAssignParent)
            {
                // should set gameobject parent
                Node parentNode = this.Parent;

                obj.transform.SetParent(parentNode.Entity.transform);

                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = new Vector3(1,1,1);
            }

            entityOut = obj.AddComponent<NodeEntity>();

            Entity = entityOut;
            Entity.Link(this.Name, this);

            IsBuilt = true;

            BuildChildren();
        }
        void BuildChildren() {
            foreach (Node n in Children) {
                if (n != null)
                    n.Build();
            }
        }
        
        
        // -- enables -- //
        public void Enable(bool isEnable) {
            if (Entity != null) {
                Entity.gameObject.SetActive(isEnable);
            }

            LOG.Console("node enabled! " + this.Name + ", value: " + isEnable.ToString());

            if (isEnable) {
                InformActorsAndChildren(ActorState.Enable);
            } else {
                InformActorsAndChildren(ActorState.Disable);
            }
        }

        // -- children -- //
        public bool HasChild(string nodeName) {
            foreach (Node n in Children) {
                if (n.Name == nodeName) {
                    return true;
                }
            } return false;
        }
        public Node AddChild(string nodeName, Map overrideMap) {
            if (!HasChild(nodeName))
            {
                Node childNode = new Node(nodeName, overrideMap, this);

                if (IsBuilt)
                    childNode.Build();

                Children.Append(childNode);

                return childNode;

            } else
            {
                LOG.Console("tried adding node but one already exists: " + nodeName + ", parent: " + this.Name);
            }
            return null;
        }
        public bool FindChild(string name, out Node childNode) {
            childNode = null;
            foreach (Node n in Children) {
                if (n.Name == name) {
                    childNode = n;
                    return true;
                }
            } return false;
        }

        // -- actors -- //
        void SetupActor(string actorTypeName, Map actorTypeMap, GameObject obj) {
            Type classType = UTIL.GetSnorriType(actorTypeName);

            object o = null;

            if (typeof(Module).IsAssignableFrom(classType)) {
                Component component = obj.GetComponent(classType);
                if (component == null)
                    component = obj.AddComponent(classType);

                o = component as object;
            } else {
                LOG.Console($"node create instance! {actorTypeName}");
                o = UTIL.CreateInstance(classType);
            }

            IActor a = o as IActor;

            a.Link(this, actorTypeMap);

            this.Actors.Append(a);

            if (o is Ticker ticker) {
                this.Tickers.Append(ticker);
            }
        }
        void PopulateActors(GameObject obj) {
            Map actorMap = VarsResource.Get<Map>("actors", new Map());

            SetupDefaultActors(actorMap);

            foreach (string actorTypeName in actorMap.Elements.Keys) {
                Map actorTypeMap = actorMap.Get<Map>(actorTypeName, new Map());
                
                if (actorTypeMap.Has("instances")) {    
                    foreach (Map m in actorTypeMap.Get<Bag<Map>>("instances")) {
                        SetupActor(actorTypeName, m, obj);

                        LOG.Console("succesfully setup actor from instances!");
                    }
                    actorTypeMap.Remove("instances");
                }

                SetupActor(actorTypeName, actorTypeMap, obj);
            }
        }
        void SetupDefaultActors(Map actorMap) {
            // point check:
            if (!actorMap.Has("point") && !actorMap.Has("point_layout")) {
                actorMap.Set<Map>("point", new Map());
            }
        }
        public bool FindActor(string name, out Actor actor) {
            IActor iactor = Actors.Get(name);
            actor = null;
            if (iactor == null)
                return false;

            actor = iactor as Actor;
            return true;
        }
        public bool FindTicker(string name, out Ticker ticker) {
            IActor iactor = Actors.Get(name);
            ticker = null;
            if (iactor == null)
                return false;

            ticker = iactor as Ticker;
            return true;
        }
        public bool FindModule(string name, out Module module) {
            IActor iactor = Actors.Get(name);
            module = null;
            if (iactor == null)
                return false;

            module = iactor as Module;
            return true;
        }
        public bool FindOperation(string name, out Operation op) {
            IActor iactor = Actors.Get(name);
            op = null;
            if (iactor == null)
                return false;

            op = iactor as Operation;
            return true;
        }
        public T GetActor<T>(bool forceAdd = false) where T : IActor {
            foreach (IActor actor in this.Actors)
            {
                if (actor is T val)
                {
                    return val;
                }
            }
            if (forceAdd)
            {
                return AddActor<T>();
            }
            return default(T);
        }
        public Bag<T> GetBagOActors<T>() {
            Bag<T> bagOut = new Bag<T>();

            foreach (IActor actor in this.Actors)
            {
                if (actor is T val)
                {
                    bagOut.Append(val);
                }
            }

            return bagOut;
        }
        public Bag<T> GetBagOChildActors<T>(bool isLimitedToFirstLevel = false) {
            Bag<T> bagOut = GetBagOActors<T>();
            foreach (Node n in Children)
            {
                if (n != null)
                {
                    if (isLimitedToFirstLevel)
                    {
                        bagOut.AppendBag(n.GetBagOActors<T>());
                    } else
                    {
                        bagOut.AppendBag(n.GetBagOChildActors<T>());
                    }
                }
            }
            return bagOut;
        }
        public T AddActor<T>(Map vars = null) where T : IActor {
            Type classType = typeof(T);

            object o = null;

            if (typeof(Module).IsAssignableFrom(classType))
            {
                Component component = Entity.gameObject.GetComponent(classType);
                if (component == null)
                    component = Entity.gameObject.AddComponent(classType);

                o = component as object;
            } else
            {
                o = UTIL.CreateInstance(classType);
            }

            IActor a = o as IActor;

            if (vars == null)
                vars = new Map();
                
            a.Link(this, vars);

            a.Inform(ActorState.Setup);
            a.Inform(ActorState.Launch);

            this.Actors.Append(a);

            if (o is Ticker ticker)
            {
                this.Tickers.Append(ticker);
            }

            return (T)o;
        }
        public void AddActor(string actorTypeName) {
            Type classType = UTIL.GetSnorriType(actorTypeName);

            object o = null;

            if (typeof(Module).IsAssignableFrom(classType))
            {
                Component component = Entity.gameObject.GetComponent(classType);
                if (component == null)
                    component = Entity.gameObject.AddComponent(classType);

                o = component as object;
            } else
            {
                o = UTIL.CreateInstance(classType);
            }

            IActor a = o as IActor;

            a.Link(this, new Map());

            a.Inform(ActorState.Setup);
            a.Inform(ActorState.Launch);

            this.Actors.Append(a);

            if (o is Ticker ticker)
            {
                this.Tickers.Append(ticker);
            }
        }
        public void InformActorsAndChildren(ActorState state) {
            InformActors(state);

            foreach (Node n in Children) {
                if (n != null) {
                    n.InformActorsAndChildren(state);
                }
            }
        }
        public void InformActors(ActorState state) {
            foreach (IActor actor in Actors)
            {
                actor.Inform(state);
            }
        }
        
        // -- coroutines -- //
        public Coroutine Execute(string routineName, Map args) {
            LOG.Console($"node has been executed with name: {routineName}");
            switch (routineName) {
                case "delay":
                    return Entity.InvokeTask(args.Get<Task>("task", null),
                        args.Get<float>("time_delay", 0.1f));

                case "wait":
                    return Entity.InvokeWait(args.Get<Task<int, bool>>("task_wait_condition", null),
                        args.Get<Task>("task_when_complete", null));
            }

            return null;
        }
        public Coroutine Execute<T>(string routineName, Map args) {
            switch (routineName) {
                case "load":
                    return Entity.InvokeLoad<T>(
                        source:args.Get<IEnumerable<T>>("source_collection", null),
                        actionCallback:args.Get<Task<T>>("task_when_callback", null),
                        counter:args.Get<int>("counter", 200),
                        actionOnEnd:args.Get<Task>("task_when_end", null));

                case "lerp":
                    return Entity.InvokeLerp<T>(
                        args.Get<string>("lerp_type", "point"),
                        args.Get<T>("val_in", default(T)),
                        args.Get<T>("val_out", default(T)),
                        args.Get<float>("time", 1f),
                        args.Get<Task<T>>("task_when_callback", null),
                        args.Get<Task>("task_when_end", null));
            }

            return null;
        }
        public Coroutine Execute(IEnumerator routine) {
            return Entity.InvokeRoutine(routine);
        }

        // -- sync vars -- //
        public void Sync(Map args, Bag<string> exceptions = null) {
            this.Vars.Sync(args, exceptions);
        }
        public void SyncChildren() {
            foreach (Node n in this.Children) {
                if (n != null) {
                    n.Sync(this.Vars, new Bag<string>("name"));
                }
            }
        }
        public void SyncAndChildren(Map args, Bag<string> exceptions = null) {
            this.Sync(args, exceptions);
            foreach (Node n in this.Children) {
                if (n != null) {
                    n.SyncAndChildren(args, exceptions);
                }
            }
        }
        
        // -- operations -- //
        public void ExecuteOperation(string operationName, Map args = null) {
            if (!IsBuilt)
                return;
            // executes an operation

            foreach (IActor actor in Actors) {
                if (actor.Name == operationName) {
                    Operation op = actor as Operation;
                    if (op != null) {
                        op.Execute(args);
                    }
                }
            }
        }
        
        // -- static -- //
        public static GameObject New(string name) {
            GameObject obj = new GameObject(name);
            return obj;
        }
        public static GameObject NewPrefab(string prefabName) {
            GameObject obj = NODE.TaskToSpawnPrefab.Execute(prefabName);
            obj.name = prefabName;
            return obj;
        }
    }
}