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
    public const string Name = "SunFlower";
    public const int Price = 50;
    public const float TimeOut = 7.5f;
    public SunFlower(PlantSpace space) : 
        
        base(
        health: 6, 
        space: space,
        timer: new Timer(7)
        )
    {
        ChangeImage(new ImageType(Layers.Plant, Name));
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
