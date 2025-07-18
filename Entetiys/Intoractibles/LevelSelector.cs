using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



namespace PlantsVSZombies;

public readonly struct LevelType
{
    // a list of levels that have been made
    public static readonly Position[] CompleatedLevels =[
        (1,1),
        (1,2),
        (1,3),
        ];
    public Fight GetLevel()
    {
        return level switch
        {
            (1, 1) => new _1_1_(),
            (1, 2) => new _1_2_(),
            (1, 3) => new _1_3_(),

            _ => throw new Exception(),
        } ;
    }
    public readonly (int,int) level;


    /// <summary>
    /// creates a new <see cref="LevelType"/>
    /// Throws <see cref="ArgumentOutOfRangeException"/> if the level doesn't exist
    /// </summary>
    /// <param name="level"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public LevelType((int,int) level)
    {
        if (CompleatedLevels.Contains(level))
            this.level = level;
        else
            throw new ArgumentOutOfRangeException(nameof(level),"The level has not been made");
    }
    

    public static bool operator ==(LevelType left, LevelType right)
    {
        if (left.level.Equals(right.level))
            return true;
        else
            return false;
    }
    public static bool operator !=(LevelType left, LevelType right)
    {
        return !(left == right);
    }
    public static LevelType operator +(LevelType left, int right)
    {
        //means that if there were other worlds, it would cicle into it
        (int,int) d = left.level;
        if (CompleatedLevels.Contains(d.Add((0, right))))
            return new(d.Add((0, 1)));
        else if (CompleatedLevels.Contains((d.Item1 + 1, 1)))
            return new((d.Item1 + 1, 1));

        return left;
        
    }
    public override bool Equals(object? obj)
    {
        if (obj is LevelType level)
        {
            return (level == this);
        }
        else
            return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
internal class LevelSelector : Intoractible
{
    public enum State
    {
        Locked,
        Unlocked,
        Compleated,
    }

    static public readonly List<LevelSelector> levels;
    static public int Map { get; private set; }

    static void OnMenueChange(MenueType menue)
    {
        if (menue == MenueType.LevelSelect)
        {
            ShowAll();
            Screen.CreatLine(levels[0].frame.Pos, levels[1].frame.Pos, new(Colors.OddArmour));
            Screen.CreatLine(levels[1].frame.Pos, levels[2].frame.Pos, new(Colors.OddArmour));
            
        }
        else
            HideAll();
    }
    static void ShowAll()
    {
        foreach (var level in levels)
        {
            if (level.level.level.Item1 == Map)
                level.Show();
            else
                level.Hide();
        }
    }
    static void HideAll()
    {
        foreach (var level in levels)
        {
            level.Hide();
        }
    }

    static public void Nothing() { }
    static LevelSelector()
    {
        Map = 1;
        //initialises the levels in their correct Positions
        levels = [
            new( (30, 15), new( (1, 1) )),
            new( (50, 30), new( (1, 2) )),
            new( (55, 20), new( (1, 3) )),
        ];

        Scene.OnMenueChange += OnMenueChange;
        //Throws this if there are levels that arn't visible;
        if (levels.Count != LevelType.CompleatedLevels.Length)
            throw new NotImplementedException();
    }


    public readonly LevelType level;
    //wether or not youve beaten/unlocked the level
    State state;

    public State Statê 
    { 
        get
        {
            return state;
        }
        set
        {
            //changes the image of the levelselctor depending on wether youve beaten it or not
            switch (value)
            {
                case State.Locked:
                    frame.ChangeImage(new ImageType(Layers.Frame, "LevelLocked"), shown);
                    break;
                case State.Unlocked:
                    frame.ChangeImage(new ImageType(Layers.Frame, "LevelUnlocked"), shown);
                    break;
                case State.Compleated:
                    frame.ChangeImage(new ImageType(Layers.Frame, "LevelCompleated"), shown);
                    break;          
            }
            state = value;
        } 
    }
        
    public override void TakeAction()
    {    }
    public override bool BeActedOn<T>(T d)
    {
        if (Statê != State.Locked)
            if (d is Pressed)
            {
                Fight.StartFight(level);
                return true;
            }
        return false;
    }
    public LevelSelector(Position pos, LevelType level) : base(pos)
    {
        Displacement = (0, 2);
        this.level = level;
        //unlockes it if its the first level;
        if (level.level.Equals((1, 1)))
            Statê = State.Unlocked;
        else 
            Statê = State.Locked;
    }
}
