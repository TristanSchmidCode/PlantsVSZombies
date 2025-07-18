using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public static class Money
{
    
    static readonly ImageHaver image;
    static bool Shown = false;
    public static int SunAmount { get; private set; }
    static void OnMenueChange(MenueType menue)
    {
        if (menue == MenueType.Fight)
        {
            Shown = true;
            Print();
            image.PrintImage();
        }
        else
        {
            Shown = false;
            image.RemoveImage();
        }

    }
    public static void MoneyStart() {SunAmount = 0; Print(); }
    public static void SetMoney(int money)
    {
        SunAmount = money;
        Print();
    }
    static void Print()
    {
        string text = "|Sun: " + SunAmount +"|";
        List<(Position, PixelType)> building = [];

        char[] letters = text.ToCharArray();

        for (int i = 0; i < letters.Length-1; i++)
        {
            building.Add(((i, 0), 
                new PixelType( Colors.Yellow, Colors.Black, letters[i])));
        }
        image.ChangeImage(new ImageType(building),Shown);
    }
    public static void AddMoney(int money)
    {
        SunAmount += money;
        Print();
    }   
    /// <param name="money"></param>
    /// <returns>true if sucsesfully removed money, false if there wasn't enough money</returns>
    public static bool MinuseMoney(int money) 
    {
        if (money <= SunAmount)
        {
            SunAmount -= money;
            Print();
            return true;
        }
        else 
            return false;        
    }
    static Money()
    {
        image = new(Layers.Menue, (80, 3));
        Scene.OnMenueChange += OnMenueChange;
    }
}


