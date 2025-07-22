using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PlantsVSZombies;

internal class SunFlower: Plant
{
    public static readonly PlantData SunFlowerData = new(
        type: typeof(SunFlower),
        name: "SunFlower",
        health: 6,
        price: 50,
        timeOut: 7.5f
    );
    public override PlantData Data => SunFlowerData;
    const float SunGenerateInterval = 7;

    Timer timer;

    public SunFlower(PlantSpace space) : base(space)
    {
        ChangeImage(Image.GetImage(Layers.Plant, Data.Name));
        timer = new(SunGenerateInterval);
        timer.Start();
    }
    /// <summary>
    /// Checks if it needs to create a sun or not, Makes Sun25
    /// </summary>
    public override void TakeAction()
    {
        if (timer.Check())
        {
            _ = new Sun25(Pos);
            timer.NewInterval(24);
        }
        return;
    }
   
}
