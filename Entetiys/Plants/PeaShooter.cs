using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlantsVSZombies;
public class PeaShooter : Plant
{
    public static readonly PlantData PeashooterData = new PlantData(
        type: typeof(PeaShooter),
        name: "PeaShooter",
        health: 5,
        price: 100, 5f);
    public override PlantData Data => PeashooterData;

    const int damage = 1;
    const float attackInterval = 1.5f;
    protected Timer timer;
    public PeaShooter(PlantSpace space) : base(space)
    {
        ChangeImage(Image.GetImage(Layers.Plant, Data.Name));

        timer = new(attackInterval);
        timer.Start();
    }
    public override void TakeAction()
    {
        if (timer.Check())
            if (Zombie.GetZombieRow(space.plantPos.Y) != 0)
                _ = new Bullet(Pos + (2, 0), damage);
        return;
    }

}

