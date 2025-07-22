using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public abstract class Sun : ImageHaver, IEntity
{
    public const float speed = 4;
    //Is the amount of sun it gives
    public readonly int value;
    //is the position that the sun will end at 
    readonly Position _endPos;
    
    //uses the function f(x)=a*(x-h)^2+k were (h,k) is the functions vertex
    protected Sun(
        Position EndPos, 
        Position StartPos, 
        int value, 
        bool plantMade = false) : base(Layers.Sun,StartPos)
    {
        float C = 5;
        EntityHanderler.Instance.AddEntity(this);

        this._endPos = EndPos;
        this.value = value;

        float x1 = StartPos.X;
        float y1 = StartPos.Y;

        float x2 = EndPos.X;
        float y2 = EndPos.Y;
        if (plantMade == true)
        {
            float X = (x1 - x2)/2;
            x1 -= X;
            y1 -= C;
        }

        _k = y1;
        _h = x1;
        
        _a = (-_k+y2) / (MathF.Pow(_h,2)-2*x2*_h+MathF.Pow(x2,2));


        if (EndPos.X > StartPos.X)
            _direction = 1;
        else
            _direction = -1;
    }
    public virtual bool BeActedOn<T>(T d) where T : IAction
    {
        if (d is PickUP)
        {
            Money.Instance.AddMoney(value);
            Destroy();
        }
        return false;
    }

    public virtual void Destroy()
    {
        EntityHanderler.Instance.RemoveEntity(this);
        RemoveImage();
    }


    readonly float _h;
    readonly float _k;
    readonly float _a;

    readonly float _direction;

    public virtual void TakeAction()
    {
        if (Pos.Pos == (_endPos.X, _endPos.Y))
            return;

        float x = SpecificPos.X + Time.DeltaTime * speed * _direction;

        float y = _a*MathF.Pow((x-_h),2)+_k;

        if (Pos.X * _direction > _endPos.X * _direction)
            MoveTo(new Position((_endPos.X, _endPos.Y)));
        else
            MoveTo((x, y));
    }
}
public class Sun25: Sun
{   
    /// <summary>
    /// Creates a sun at the given pos, and makes it arc to a position 15 pixels ahed
    /// </summary>
    /// <param name="Pos"> </param>
    public Sun25(Position Pos) : base(
        EndPos: Pos + (15, 0),
        StartPos: Pos,
        value: 25,
        plantMade: true
        )
    {
        Image building = Image.GetImage(Layers.Sun, "Sun25");
        ChangeImage(building.RandomiseImage(10));
    }
    /// <summary>
    /// Creates a sun at the top of the screen, that will then ark towards a random plant pos
    /// </summary>
    public Sun25() : base(
        EndPos:
        new Position(
            (new Random().Next(BordInfo.PlantMapSize.left), 
            new Random().Next(BordInfo.PlantMapSize.up)
            ).PlantPosition()
            + (new Random().Next(-3, 3), new Random().Next(-1, 1) ) ),
        StartPos: (new Random().Next(12, BordInfo.MapLeft - 12), 0),
        value: 25,
        plantMade: false)
    {
        ChangeImage(Image.GetImage(Layers.Sun, "Sun25", 10));
    }
}

    

