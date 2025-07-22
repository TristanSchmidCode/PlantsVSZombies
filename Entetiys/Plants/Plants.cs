using PlantsVSZombies;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PlantsVSZombies;
public class PlantData(Type type, string name, int health, int price, float timeOut)
{
    public Type Type { get; } = type;
    public string Name { get; } = name;
    public int Health { get; } = health; 
    public int Price { get; } = price;
    public float TimeOut { get; } = timeOut;
}

public abstract class SingleTone<T> where T : SingleTone<T>
{
    static T? instance;
    public static T Instance {
        get => instance ?? throw new Exception("instance cant be null");
        private set => instance = value;
    }
    public static bool IsCreated => instance != null;
    public static bool TryGetInstance(out T instance)
    {
        instance = SingleTone<T>.instance!;
        return instance != null;
    }

    protected SingleTone()
    {
        if (instance != null)
            throw new InvalidOperationException($"There is already an instance of {typeof(T).Name}.");
        if (!typeof(T).IsAssignableFrom(GetType()))
            throw new InvalidOperationException("Wrong type");
        instance = (T)this;
    }
    public void Destroy()
    {        
        DestroyThis();
        instance = null;
    }
    protected abstract void DestroyThis();

}
public static class SaveState
{
    static readonly List<string> unlockedPlants = [

            PeaShooter.PeashooterData.Name,
            SunFlower.SunFlowerData.Name
        ];


    public static void UnlockPlant(string plantName)
    {
        if (unlockedPlants.Contains(plantName))
            return;
        unlockedPlants.Add(plantName);
    }
    public static ReadOnlyCollection<string> GetUnlockedPlants() => new ReadOnlyCollection<string>(unlockedPlants);
    public static bool PlantIsUnlocked(string plantName)
    {
        if (plantName is null)
            throw new ArgumentNullException(nameof(plantName), "Plant name cannot be null");
        return unlockedPlants.Contains(plantName);
    }
}

public class PlantHanderler : SingleTone<PlantHanderler>
{
    static readonly Dictionary<Type, PlantData> plantData = [];
    static readonly Dictionary<string, Func<PlantSpace, Plant>> plantConstructors = [];
    static readonly Dictionary<string, Type> plantTypes = [];
   

    static void AddPlantEntry(PlantData data)
    {
        if (plantTypes.ContainsKey(data.Name))
            throw new ArgumentException($"Plant with name {data.Name} already exists.", nameof(data));
        if (plantData.ContainsKey(data.Type))
            throw new ArgumentException($"Plant with type {data.Type.Name} already exists.", nameof(data));
        plantData.Add(data.Type, data);
        plantTypes.Add(data.Name, data.Type);

        ConstructorInfo constructor = data.Type.GetConstructor([typeof(PlantSpace)])
           ?? throw new ArgumentException($"Type {data.Type.Name} must have a constructor with one PlantSpace argument.");

        ParameterExpression param = Expression.Parameter(typeof(PlantSpace), "space");
        NewExpression newExpr = Expression.New(constructor, param);
        Func<PlantSpace, Plant> Function = Expression.Lambda<Func<PlantSpace, Plant>>(newExpr, param).Compile();

        plantConstructors.Add(data.Name, Function);
    }
    public static PlantData GetPlantData(Type type)
    {
        if (!plantData.TryGetValue(type, out PlantData? data))
            throw new ArgumentOutOfRangeException(nameof(type), "Wasn't a valid plant type");
        return data;
    }
    public static Type TypeFromName(string plantName)
    {
        if (!plantTypes.TryGetValue(plantName, out Type? type))        
            throw new ArgumentOutOfRangeException(nameof(plantName), "Wasn't a valid plant name");
        return type;
        
    }
    public static PlantData GetPlantData(string plantName)
    {
        Type type = TypeFromName(plantName);
        return GetPlantData(type);
    }
    public static Plant CreatePlant(string plantName, PlantSpace space)
    {
        if (!plantConstructors.TryGetValue(plantName, out Func<PlantSpace, Plant>? constructor))
            throw new ArgumentOutOfRangeException(nameof(plantName), "Wasn't a valid plant name");
        return constructor(space);
    }
    static PlantHanderler()
    {
        AddPlantEntry(PeaShooter.PeashooterData);
        AddPlantEntry(SunFlower.SunFlowerData);
        AddPlantEntry(WallNut.WallNutData);
    }


    Planter selectedPlanter;
    public Planter SelectedPlanter
    {
        get => selectedPlanter;
        set
        {
            selectedPlanter.Select(false);
            selectedPlanter = value;
            selectedPlanter.Select(true);
        }
    }
    readonly Planter[] planters = new Planter[5];
    readonly PlantSpace[,] spaces = new PlantSpace[9, 4];
    public Plant MakePlant(string plantName, PlantSpace space)
    {
        if (!SaveState.PlantIsUnlocked(plantName))
            throw new ArgumentOutOfRangeException(nameof(plantName), "Wasn't a valid plant name or not unlocked yet");
        if (!plantConstructors.TryGetValue(plantName, out Func<PlantSpace, Plant>? constructor))
            throw new ArgumentOutOfRangeException(nameof(plantName), "Wasn't a valid plant name");
        Plant plant = constructor(space);
        space.Plant = plant;
        return plant;
    }
   
    public PlantHanderler()
    {
        int[] poses = [5, 13, 21, 29, 37];
        for (int i = 0; i < poses.Length; i++)
            planters[i] = new Planter((BordInfo.LeftBorder / 2, poses[i]));

        var unlockedPlants = SaveState.GetUnlockedPlants();
        if (unlockedPlants.Count > poses.Length)
            throw new NotImplementedException();
        else
        {
            for (int i = 0; i < unlockedPlants.Count; i ++)
                planters[i].ChangePlant(unlockedPlants[i]);
        }
        selectedPlanter = planters[0];



        for (int i = 0; i < spaces.GetLength(0); i++)
            for (int j = 0; j < spaces.GetLength(1); j++)
            {
                PlantSpace planter = new((i, j));
                spaces[i, j] = planter;
            }
    }
    protected override void DestroyThis()
    {
        foreach (var planter in planters)
            planter.Destroy();
        foreach (var plantSpace in spaces)
            plantSpace.Destroy();
    }
    public void SetShow(bool show)
    {
        if (show)
        {
            foreach (var planter in planters)
                planter.Show();
            foreach (var plantSpace in spaces)
                plantSpace.Show();
        }
        else
        {
            foreach (var planter in planters)
                planter.Hide();
            foreach (var plantSpace in spaces)
                plantSpace.Hide();
        }
    }

}

public abstract class Plant : ImageHaver, IEntity
{
    public abstract PlantData Data { get; }

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
    public void Destroy()
    {
        EntityHanderler.Instance.RemoveEntity(this);
        space.RemovePlant();
        RemoveImage();
        
    }

    protected Plant(PlantSpace space) : base(Layers.Plant, space.plantPos.Pos.PlantPosition())
    {
        this.space = space;
        Health = Data.Health;
        EntityHanderler.Instance.AddEntity(this);
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

