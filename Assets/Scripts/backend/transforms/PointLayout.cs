using UnityEngine;

namespace snorri
{
    [UnityEngine.DisallowMultipleComponent]
    public class PointLayout : Point
    {
        public RectTransform rect;
        public Canvas Canvas {
            get {
                return GAME.Vars.Get<Canvas>("canvas", null);
        }}

        public override PointState State
        {
            get
            {
                return new PointState(this);
            }
            set
            {
                value.Apply(this);
            }
        }

        protected override void AddClasses()
        {
            rect = ComponentCheck<RectTransform>();
        }

        protected override void Setup()
        {
            base.Setup();

            Configure();
        }

        void Configure()
        {
            Bag<float> pivotPoint = Vars.Get<Bag<float>>("pivot", new Bag<float>(0.5f,0.5f));
            Pivot = new Vec(pivotPoint);

            if (Vars.Has("anchor_min") && Vars.Has("anchor_max"))
            {
                Bag<float> anchorPointMin = Vars.Get<Bag<float>>("anchor_min", new Bag<float>(0f,0f));
                Bag<float> anchorPointMax = Vars.Get<Bag<float>>("anchor_max", new Bag<float>(1f,1f));

                AnchorMin = new Vec(anchorPointMin);
                AnchorMax = new Vec(anchorPointMax);

                rect.offsetMin = Vector2.zero; // This sets left and bottom offsets to 0
                rect.offsetMax = Vector2.zero; // This sets right and top offsets to 0
            } else
            {
                Bag<float> anchorPoint = Vars.Get<Bag<float>>("anchor", new Bag<float>(0.5f,0.5f));
                Anchor = new Vec(anchorPoint);
            }

            PointState state = new PointState(Vars);
            this.State = state;

            //float width = Vars.Get<float>("width",1f);
            //float height = Vars.Get<float>("height",1f);
            //rect.sizeDelta = new Vector2(width, height);
        }

        public float Width {
            get {
                return rect.sizeDelta.x;
            }
            set {
                Vector2 size = rect.sizeDelta;
                size.x = value;
                rect.sizeDelta = size;
            }
        }
        public float Height {
            get {
                return rect.sizeDelta.y;
            }
            set {
                Vector2 size = rect.sizeDelta;
                size.y = value;
                rect.sizeDelta = size;
            }
        }

        public Vec Pivot {
            get { return new Vec(rect.pivot); }
            set {
                rect.pivot = value.vec2;
            }
        }
        public Vec Anchor {
            get { return new Vec(rect.anchorMin); }
            set {
                rect.anchorMin = value.vec2;
                rect.anchorMax = value.vec2;
            }
        }
        public Vec AnchorMin {
            get { return new Vec(rect.anchorMin); }
            set {
                rect.anchorMin = value.vec2;
            }
        }
        public Vec AnchorMax {
            get { return new Vec(rect.anchorMax); }
            set {
                rect.anchorMax = value.vec2;
            }
        }
        public Vec ScreenPosition
        {
            get
            {
                Vector3 worldPosition = rect.position;
                return new Vec(RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition));
            }
            set
            {
                if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, value.vec2, Camera.main, out Vector3 worldPosition))
                {
                    rect.position = worldPosition;
                }
            }
        }
        public bool IsMouseOver {
            get {
                if (Canvas == null) {
                    LOG.Console("point layout no canvas!");
                    return false;
                }

                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(Canvas.transform as RectTransform, UnityEngine.Input.mousePosition, Canvas.worldCamera, out canvasPos);
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, 
                    UnityEngine.Input.mousePosition, Canvas.worldCamera)) {
                    return true;
                }   
                else {
                    return false;
                }
            }
        }

        // publics
        public override Vec Position { get { return new Vec(rect.anchoredPosition); } set { rect.anchoredPosition = value.vec2; } }
        public override VecInt PositionInt { get { return new VecInt(rect.anchoredPosition); } set { rect.anchoredPosition = value.vec2; } }
    }
}