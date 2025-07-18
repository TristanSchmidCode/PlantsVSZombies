using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;


public abstract class Hats : ImageHaver, IEntity
{
    protected int armour;
    protected Hats((int,int) pos, int armour) : base(Layers.Special, pos)
    {
        this.armour = armour;
    }

    public abstract void Summon();
    /// <summary>
    ///  Removes the damage from its armour, and does any image 
    ///  changes that might result
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="dead">Is true if the Hat should be removed</param>
    /// <returns> The remaining damage if any</returns>
    public abstract int TakeDamage(int damage, out bool dead);
    
    //if there is a falling off animation it will run it
    public virtual void FallOff() 
    {
        Destroy();
    }
    public bool BeActedOn<T>(T d) where T : IAction
    {
        return false;
    }

    public void Destroy()
    {
        RemoveImage();
    }

    public void TakeAction()
    {
        throw new NotImplementedException();
    }
}

public class Cone(Position pos) : Hats(pos, 15)
{
  
    public override int TakeDamage(int damage, out bool dead)
    {
        if (armour >= 7)
        {
            armour -= damage;
            if (armour < 7)
                ChangeImage(new ImageType(Layers.Special, "DamagedCone"));
        }
        else
            armour -= damage;
        //if there is damage "left over", returns that left over amount
        if (armour < 0)
        {
            FallOff();
            dead = true;
            return armour * -1;
        }

        dead = false;
        return 0;
    }
    public override void Summon()
    {
        ChangeImage(new ImageType(Layers.Special, "Cone"));
        PrintImage(false);
    }
    public Cone() : this((0, 0)) { }
}
public class Bucket(Position pos): Hats(pos, 65)
{
    public override int TakeDamage(int damage, out bool dead)
    {
        int damagePhase1 = 40;
        int damagePhase2 = 20;
        
        if (armour >= damagePhase1)
        {
            armour -= damage;
            if (armour < damagePhase1)
                ChangeImage(new ImageType(Layers.Special, "DamagedBucket"));
        }
        else if (armour >= damagePhase2)
        {
            armour -= damage;
            if (armour < damagePhase2)
                ChangeImage(new ImageType(Layers.Special, "BadlyDamagedBucket"));
        }
        else
            armour -= damage;
        //if there is damage "left over", returns that left over amount
        if (armour < 0)
        {
            FallOff();
            dead = true;
            return armour * -1;
        }

        dead = false;
        return 0;
    }
    public override void Summon()
    {
        ChangeImage(new ImageType(Layers.Special, "Bucket"));
        PrintImage(false);
    }
    public Bucket() : this((0, 0)) { }
}
