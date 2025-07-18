using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

internal class _1_2_: Fight
{

    protected override Queue<Wave> Waves { get; init; }
    public _1_2_() : base(new((1, 2)), Background.grassFight)
    {
        Queue<Wave> waves = new();  
        waves.Enqueue(Wave1());
        waves.Enqueue(FinalWave());
        Waves = waves;
    }
    
    Wave Wave1()
    {
        WaveInfo Intermition = new(new List<Zombie>
        {
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(new Cone()),
            new BaseZombie(),
        }, 20);
        WaveInfo Wave = new(new Zombie[] {
            new BaseZombie(),
            new BaseZombie(new Cone()),
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(),

            new FatZombie(),
        }, 0.5f);
        Wave Wave1 = new(Intermition, Wave, true);
        return Wave1;
    }
    Wave FinalWave()
    {
        WaveInfo Intermition = new(new Zombie[] 
        {
            new BaseZombie(),
            new BaseZombie(new Cone()),
            new FatZombie(),

            new BaseZombie(),
            new BaseZombie(new Cone()),
            new FatZombie(),
            new BaseZombie(),
        }, 8);
        
        WaveInfo Wave = new( new Zombie[] {
            new BaseZombie(),
            new BaseZombie(),
            new BaseZombie(new Cone()),

            new BaseZombie(new Cone()),
            new BaseZombie(),
            new FatZombie(new Cone()),
            new FatZombie(new Cone()),
            new FatZombie(new Cone()),
            new FatZombie(),
        }, 0.5f);


        Wave Wave1 = new(Intermition, Wave, true);
        return Wave1;
    }

}
