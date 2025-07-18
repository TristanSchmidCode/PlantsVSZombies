namespace PlantsVSZombies;

public class Gradient(
    Layers layer,
    Position pos,
    List<Position> poses,
    GradientType color)
{
    public LayerID Layer { get; private set; } = new(layer);
    public Position Pos { get; private set; } = new(pos);
    
    public Position[] GradientPoses { get; private set;  } = [.. poses];
    GradientType _gradient = color;

    public void ChangeGradient(GradientType gradent)
    {
        _gradient = gradent;
        Print(true);
    }
    public void Print(bool remove = true)
    {
        if (remove)
            Remove();

        foreach (Position pos in GradientPoses)
        {
            // this is faster then a try...catch 
            if (!(Pos + pos).IsOutOfBounds())
                ((Pos + pos)).GetPixel().AddGradient(Layer, _gradient);
        }
    }
    public void ChangeGradient(Position[] shape)
    {
        Remove();
        GradientPoses = shape;
        Print(false);
    }
    public void Remove()
    {
        foreach (Position pos in GradientPoses)
        {
            if (!(Pos + pos).IsOutOfBounds())
                (Pos + pos).GetPixel().RemoveGradient(Layer);
           
        }
    }

    /// <summary>
    /// Changes the position to a new position
    /// </summary>
    /// <param name="MoveTo"></param>
    public void MoveGradient(Position MoveTo)
    {
        Remove();
        Pos = MoveTo;
        Print(false);
        Pos = MoveTo;
    }
}

