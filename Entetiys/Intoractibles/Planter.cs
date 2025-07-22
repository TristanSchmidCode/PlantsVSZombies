using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class Planter : Intoractible, IEntity
{
    //shows all planters during fights 
    public string? PlantName { get; private set; }
    public int Price => PlantName != null ? PlantHanderler.GetPlantData(PlantName).Price : throw new ArgumentNullException("PlantName was null");
    //wether or not the image has shadded out as it should be
    public bool changed = true;
    readonly Timer _timer;
    
    /// <summary>
    /// checks if the this planter has a plant, wether the its on time out, 
    /// and if there's enough money
    /// </summary>
    /// <returns></returns>
    public bool CanPlant()
    {
        if (PlantName == null)
            return false;
        if (!_timer.Check(false))
            return false;
        if (!Money.Instance!.HasEnoughtMoney(Price))
            return false;
        return true;
    }
    public void HasPlanted()
    {
        if (PlantName == null)
            throw new ArgumentNullException(nameof(PlantName), "PlantName was null, this should not happen");
        changed = true;
        
        _timer.NewInterval(PlantHanderler.GetPlantData(PlantName!).TimeOut);
        _timer.ReStart();
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
        PlantName = newPlant;
        if (newPlant == null)
            Symol.ChangeImage(new Image(), shown);
        else
            Symol.ChangeImage(Image.GetImage(Layers.Plant, newPlant), shown);
        return true;
    }
    public void Select(bool select)
    {
        if (select)
            Frame.ChangeImage(Image.GetImage(Layers.Frame, "SelFrame"));     
        else
            Frame.ChangeImage(Image.GetImage(Layers.Frame, "Frame1"));
    }

    public override bool BeActedOn<T>(T d)
    {
        if (shown == false || PlantName == null)
            return false;

        if (d is Pressed) {
            PlantHanderler.Instance!.SelectedPlanter = this;
            return true;
        }
        // if neither of this things ocure, throws exeption
        throw new NotImplementedException();
    }
    public void TakeAction()
    {
        if (!_timer.Check(false) && changed == true)
        {
            Symol.ChangeImage(new GradientType(-60, -60, -60), shown);
            changed = false;
        }
        else if (_timer.Check(false) && changed == false)
        {
            Symol.ChangeImage(null, shown);
            changed = true;
        }
        return;
    }

    public Planter(Position pos) : base(pos)
    {
        Displacement = (0, 2);
        _timer = Timer.StartNew(0);
        Frame = new(Layers.Frame, pos);
        Frame.ChangeImage(Image.GetImage(Layers.Frame, "Frame1"), shown);
        EntityHanderler.Instance.AddEntity(this);
    }
    public override void Destroy()
    {
        EntityHanderler.Instance!.RemoveEntity(this);
        base.Destroy();
    }
}

