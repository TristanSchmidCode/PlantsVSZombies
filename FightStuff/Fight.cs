using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;


namespace PlantsVSZombies;
public abstract class Fight
{
    public static Fight? CurrentFight { get; set; }
    public static event OnFightEnd? FightEnd;    
    public delegate void OnFightEnd();
    public static event EventHandler<bool>? FightPause;    
    public static void StartFight(LevelType level)
    {
        CurrentFight = level.GetLevel();
        Scene.ChangeMenue(MenueType.Fight);
        CurrentFight.Start();
        Planter.ShowAll(); 
    }
    public void Pause(bool pause)
    {
        FightPause!.Invoke(this, pause);
    }
    
    public void EndEarly() => _endEarly = true;
    

    readonly Background _background;    
    readonly LevelType _level;

    bool _endEarly = false;
    public string FinalMessage  = string.Empty;

    protected abstract Queue<Wave> Waves { get; init; }
    protected SunHanderler sunHanderler = new(10);


    public void Start()
    {       
        ChangeBackground(_background);
        Money.SetMoney(100);
        Waves.Peek().Start();
        sunHanderler.Start();
    }
    protected virtual void End(FightingState state)
    {
        FightEnd?.Invoke();
        if (state == FightingState.FightWon)
            foreach (var level in LevelSelector.levels)
                if (level.level == this._level)
                    level.Statê = LevelSelector.State.Compleated;
                else if (level.level == this._level + 1)
                    level.Statê = LevelSelector.State.Unlocked;
    }
    public FightingState ActivateAll()  
    {
        if (_endEarly)
        {
            End(FightingState.FightLost);
            return FightingState.FightLost;
        }

        try
        {
            AllEntitys.ActivateAll();
        }
        catch (FightLostException)
        {
            End(FightingState.FightLost);
            return FightingState.FightLost;
        }
        sunHanderler.TakeAction();
        //wave is null if Waves is empty, meaning player only needs to kill the 
        //remaining zombies to continue
        if (Waves.TryPeek(out Wave? wave) == false) 
        {
            if (AllEntitys.Get<Zombie>().Count == 0) 
            {
                End(FightingState.FightWon);
                return FightingState.FightWon;
            }
        }
        
        FightingState state = wave?.TakeAction() ?? FightingState.InFight;
        if (state == FightingState.WaveEnded)
        {
            Waves.Dequeue();
            Waves.TryPeek(out wave);
            wave?.Start();
        }
        //If the final round is empty, 
        return state;
    }
    protected Fight(LevelType level, Background back)
    {
        this._level = level;
        _background = back;
        Money.AddMoney(100);
    }

    public enum FightingState
    {
        InFight,
        WaveEnded,
        FightWon,
        FightLost,
        FightPaused,
        OutOfFight
    }
}




public class SunHanderler
{
    readonly Timer _timer;
    readonly int _frequency;
    public SunHanderler(int frequency)
    {
        _frequency = frequency;
        _timer = new(NextFreq());
    }
    public void Start() => _timer.Start();
    static Sun25 GetSun() => new();
    int NextFreq() => _frequency + new Random().Next(-_frequency / 20, _frequency / 20);
    public void TakeAction()
    {
        if (_timer.Check())
        {
            GetSun();
            _timer.Check(NextFreq());
        }
    }
}