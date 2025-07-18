using PlantsVSZombies;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public abstract class Plant : ImageHaver, IEntity
{
    protected int Health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health < 0)
                Destroy();
        }

    }
    
    int _health;
    protected PlantSpace space;
    protected Timer timer;
    public void Destroy()
    {
        AllEntitys.RemoveEntities(this);
        space.RemovePlant();
        RemoveImage();
        
    }

    /// <param name="plantPos"></param>
    /// <remarks>Throws <see cref="IndexFullExeption"/> if it couldn't add itself to <see cref="AllEntitys.allEntitiyes"/> </remarks>  
    protected Plant(
        int health,
        PlantSpace space, 
        Timer timer
        ) : base(Layers.Plant, space.plantPos.Pos.PlantPosition())
    {
        this._health = health;
        this.space = space;
        this.timer = timer;
        timer.Start();
        
        
        //If this is thrown and breaks, the issue is your debug settings, and not the game, as this is always handeled
        if (AllEntitys.AddEntities(this) == false)
            throw new IndexFullExeption();
    }

    public abstract void TakeAction();
    public bool BeActedOn<T>(T d) where T : IAction
    {
        if (d is Attack attack)
        {
            Health -= attack.damage;
        }
        return true;
    }
}

