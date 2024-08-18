using Newtonsoft.Json;
namespace snorri
{
    [System.Serializable]
    public class PointState : Element
    {
        public Map Vars {get; set;}

        public bool isLayout;
        public bool isLocal;

        public PointState()
        {

        }
        public PointState(Map vars)
        {
            Name = "point_state";

            this.Vars = new Map();

            Vars.Set<Vec>("position", new Vec(vars.Get<Bag<float>>("position", new Bag<float>(0,0,0))));
            Vars.Set<Vec>("rotation", new Vec(vars.Get<Bag<float>>("rotation", new Bag<float>(0,0,0))));
            Vars.Set<Vec>("scale", new Vec(vars.Get<Bag<float>>("scale", new Bag<float>(1,1,1))));

            isLocal = vars.Get<bool>("is_local", false);
            
            // layout maybe
            if (vars.Has("width")) {
                Vars.Set<float>("width", vars.Get<float>("width", 0f));
            }
            if (vars.Has("height")) {
                Vars.Set<float>("height", vars.Get<float>("height", 0f));
            }
            if (Position.Length < 3 || vars.Has("width") || vars.Has("height")) {
                isLayout = true;
            }
        }
        public Map ToMap() {
            Map m = new Map();
            Vec pos = Position;
            Vec rot = Rotation;
            Vec scale = Scale;
            if (pos.Length == 3) {
                m.Set<Bag<float>>("position", new Bag<float>(Position.x, Position.y, Position.z));
            } else {
                m.Set<Bag<float>>("position", new Bag<float>(Position.x, Position.y));
            }
            m.Set<Bag<float>>("rotation", new Bag<float>(Rotation.x, Rotation.y, Rotation.z));
            m.Set<Bag<float>>("scale", new Bag<float>(Scale.x, Scale.y, Scale.z));
            m.Set<bool>("is_local", isLocal);
            if (isLayout) {
                m.Set<float>("height", Height);
                m.Set<float>("width", Width);
            }
            return m;
        }
        public PointState(Point p, bool isLocal = false)
        {
            Name = "point_state";

            Vars = new Map();

            if (isLocal) {
                Vars.Set<Vec>("position", new Vec(p.transform.localPosition));
                Vars.Set<Vec>("rotation", new Vec(p.transform.localEulerAngles));
            } else {
                Vars.Set<Vec>("position", new Vec(p.transform.position));
                Vars.Set<Vec>("rotation", new Vec(p.transform.eulerAngles));
            }
            Vars.Set<Vec>("scale", new Vec(p.transform.localScale));

            this.isLocal = isLocal;
        }
        public PointState(PointLayout pL)
        {
            Name = "point_state";
            Vars = new Map();

            if (isLocal) {
                Vars.Set<Vec>("rotation", new Vec(pL.transform.localEulerAngles));
            } else {
                Vars.Set<Vec>("rotation", new Vec(pL.transform.eulerAngles));
            }
            Vars.Set<Vec>("position", new Vec(pL.rect.anchoredPosition));
            Vars.Set<Vec>("scale", new Vec(pL.transform.localScale));
            Vars.Set<float>("width", pL.Width);
            Vars.Set<float>("height", pL.Height);

            isLayout = true;
        }
        public PointState(PointState state)
        {
            this.Name = "point_state";

            Vars = new Map();

            Vars.Set<Vec>("position", state.Position);
            Vars.Set<Vec>("rotation", state.Rotation);
            Vars.Set<Vec>("scale", state.Scale);

            Vars.Set<float>("width", state.Width);
            Vars.Set<float>("height", state.Height);

            isLayout = state.isLayout;
        }

        public PointState Lerp(PointState state_out, float time)
        {
            PointState state = new PointState(this);
            state.Position = state.Position.Lerp(state_out.Position, time);
            state.Rotation = state.Rotation.Lerp(state_out.Rotation, time, true);
            state.Scale = state.Scale.Lerp(state_out.Scale, time);
            if (isLayout) {
                state.Width = UnityEngine.Mathf.Lerp(state.Width, state_out.Width, time);
                state.Height = UnityEngine.Mathf.Lerp(state.Height, state_out.Height, time);
            }
            // LOG.Console("point state lerp - rotation: " + state.Rotation.Text());
            return state;
        }

        public Vec Position
        {
            get { return Vars.Get<Vec>("position", Vec.zero); }
            set { Vars.Set<Vec>("position", value); }
        }
        public Vec Rotation
        {
            get { return Vars.Get<Vec>("rotation", Vec.zero); }
            set { Vars.Set<Vec>("rotation", value); }
        }
        public Vec Scale
        {
            get { return Vars.Get<Vec>("scale", Vec.ones); }
            set { Vars.Set<Vec>("scale", value); }
        }
        public float Width {
            get { return Vars.Get<float>("width", 0f); }
            set { Vars.Set<float>("width", value); }
        }
        public float Height {
            get { return Vars.Get<float>("height", 0f); }
            set { Vars.Set<float>("height", value); }
        }


        public void Apply(Point p)
        {
            if (p is PointLayout pL)
            {
                pL.rect.anchoredPosition = Position.vec2;
                if (isLocal)
                    p.transform.localEulerAngles = Rotation.vec3;
                else
                    p.transform.eulerAngles = Rotation.vec3;
                    
                p.transform.localScale = Scale.vec3;

                pL.rect.sizeDelta = new UnityEngine.Vector2(Width, Height);
            } else
            {
                if (isLocal)
                {
                    p.transform.localPosition = Position.vec3;
                    p.transform.localEulerAngles = Rotation.vec3;
                    p.transform.localScale = Scale.vec3;
                } else
                {
                    p.transform.position = Position.vec3;
                    p.transform.eulerAngles = Rotation.vec3;
                    p.transform.localScale = Scale.vec3;
                }
            }
        }
    }
}