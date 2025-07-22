using System.Collections;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;


namespace PlantsVSZombies;

public abstract class Fight : SingleTone<Fight>
{
    
    public event EventHandler<bool>? FightPause;    
    public void Pause(bool pause)
    {
        FightPause!.Invoke(this, pause);
    }
    public void Start()
    {
        sunHanderler.Start();
        Waves.Peek().Start();
    }    

    readonly Background _background;    
    readonly LevelType _level;

    public string FinalMessage  = string.Empty;

    protected abstract Queue<Wave> Waves { get; init; }
    protected SunHanderler sunHanderler;

    public virtual void End(bool fightWon)
    {
        if (fightWon)
            Scene.Instance.LevelHanderler.CompleateLevel(_level);
    }
    public void ActivateAll()  
    {
        EntityHanderler.Instance.ActivateAll();

        sunHanderler.TakeAction();
        //wave is null if Waves is empty, meaning player only needs to kill the 
        //remaining zombies to continue
        if (Waves.TryPeek(out Wave? wave) == false) 
            if (Zombie.ZombieCount == 0) 
                Scene.Instance.SendMessage(Message.EndFight, FightEndState.FightWon);
        if (wave?.TakeAction() ?? false)
        {
            Waves.Dequeue();
            Waves.TryPeek(out wave);
            wave?.Start();
        }
    }
    protected Fight(LevelType level, Background back)
    {
        this._level = level;
        _background = back;
        sunHanderler = new(frequency: 10);
    }
    
    protected override void DestroyThis()
    {
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