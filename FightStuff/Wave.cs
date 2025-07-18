using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlantsVSZombies.Fight;

namespace PlantsVSZombies;

public class Wave
{
    WaveInfo _intermition;
    WaveInfo _fullWave;

    readonly Timer _timer;
        
    public void Start()
    {
        _timer.Start();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns> In what stage The <see cref="Wave"/> is in;</returns>
    public FightingState TakeAction()
    {
       
        if (_intermition.zombies.Count != 0)
            return InIntermition();
        else
            return InWave();
    }
    FightingState InIntermition()
    {
        if (_timer.Check())
        {
            (Zombie zombie, float Wait) = _intermition.zombies.Pop();
            zombie.Summon();
            _timer.NewInterval(Wait);
        }
        return FightingState.InFight;
    }

    FightingState InWave()
    {
        if (_timer.Check())
        {
            if (_fullWave.zombies.Count == 0)
                return FightingState.WaveEnded;
            (Zombie zombie, float Wait) = _fullWave.zombies.Pop();
            zombie.Summon();
            _timer.NewInterval(Wait);
        }
        return FightingState.InFight;
    }

    public Wave(WaveInfo InbetweenWave, WaveInfo Wave, bool firstWave)
    {
        this._intermition = InbetweenWave;
        this._fullWave = Wave;
        if (firstWave)
            _timer = new(20);
        else
            _timer = new(0);
        


    }
    public Wave(WaveInfo Wave, bool firstWave) : this(new(), Wave, firstWave) { }

    


}
public struct WaveInfo
{
    const float _waittimeVariation = 0.2f;
    public Stack<(Zombie, float)> zombies;
    public static implicit operator Stack<(Zombie, float)>(WaveInfo d) => d.zombies;
    
    public WaveInfo()
    {
        zombies = new();
    }

    /// <summary>
    /// Flips the order of the zombies before putting them in the stack
    /// </summary>
    /// <param name="zombies"></param>
    /// <param name="waitTime"></param>
    public WaveInfo(List<Zombie> zombies, float waitTime)
    {
        List<(Zombie, float)> builder = [];
        
        zombies.Reverse();
        foreach (Zombie zombie in zombies)
        {
            float veriation = waitTime * _waittimeVariation;
            float Variation = veriation * (float)new Random().Next(-100, 100) / 100f;
            builder.Add((zombie, waitTime + Variation));
        }

        this.zombies = new(builder);
    }
    /// <summary>
    /// Shuffels the zombies given before placing them in the stack
    /// </summary>
    public WaveInfo(Zombie[] zombies, float waitTime)
    {
        List<(Zombie, float)> builder = [];
        new Random().Shuffle(zombies);
        foreach (Zombie zombie in zombies)
        {
            float veriation = waitTime * _waittimeVariation;
            float Variation = veriation * (float)new Random().Next(-100, 100) / 100f;
            builder.Add((zombie, waitTime + Variation));
        }

        this.zombies = new(builder);
    }
}

