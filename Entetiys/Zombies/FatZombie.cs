
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class FatZombie(int hight) : Zombie(
    movementSpeed: 2.3f,
    attackSpeed: 1,
    layer: Layers.BZombie, 
    pos: hight, 
    health: 15,
    damage: 1)
{ 
    public override void TakeDamage(int damage)
    {
        //If the monster has armour
        bool hatDead = true;
        
        int remainingDamage = Hat?.TakeDamage(damage, out hatDead) ?? damage;
        if (hatDead)
            Hat = null;
        
        if (Health >=6 & Health-remainingDamage < 6)
            ChangeImage(new ImageType(Layers.BZombie, "BloodyFatZombie"));
         
        Health -= remainingDamage;

        if (Health < 0)
            Destroy();
        
        
    }    
    public override void Summon()
    {
        attackTimer.Start();
        AllEntitys.AddEntities(this);
        ChangeImage(new ImageType(Layers.BZombie,"FatZombie",10));
        Hat?.MoveTo(Pos + Displacement);
        Hat?.Summon();

    }
    public FatZombie() : this(new Random().Next(0,4)) { }
    public FatZombie(Hats hat) : this()
    {
        Hat = hat;
    }
}

