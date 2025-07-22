using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlantsVSZombies.PlantSpace;

namespace PlantsVSZombies;

public class PlantSpace((int x, int y) plantPos) : Intoractible(
    pos: plantPos.PlantPosition(), 
    fromCenter: (0,0),
    layer: Layers.PlantSpace, 
    symol: null,
    frame: MakeImage(plantPos))
{
    static Image MakeImage((int x, int y) plantPos)
    {
        (Position,PixelType)[] building = new (Position, PixelType)[15 * 7];
        for (int x = 0; x < 15; x++) {
        for (int y = 0; y < 7; y++)
        {
            ColorType green = GradientType.ShadeRandom(new(Colors.Green), 15);
            PixelType pixel;
            if ((plantPos.x + plantPos.y) % 2 == 0)
                green = GradientType.ShadeRandom(new ColorType(0, 225, 0), 15);
            else
                green = GradientType.ShadeRandom(new ColorType(0, 255, 0), 15);
            Random rnd = new();
            if (rnd.Next(0, 100) == 0)
            {
                ColorType other;
                if (rnd.Next(0, 2) == 1)
                    other = new(200, 50, 0);
                else
                    other = Colors.Yellow;
                char[] symbols = ['●', '^'];
                pixel = new(green, other, symbols[rnd.Next(0, 2)]);
            }
            else
                pixel = new(green, Colors.Black, ' ');
            building[y * 15 + x] = (new(x-7,y-5), pixel);            
        }
        }
        return new Image(building);
    }
    public Plant? Plant { get; set; }
    public readonly Position plantPos = plantPos;
    public void RemovePlant() {
        Plant? plant = Plant;
        Plant = null; 
        Plant?.Destroy();
    }
    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed) {
            var planter = PlantHanderler.Instance.SelectedPlanter;
            return PlantThis(planter);
        }
        throw new NotImplementedException();
    }
    /// <summary>
    ///  Plants a plant in this plant spaces position
    /// </summary>
    /// <returns> true if sucsessfull, else false</returns>
    bool PlantThis(Planter planter)
    {
        if (Plant != null)
            return false;
        if (!planter.CanPlant())
            return false;

        Plant = PlantHanderler.CreatePlant(planter.PlantName!, this);
        Money.Instance.RemoveMoney(planter.Price);

        planter.HasPlanted();
        return true;
    }
    public override void Destroy()
    {
        RemovePlant();
        base.Destroy();
    }
}
