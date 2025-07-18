namespace PlantsVSZombies;

public abstract class Intoractible:IEntity
{
    public static readonly List<Intoractible> all = [];
    
    public static Intoractible? NextIntor(ConsoleKey direction)
    {
        Intoractible? ds = null;
        float Priority = float.MaxValue;
        foreach (Intoractible d in all)
        {
            float? DSD = d.MovmentPriority(Cursor.Current.frame.Pos, direction);
            if (DSD == null)
                continue;
            if (DSD < Priority)
            {
                ds = d;
                Priority = (float)DSD;
            }
        }
        return ds;
    }
    static Intoractible() { }

    
    public ImageHaver Symol;
    public ImageHaver frame;
    
    public  LayerID Layer { get { return frame.Layer; } }
    public bool shown = false;
    //the posiiton from the its center which the cursor should move to. 
    public Position Displacement { get; protected set; }

    public virtual void Show()
    {
        frame?.PrintImage();
        Symol?.PrintImage();
        shown = true;
    }
    public virtual void Hide()
    {
        frame?.RemoveImage();
        Symol?.RemoveImage();
        shown = false;
    }
    //returns how high a priority it is for the cursor to move there 
    //the lower the result, the higher the priority
    float? MovmentPriority(Position currentPos,ConsoleKey direction)
    {
        if (shown == false)
            return null;
        float Priority;

        int c_Left = currentPos.X;
        int c_Up = currentPos.Y;
        int Left = frame.Pos.X;
        int Up = frame.Pos.Y;
        float leftDifrence = Math.Abs(c_Left - Left);
        float upDifrence = Math.Abs(c_Up - Up);

        if (direction == ConsoleKey.UpArrow)
            if (c_Up <= Up)
                return null;
        if (direction == ConsoleKey.DownArrow)
            if (c_Up >= Up)
                return null;

        if (direction == ConsoleKey.LeftArrow)
            if (c_Left <= Left)
                return null;
        if (direction == ConsoleKey.RightArrow)
            if (c_Left >= Left)
                return null;


        if (direction == ConsoleKey.RightArrow || direction == ConsoleKey.LeftArrow)
        {
            Priority = 0;
            Priority += leftDifrence;
            Priority += upDifrence * 100;

        }
        else
        {
            int i = 1;
            while (leftDifrence > 10 * i)
                i++;

            Priority = 0;
            Priority += i * 100;
            Priority += upDifrence;


        }
        return Priority;
    }
    public abstract void TakeAction();
    public abstract bool BeActedOn<T>(T d) where T : IAction;
    public virtual void Destroy()
    {
        AllEntitys.RemoveEntities(this);
        all.Remove(this);
        frame.RemoveImage();
        Symol.RemoveImage();
    }
    public Intoractible(Position pos)
    {
        PlantSpace.Nothing();
        frame = new(Layers.Frame, pos);
        Symol = new(Layers.InerFrame, pos);
        all.Add(this);
    }
    public Intoractible(Position pos, Position fromCenter)
    {
        PlantSpace.Nothing();
        frame = new(Layers.Frame, pos);
        Symol = new(Layers.InerFrame, pos);
        all.Add(this);
        Displacement = fromCenter;
    }
    public Intoractible(Position pos, Position fromCenter,Layers layer)
    {
        PlantSpace.Nothing();
        Symol = new(layer, pos);
        frame = new(layer, pos);
        all.Add(this);
        Displacement = fromCenter;
    }

}