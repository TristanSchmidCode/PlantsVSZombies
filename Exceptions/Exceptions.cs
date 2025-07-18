using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;


/// <summary>
/// Represents errors that occur when trying to add an object into an already full slot of an <see cref="ICollection{T}"/> Collection
/// </summary>
[Serializable]
public class IndexFullExeption : Exception
{
    public IndexFullExeption() :base() { }
    public IndexFullExeption(string? message) : base(message) { }
    public IndexFullExeption(string? message, Exception innerException):base(message, innerException) { }
}


/// <summary>
///    Represents errors that occur when Giving a position Which is out its corraspoinding map
/// </summary>
[Serializable]
public class PositionOutOfBoundsException : Exception
{
    public PositionOutOfBoundsException() :base() { }
    public PositionOutOfBoundsException(string? message) : base(message) { }
    public PositionOutOfBoundsException(string? message, Exception innerException) : base(message, innerException) { }
}
[Serializable]

public class FightWonException:Exception
{
    public FightWonException() :base() { }
    public FightWonException(string? message) : base(message) { }
    public FightWonException(string? message, Exception innerException) : base(message, innerException) { }
}
[Serializable]
public class FightLostException:Exception
{
    public FightLostException() :base() { }
    public FightLostException(string? message) : base(message) { }
    public FightLostException(string? message, Exception innerException) : base(message, innerException) { }
}


