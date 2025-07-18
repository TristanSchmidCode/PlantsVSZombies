global using static PlantsVSZombies.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static PlantsVSZombies.Scene;

namespace PlantsVSZombies;

public class Settings : Intoractible
{
    public Settings(Position pos) : base(pos)
    {
        frame.ChangeImage(new ImageType(Layers.Frame, "Settings"), shown);
        Show();
    }
    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed f)
        {
            if (f.situation == MenueType.FightSettings)
                ChangeMenue(MenueType.FightSettings);
            else if (f.situation != MenueType.Fight)
                ChangeMenue(MenueType.Settings);
            return true;
        }
        return false;
    }
    public override void TakeAction()
    {
        throw new NotImplementedException();
    }
}
public class SettingIntor: Intoractible 
{
    public SettingIntor(Position pos, Position fromCenter, OnIncrease d, Layers layer) : base(pos,fromCenter, layer)
    {
        increase = d;
        Show();
    }
    readonly OnIncrease increase;
    public delegate void OnIncrease(bool ind);
    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed)
            return false; 
        else if (d is Move move)
        {
            if (move.direction == ConsoleKey.LeftArrow | move.direction == ConsoleKey.UpArrow)
                increase.Invoke(true);
            else
                increase.Invoke(false);
            return true;
        }
        throw new Exception();
    }
    public override void TakeAction()
    {
        throw new NotImplementedException();
    }
}

