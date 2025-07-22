using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace PlantsVSZombies;


/// <summary>
///  A struct that contains a position, and methods to olter it 
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
public readonly struct Position(int X, int Y)
{
    
    public readonly (int X, int Y) Pos => (X,Y);
    public readonly int X = X;
    public readonly int Y = Y;

    public static implicit operator (int, int)(Position d) => d.Pos;
    public static implicit operator Position((int, int) d) => new(d);

    public static bool operator ==(Position a, Position b) => (a.X == b.X | a.Y ==  b.Y);
    
    public static bool operator !=(Position a, Position b) => !(a == b);
    public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, Position b) =>new(a.X - b.X, a.Y - b.Y);
    public static Position operator +(Position a, (int X, int Y) b) => new(a.X + b.X, a.Y + b.Y);
    public static Position operator -(Position a, (int X, int Y) b) => new(a.X - b.X, a.Y - b.Y);
    public static Position operator *(Position a, int b) => new(a.X * b, a.Y * b);
    public static Position operator /(Position a, int b) => new(a.X / b, a.Y / b);
    


    public override bool Equals(object? obj) =>obj is Position pos && (pos.Pos == this);
    public override int GetHashCode() => base.GetHashCode();
    public override string ToString() => $"({X}, {Y})";

    public Position((int x, int y) pos) : this(pos.x, pos.y) { }
    public Position() : this(0, 0) { }       
}
