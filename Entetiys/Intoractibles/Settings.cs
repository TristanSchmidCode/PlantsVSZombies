global using static PlantsVSZombies.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static PlantsVSZombies.Scene;

namespace PlantsVSZombies;

public class SettingIntor: Intoractible 
{
    public SettingIntor(Position pos, Position fromCenter, OnIncrease @delegate, Layers layer, 
        Image? symol = null, Image? frame = null)
        : base(pos, fromCenter, layer,symol,frame)
    {
        increase = @delegate;
        Show();
    }
    readonly OnIncrease increase;
    public delegate void OnIncrease(bool ind);
    public override bool BeActedOn<T>(T d)
    {
        if (d is Pressed)
        {
            Cursor.TakeKeybordFocus(this);
            
            return true;
        }
        else if (d is KeyPress move)
        {
            if (move.key == ConsoleKey.LeftArrow | move.key == ConsoleKey.UpArrow)
                increase.Invoke(true);
            else if (move.key == ConsoleKey.RightArrow | move.key == ConsoleKey.DownArrow)
                increase.Invoke(false);
            return true;
        }

        throw new Exception();
    }
}

