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
        waves.Enqueue(TestWave());
        Waves = waves;
    }
    protected override void End(FightingState state)
    {

        if (Planter.planters[2].PlantName == null && state == FightingState.FightWon)
        {
            FinalMessage = "You unlocked the WallNut Plant";
            Planter.planters[2].ChangePlant(WallNut.Name);
        }

        base.End(state);
    }
    //as the name impliys, this is for testing only.
    Wave TestWave()
    {
        WaveInfo Intermition = new(new List<Zombie>
        {
            new BaseZombie(),
        }, 1);
        WaveInfo k = new(new List<Zombie>
        {
            new BaseZombie(),
        }, 1);
        return new(Intermition, k, true);
    }
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

