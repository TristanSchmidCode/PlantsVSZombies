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
    public static int ZombieCount { get; protected set; } = 0;
    readonly static int[] zombieRows = new int[4];
    public static int GetZombieRow(int row)
    {
        if (row < 0 || row >= zombieRows.Length)
            throw new ArgumentOutOfRangeException(nameof(row), "Row must be between 0 and 4");
        return zombieRows[row];
    }

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
        EntityHanderler.Instance.RemoveEntity(this);
        ZombieCount--;
        zombieRows[Hight]--;
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
    public virtual void Summon()
    {
        EntityHanderler.Instance.AddEntity(this);
        ZombieCount++;
        zombieRows[Hight]++;
        _Summon();
    }
    protected virtual void _Summon() { }


    public virtual void TakeAction()
    {
        //if the zombie walks out of bounds, the zombies win
        if (Screen.IsOutOfBounds(Pos - Reach))
        {
            Scene.Instance.SendMessage(Message.EndFight, FightEndState.FightLost);
            return;
        }
        //checks if the the space is infront of it has a plant
        if (Screen.GetPixel(Pos - Reach)[Layers.Plant] is not LayerID layer) {
            //if not, it will move to the left
            MoveBy((-Time.DeltaTime * movementSpeed, 0));
            Hat?.MoveBy((-Time.DeltaTime * movementSpeed, 0));
            return;
        }
        
        if (!attackTimer.Check())
            return;
        
        attackTimer.Reset();
        Attack attack = new(Layers.Plant, damage);
        //if the pixel infront of it is a plant, it will attack it
        if (EntityHanderler.Instance.GetEntity(layer) is not Plant plant)
            throw new InvalidOperationException("Pixel with layer Plant not atributed with ID not atributed to a Plant"); 
        plant.BeActedOn(attack);      
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

