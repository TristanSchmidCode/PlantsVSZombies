using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using static PlantsVSZombies.Fight;

namespace PlantsVSZombies;
/// <summary>
/// A static class that contains refrences to all entities currently active and events
/// that are relivant
/// </summary>
public static class AllEntitys
{
    /// <summary>
    ///  Trys to add an entity to all entitieys and its respective entity container
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="entity"> </param>
    /// <returns>
    /// Returns <see cref="true"/> if Succeeded to add <see cref="IEntity"/> <see cref="allEntitiyes"/> 
    /// </returns>
    public static bool AddEntities<T>(T entity) where T : IEntity
    {
        Activate += entity.TakeAction;
        if (entity is Plant plant)
        {
            bool ddd = (allEntitiyes.TryAdd(plant.Layer, plant) &&
                allPlants.TryAdd(plant.Layer, plant));
            if (ddd)
            {
                FightEnd += plant.Destroy;
            }
            return ddd;
        }
        if (entity is Zombie zombie)
        {
            bool ddd = (allEntitiyes.TryAdd(zombie.Layer, zombie) &&
                allZombies[zombie.Hight].TryAdd(zombie.Layer, zombie) &&
                allZombies2.TryAdd(zombie.Layer, zombie));
            if (ddd)
            {
                FightEnd += zombie.Destroy;
            }
            return ddd;
        }
        if (entity is Bullet bullet)
        {
            bool ddd = (allEntitiyes.TryAdd(bullet.Layer, bullet) &&
                allBullets.TryAdd(bullet.Layer, bullet));
            if (ddd)
            {
                FightEnd += bullet.Destroy;
            }
            return ddd;
        }
        if (entity is Sun sun)
        {
            bool ddd = (allEntitiyes.TryAdd(sun.Layer, sun) &&
                allSuns.TryAdd(sun.Layer, sun));
            if (ddd)
            {
                FightEnd += sun.Destroy;
            }
            return ddd;

        }
        if (entity is Intoractible intor)
        {
            return
               (allEntitiyes.TryAdd(intor.Layer, intor) &&
               allIntoractibles.TryAdd(intor.Layer, intor));

        }
        return false;
    }
    /// <summary>
    ///  Trys to Remove an entity from all entitieys and its respective entity container
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <param name="entity"> </param>
    /// <returns>
    /// Returns <see cref="true"/> if Succeeded to remove <see cref="IEntity"/> <see cref="allEntitiyes"/> 
    /// </returns>
    public static bool RemoveEntities<T>(T entity) where T : IEntity
    {
        Activate -= entity.TakeAction;
        if (entity is Plant plant)
        {
            bool ddd = (allEntitiyes.Remove(plant.Layer) &&
                allPlants.Remove(plant.Layer));
            if (ddd)
            {
                FightEnd -= plant.Destroy;
            }
            return ddd;

        }
        if (entity is Zombie zombie)
        {
            bool ddd = (allEntitiyes.Remove(zombie.Layer) &&
               allZombies[zombie.Hight].Remove(zombie.Layer) && allZombies2.Remove(zombie.Layer));
            if (ddd)
            {
                FightEnd -= zombie.Destroy;
            }
            return ddd;
        }
        if (entity is Bullet bullet)
        {
            bool ddd = (allEntitiyes.Remove(bullet.Layer) &&
                allBullets.Remove(bullet.Layer));
            if (ddd)
            {
                FightEnd -= bullet.Destroy;
            }
            return ddd;

        }
        if (entity is Sun sun)
        {
            bool ddd = (allEntitiyes.Remove(sun.Layer) &&
                allSuns.Remove(sun.Layer));
            if (ddd)
            {
                FightEnd -= sun.Destroy;
            }
            return ddd;

        }
        if (entity is Intoractible intor)
        {
            return
               (allEntitiyes.Remove(intor.Layer) &&
               allIntoractibles.Remove(intor.Layer));
        }
        return false;
    }
    public static ReadOnlyDictionary<LayerID, IEntity> Getall()
    {
        ReadOnlyDictionary<LayerID, IEntity> D = new(allEntitiyes);
        return D;
    }
    public static ReadOnlyDictionary<LayerID, TEntity> Get<TEntity>()
    {
        if (allPlants is Dictionary<LayerID, TEntity> plants)
        {
            return new(plants);
        }
        if (allSuns is Dictionary<LayerID, TEntity> suns)
        {
            return new(suns);
        }
        if (allZombies2 is Dictionary<LayerID, TEntity> zombies)
        {
            return new(zombies);
        }
        if (allBullets is Dictionary<LayerID, TEntity> bullets)
        {
            return new(bullets);
        }
        if (allIntoractibles is Dictionary<LayerID, TEntity> intor)
        {
            return new(intor);
        }
        throw new Exception();
    }
    public static void ActivateAll() => Activate?.Invoke();
    
    static event OnActivate? Activate;
    delegate void OnActivate();

    static readonly Dictionary<LayerID, IEntity> allEntitiyes = [];
    static readonly Dictionary<LayerID, Sun> allSuns = [];
    static readonly Dictionary<LayerID, Zombie> allZombies2 = [];
    static public readonly Dictionary<LayerID, Zombie>[] allZombies = [[], [], [], []];
    static readonly Dictionary<LayerID,  Plant> allPlants = [];
    static readonly Dictionary<LayerID, Bullet> allBullets = [];
    static readonly Dictionary<LayerID, Intoractible> allIntoractibles = [];
}
    
