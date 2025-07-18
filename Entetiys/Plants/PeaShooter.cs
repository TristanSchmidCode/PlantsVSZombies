using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlantsVSZombies;
public class PeaShooter : Plant
{
    public const string Name = "PeaShooter";
    public const int Price = 100;
    public const float TimeOut = 7.5f; 
    private const int damage = 1;
    /// <remarks>Throws <see cref="IndexFullExeption"/> if it couldn't 
    /// add itself to <see cref="AllEntitys.allEntitiyes"/> </remarks>  
    public PeaShooter(PlantSpace space) :
        base(
        health: 4,
        space: space, 
        timer: new Timer(1.5f))
    {
        ChangeImage(new ImageType(Layers.Plant, Name));
    }
    /// <summary>
    /// Creates a new PeaShooter, which is added to <see cref="AllEntitys.allEntitiyes"/>
    /// </summary>
    /// <remarks>Throws <see cref="IndexFullExeption"/> if it couldn't 
    /// add itself to <see cref="AllEntitys.allEntitiyes"/> </remarks> 
    public override void TakeAction()
    {
        if (timer.Check())
            if (AllEntitys.allZombies[space.plantPos.Y].Count != 0)
                _ = new Bullet(Pos + (2,0), damage);
        return;
    }

}

