using Microsoft.VisualBasic;

namespace PlantsVSZombies;
public abstract class Intoractible
{
    public static readonly List<Intoractible> all = [];
    
    public static Intoractible? NextIntor(ConsoleKey direction)
    {
        Intoractible? ds = null;
        float Priority = float.MaxValue;
        foreach (Intoractible d in all)
        {
            float? DSD = d.MovmentPriority(Cursor.Target.Frame.Pos, direction);
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
    public ImageHaver Frame;
    
    public  LayerID Layer { get { return Frame.Layer; } }
    public bool shown = true;
    //the posiiton from the its center which the cursor should move to. 
    public Position Displacement { get; protected set; }

    public virtual void Show()
    {
        Frame?.PrintImage();
        Symol?.PrintImage();
        shown = true;
    }
    public virtual void Hide()
    {
        Frame?.RemoveImage();
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
        int Left = Frame.Pos.X;
        int Up = Frame.Pos.Y;
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
    public abstract bool BeActedOn<T>(T d) where T : IAction;
    public virtual void Destroy()
    {
        all.Remove(this);
        shown = false;
        Frame.RemoveImage();
        Symol.RemoveImage();
    }
    public Intoractible(Position pos): this(pos, new Position(0,0)) { }
    public Intoractible(Position pos, Position fromCenter)
    {
        all.Add(this);

        Frame = new(Layers.Frame, pos);
        Symol = new(Layers.InerFrame, pos);
        Displacement = fromCenter;
        Show();
    }
    public Intoractible(Position pos, Position fromCenter,Layers layer) : 
        this(pos,fromCenter,layer, null,null) { }

    public Intoractible(Position pos, Position fromCenter, Layers layer, Image? symol, Image? frame) 
    {
        all.Add(this);
        Symol = new ImageHaver(symol ?? new Image(), layer, pos);
        Frame = new ImageHaver(frame ?? new Image(), layer, pos);     
        Displacement = fromCenter;
        Show();
    }

}