using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class Planter : Intoractible
{
    //the planter that is currently active
    public static Planter SelPlanter { get; private set; }
    //stores all planters here
    public static readonly Planter[] planters;
    
    //shows all planters during fights 
    static void OnMenueChange(MenueType menue)
    {
        if (menue == MenueType.Fight)
        {
            ShowAll();
        }
        else
            HideAll();
    }
    public static void ShowAll()
    {
        foreach (Planter planter in planters)
        {
            planter.Show();
        }
    }
    public static void HideAll()
    {
        foreach (var planter in planters)
        {
            planter.Hide();
        }
    }
    static void SetPlant(Planter planter)
    {
        SelPlanter.ChangeFrame();
        SelPlanter = planter;
        SelPlanter.ChangeFrame("SelFrame");
    }
    static float GetTimeOut(string plantName)
    {
        if (plantName == "PeaShooter")
            return PeaShooter.TimeOut;
        if (plantName == "SunFlower")
            return SunFlower.TimeOut;
        if (plantName == "WallNut")
            return WallNut.TimeOut;
        throw new ArgumentOutOfRangeException(nameof(plantName), "Wasn't a valid plant name");
    }
    static int GetPrice(string plantName)
    {
        if (plantName == "PeaShooter")
            return PeaShooter.Price;
        if (plantName == "SunFlower")
            return SunFlower.Price;
        if (plantName == "WallNut")
            return WallNut.Price;
        throw new ArgumentOutOfRangeException(nameof(plantName), "Wasn't a valid plant name");
    }
    static Planter()
    {
        int[] poses = [5, 13, 21, 29, 37];
        planters = [
            new Planter((BordInfo.LeftBorder / 2, poses[0])),
            new Planter((BordInfo.LeftBorder / 2, poses[1])),
            new Planter((BordInfo.LeftBorder / 2, poses[2])),
            new Planter((BordInfo.LeftBorder / 2, poses[3])),
            new Planter((BordInfo.LeftBorder / 2, poses[4])),
        ];
        planters[0].ChangePlant(PeaShooter.Name);
        planters[1].ChangePlant(SunFlower.Name);
        SelPlanter = planters[0];

        Scene.OnMenueChange += OnMenueChange;
    }

    public string? PlantName { get; private set; }
    public int Price { get; private set; }
    //wether or not the image has shadded out as it should be
    public bool Changed = true;
    readonly Timer _timer;
    
    /// <summary>
    /// checks if the this planter has a plant, wether the its on time out, 
    /// and if there's enough money
    /// </summary>
    /// <returns></returns>
    public string? GetPlant()
    {
        if (PlantName == null)
            return null;
        if (!_timer.Check(false))
            return null;
        if (!Money.MinuseMoney(GetPrice(PlantName)))
            return null;
        _timer.NewInterval(GetTimeOut(PlantName));
        _timer.Reset();
        return PlantName;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newPlant"></param>
    /// <returns>True if the change was sucsesfull</returns>
    public bool ChangePlant(string? newPlant)
    {
        if (PlantName == newPlant)
            return false;
        if (newPlant == null)
        {
            Symol.ChangeImage(new ImageType(), shown);
            Price = 0;
            return true;
        }
        PlantName = newPlant;
        Symol.ChangeImage(new ImageType(Layers.Plant, newPlant), shown);

        Price = GetPrice(newPlant);
        return true;
    }
    void ChangeFrame(string? newFrame = "Frame1")
    {
        if (newFrame == null)
            frame.ChangeImage(new ImageType());
        else
        {
            frame.ChangeImage(new ImageType(Layers.Frame, newFrame));
        }
    }
    public override bool BeActedOn<T>(T d)
    {
        if (shown == false || Symol == null)
            return false;

        if (d is Pressed a)
        {
            if (a.situation == MenueType.Fight)
            {
                SetPlant(this);
                return true;
            }
            else
                throw new NotImplementedException();

        }

        // if neither of this things ocure, throws exeption
        throw new NotImplementedException();
    }
    public override void TakeAction()
    {
        if (!_timer.Check(false) && Changed == true)
        {
            Symol.ChangeImage(new GradientType(-60, -60, -60), shown);
            Changed = false;
        }
        else if (_timer.Check(false) && Changed == false)
        {
            Symol.ChangeImage(null, shown);
            Changed = true;
        }
        return;
    }

    Planter(Position pos) : base(pos)
    {
        AllEntitys.AddEntities(this);
        Displacement = (0, 2);
        _timer = Timer.StartNew(0);
        frame = new(Layers.Frame, pos);
        frame.ChangeImage(new ImageType(Layers.Frame, "Frame1"), shown);
    }
}

