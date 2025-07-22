using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static PlantsVSZombies.LevelHanderler;



namespace PlantsVSZombies;

public readonly struct LevelType
{
    // a list of levels that have been made
    
    readonly (int, int) value;
    public int Map => value.Item1;
    public int Level => value.Item2;
    public Fight GetLevel()
    {
        return value switch
        {
            (1, 1) => new _1_1_(),
            (1, 2) => new _1_2_(),
            (1, 3) => new _1_3_(),
            _ => throw new Exception(),
        };
    }


    /// <summary>
    /// creates a new <see cref="LevelType"/>
    /// </summary>
    /// <param name="level"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public LevelType((int, int) level)
    {
        this.value = level;
      
    }
    public LevelType(int map, int level) : this((map, level))
    {
        
    }


    public static bool operator ==(LevelType left, LevelType right) => left.value.Equals(right.value);

    public static bool operator !=(LevelType left, LevelType right) => !(left == right);

    public static LevelType operator +(LevelType left, int right)
    {
        if (right > 1 || right < 0)
            throw new NotImplementedException("The right operand must be 1 or 0 right now");
        //means that if there were other worlds, it would cicle into it
        LevelType newLevel = new(left.Map, left.Level + right);
        LevelHanderler.LevelState state = Scene.Instance.LevelHanderler[newLevel];
        if (state != LevelHanderler.LevelState.DoesntExist)
            return newLevel;

        newLevel = new(left.Map +right, 0);

        if (state != LevelHanderler.LevelState.DoesntExist)
            return newLevel;

        return left;
    }
    public bool IsValid() => Scene.Instance.LevelHanderler[this] != LevelHanderler.LevelState.DoesntExist;
    public readonly override bool Equals(object? obj) => obj is LevelType level && this ==level;
    public override int GetHashCode() => base.GetHashCode();
}
public class LevelSelector : Intoractible
{    
    public readonly LevelType level;
    //wether or not youve beaten/unlocked the level
    public override bool BeActedOn<T>(T d)
    {
        if (!Scene.Instance.LevelHanderler.HasUnlocked(level))
            return false;
        if (d is not Pressed)
            return false;
        Scene.Instance.SendMessage(Message.OpenFightMenue, level);
        return true;
    }
    public LevelSelector(Position pos, LevelType level, LevelHanderler.LevelState state) : base(pos)
    {
        Displacement = (0, 2);
        this.level = level;

        switch (state)
        {
            case LevelState.Locked:
                Frame.ChangeImage(Image.GetImage(Layers.Frame, "LevelLocked"), shown);
                break;
            case LevelState.Unlocked:
                Frame.ChangeImage(Image.GetImage(Layers.Frame, "LevelUnlocked"), shown);
                break;
            case LevelState.Compleated:
                Frame.ChangeImage(Image.GetImage(Layers.Frame, "LevelCompleated"), shown);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), "Invalid state for level selector");
        }
    }
}

public class LevelHanderler
{   

    public enum LevelState
    {
        Locked,
        Unlocked,
        Compleated,
        DoesntExist,
    }
    static readonly Dictionary<LevelType, Func<Fight>> LevelToFight 
        = new(){
        [new((1, 1))] = () => new _1_1_(),
        [new((1, 2))] = () => new _1_2_(),
        [new((1, 3))] = () => new _1_3_(),
    };
    public static Fight CreateFight(LevelType level) {
        if (!LevelToFight.ContainsKey(level))
            throw new KeyNotFoundException($"The level {level} does not exist in the level handler.");
        return LevelToFight[level]();
    }

    readonly Dictionary<LevelType, LevelState> levels = [];
    List<LevelSelector> levelsSelectors = [];

    public int Map = 1;
    public bool HasUnlocked(LevelType level) => this[level] == LevelState.Unlocked || this[level] == LevelState.Compleated;
    public void CompleateLevel(LevelType level)
    {
        if (!levels.ContainsKey(level))
            throw new KeyNotFoundException($"The level {level} does not exist in the level handler.");
        levels[level] = LevelState.Compleated;
        if (levels[level + 1] != LevelState.DoesntExist)
            levels[level + 1] =  LevelState.Unlocked;
    }



    public LevelState this[LevelType level]
    {
        get => levels.TryGetValue(level, out var value) ? value : LevelState.DoesntExist;
        set
        {
            if (!levels.ContainsKey(level))
                throw new KeyNotFoundException($"The level {level} does not exist in the level handler.");
            levels[level] = value;
        }
    }
    public LevelHanderler()
    {
        foreach ((var level, var _)in LevelToFight)
            levels.Add(level, LevelState.Locked);
        levels[new((1, 1))] = LevelState.Unlocked;
    }
}
