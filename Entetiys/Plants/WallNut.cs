using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class WallNut: Plant
{
    public static readonly PlantData WallNutData = new(
     type: typeof(WallNut),
     name: "WallNut",
     health: 40,
     price: 50,
     timeOut: 20f
 );
    public override PlantData Data => WallNutData;


    public WallNut(PlantSpace space) : base(space)
    {
        Health = 40;
        ChangeImage(Image.GetImage(Layers.Plant, Data.Name));
    }

    public override void TakeAction() { return; }
}
