using PlantsVSZombies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public abstract class Zombie : ImageHaver, IEntity
{
    protected virtual int Health
    {
        get { return health; }
        set
        {
            health = value;
        }
    }

    protected int health;

    

    protected Hats? Hat { get; set; }
    protected Timer attackTimer;
    //the postion a hat would be fron its center
    public Position Displacement { get; set; } = (0, -2);
    //distence it attacks from
    protected Position Reach { get; set; } = (2, 0);
    protected int damage;
    protected float movementSpeed;
    public int Hight { get; private set; }

    
    public virtual void TakeDamage(int damage)
    {
        bool dead = false;
        health -= Hat?.TakeDamage(damage, out dead) ?? damage;
        if (dead)
            Hat = null;
        
        if (health < 0)
            Destroy();
    }
    public void Destroy()
    {
        AllEntitys.RemoveEntities(this);
        RemoveImage();
    }

    
    protected Zombie(
        float movementSpeed, 
        Layers layer,
        int pos,
        int health,
        float attackSpeed,
        int damage)
        : base(layer, (BordInfo.MapLeft - 5, BordInfo.rows[pos]-1))
    {
        this.Hight = pos;
        this.Health = health;
        this.movementSpeed = movementSpeed;
        this.damage = damage;
        attackTimer = new(attackSpeed);
    }
    public abstract void Summon();



    /// <returns>Returns true if Zombies won</returns>
    public virtual void TakeAction()
    {
        //if there is a plant infront of it, it will attack it. 
        try
        {
            //checks if the the space is infront of it has a plant
            if (Screen.GetPixel(Pos - Reach)[Layers.Plant] is LayerID layer)
            {
                if (attackTimer.Check())
                {
                    attackTimer.Reset();
                    Attack attack = new(Layers.Plant, damage);
                    AllEntitys.Get<Plant>()[layer].BeActedOn(attack);
                }
                return;
            }
        }catch (IndexOutOfRangeException)
        { throw new FightLostException(); }

        MoveBy((-Time.DeltaTime * movementSpeed, 0));
        Hat?.MoveBy((-Time.DeltaTime * movementSpeed, 0));
    }
    
    public bool BeActedOn<T>(T d) where T : IAction
    {
        if (d is Attack attack)
        {
            TakeDamage(attack.damage);
        }
        return true;
    }
}

