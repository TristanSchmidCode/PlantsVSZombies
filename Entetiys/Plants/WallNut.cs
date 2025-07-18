using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class WallNut: Plant
{
    public const string Name = "WallNut";
    public const int Price = 50;
    public const float TimeOut = 20;

    public WallNut(PlantSpace space) 
        : base(
        health: 40,
        space: space,
        timer: new Timer(0))
    {
        Health = 40;
        ChangeImage(new ImageType(Layers.Plant, Name));
    }

    public override void TakeAction()
    {
        return;
    }
}
