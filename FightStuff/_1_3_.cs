using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class _1_3_ : Fight
{
    protected override Queue<Wave> Waves { get; init; }
    public _1_3_(): base(new((1,3)),Background.grassFight)
    {
        Queue<Wave> waves = new();
        waves.Enqueue(Wave1());
        waves.Enqueue(FinalWave());
        
        Waves = waves;
    }

    static Wave Wave1()
    {
        WaveInfo Intermition = new(new List<Zombie>
        {
            new BaseZombie(),
            new BaseZombie(),
            new FatZombie(),
            new BaseZombie(),
            new BaseZombie(new Cone()),
            new BaseZombie(),
            new BaseZombie(new Cone()),
        }, 20);
        WaveInfo Wave = new(new Zombie[] {
            new BaseZombie(),
            new FatZombie(new Cone()),
            new FatZombie(),
            new BaseZombie(new Bucket()),
            new BaseZombie(),
            new FatZombie(),
            new FatZombie(),
            new FatZombie(),
        }, 0.5f);
        Wave Wave1 = new(Intermition, Wave, true);
        return Wave1;
    }

    static Wave FinalWave()
    {
        WaveInfo Intermition = new(new Zombie[]
        {
            new BaseZombie(),
            new BaseZombie(new Cone()),
            new FatZombie(),

            new BaseZombie(new Bucket()),
            new BaseZombie(new Cone()),
            new FatZombie(),
            new BaseZombie(),
        }, 8);

        WaveInfo Wave = new(new Zombie[] {
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(new Cone()),
            new FatZombie(),
            new FatZombie(),
            new FatZombie(),
            new BaseZombie(new Cone()),
            new BaseZombie(new Bucket()),
            new FatZombie(new Cone()),
            new FatZombie(new Bucket()),
            new FatZombie(new Cone()),
            new FatZombie(),
        }, 0.5f);


        Wave Wave1 = new(Intermition, Wave, true);
        return Wave1;
    }
}
