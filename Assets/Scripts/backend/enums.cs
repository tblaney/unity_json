namespace snorri
{
    public enum ActorState
    {
        Setup,
        Launch,

        Enable,
        Disable,

        Destroy,
        
        Tick,
        TickPhysics,

        FirstFrame
    }
    public enum Direction
    {
        Up,
        Down,
        North,
        South,
        East,
        West
    }
    public enum LayoutState
    {
        HoverIn,
        HoverOut,
        ClickIn,
        ClickOut
    }
}