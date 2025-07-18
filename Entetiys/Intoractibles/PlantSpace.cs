using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class PlantSpace : Intoractible
{
    static void Scene_OnMenueChange(MenueType menue)
    {
        if (menue == MenueType.Fight)
        {
            if (spaces[0,0] is null)
                for (int i = 0; i < spaces.GetLength(0); i++)
                    for (int j = 0; j < spaces.GetLength(1); j++)
                        spaces[i, j] = new((i, j));
            else
                for (int i = 0; i < spaces.GetLength(0); i++)
                    for (int j = 0; j < spaces.GetLength(1); j++) 
                        spaces[i, j] = new((i, j));
            ShowAll();
        }
        else if (menue == MenueType.FightSettings)
        {
            HideAll();
        }
    }
    static void Fight_OnFightEnd()
    {
        foreach (PlantSpace? space in spaces)
        {
            space?.Destroy();
        }
        HideAll();
    }
    public static void Nothing() { }

    static readonly PlantSpace[,] spaces = new PlantSpace[9, 4];
    static void ShowAll()
    {
        foreach (var space in spaces)
            space?.Show();
        
    }
    static void HideAll()
    {
        foreach (var space in spaces)
            space?.Hide();  
    }
    static PlantSpace()
    {
        Scene.OnMenueChange += Scene_OnMenueChange;
        Fight.FightEnd += Fight_OnFightEnd;
    }

   
    public Plant? Plant { get; private set; }
    public readonly Position plantPos;
    public void RemovePlant()
    {
        Plant = null;
    }
    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed pres)
        {
            if (pres.situation == MenueType.Fight)
            {
                return PlantThis(); 
            }
            else
            {
                return false;
            }
        }
        throw new NotImplementedException();
    }
    /// <summary>
    ///  Plants a plant in this plant spaces position
    /// </summary>
    /// <returns> true if sucsessfull, else false</returns>
    bool PlantThis()
    {
        if (Plant != null)
            return false;
        if (Planter.SelPlanter.PlantName == null)
            return false;

        if (Planter.SelPlanter.GetPlant() is not string plantName)
            return false;

        if (plantName == "PeaShooter")
        {
            Plant = new PeaShooter(this);
            return true;
        }
        else if (plantName == "SunFlower")
        {
            Plant = new SunFlower(this);
            return true;
        }
        else if (plantName == "WallNut")
        {
            Plant = new WallNut(this);
            return true;
        }
        return false;
    }
    public override void TakeAction()
    {
        throw new NotImplementedException();
    }
    public override void Destroy()
    {
        base.Destroy();
        Fight.FightEnd -= RemovePlant;
    }
    PlantSpace((int, int) plantPos) : base(plantPos.PlantPosition())
    {
        this.plantPos = plantPos;
        Fight.FightEnd += RemovePlant;
    }

}
