using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class _1_1_ : Fight
{

    protected override Queue<Wave> Waves { get; init; }
    public _1_1_() : base(new((1, 1)), Background.grassFight)
    {
        Queue<Wave> waves = new();
        waves.Enqueue(FinalWave());
        Waves = waves;
    }
    public override void End(bool fightWon)
    {
        if (fightWon)
            SaveState.UnlockPlant("WallNut");
        base.End(fightWon);
    }
    //as the name impliys, this is for testing only.

    Wave FinalWave()
    {
        WaveInfo Intermition = new( new List<Zombie>
        {
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
        }, 20);
        Zombie[] waveZombies = [

            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new FatZombie(),

        ];
        new Random().Shuffle(waveZombies);
        WaveInfo Wave = new(waveZombies, 1f);

        Wave Wave1 = new(Intermition, Wave, true);
        return Wave1;
    }
}

