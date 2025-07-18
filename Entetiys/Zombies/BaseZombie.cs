using PlantsVSZombies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class BaseZombie(int pos) : Zombie(
    movementSpeed: 2.5f,
    attackSpeed: 1,
    layer: Layers.BZombie,
    pos: pos,
    health: 10,
    damage: 1)
{
    public override void Summon()
    {
        AllEntitys.AddEntities(this);
        
        attackTimer.Start();
        ChangeImage(new ImageType(Layers.BZombie, "BaseZombie", 10));

        Hat?.MoveTo(Pos + Displacement);
        Hat?.Summon();
    }
    /// <summary>
    ///  Creates a zombie in a random row
    /// </summary>
    public BaseZombie() : this(new Random().Next(0, 4)) { }

    public BaseZombie(Hats hat) : this() => Hat = hat;
}
