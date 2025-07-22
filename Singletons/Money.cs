using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public class Money : SingleTone<Money>
{
    static Image GetImage(int SunAmount)
    {
        string text = "|Sun: " + SunAmount + "|";

        char[] letters = text.ToCharArray();

        (Position, PixelType)[] building = new (Position, PixelType)[letters.Length];
        for (int i = 0; i < letters.Length; i++)
        {
            building[i] = ((i, 0), new PixelType(Colors.Yellow, Colors.Black, letters[i]));
        }
        return new Image(building);
    }
    readonly ImageHaver image;
    public int sunAmount { get; private set; }
    void ResetImage()
    {
        image.ChangeImage(GetImage(sunAmount),true);
    }
    public void SetMoney(int money) { sunAmount = money;  ResetImage(); }
    public void AddMoney(int money) { sunAmount += money; ResetImage(); }
    public void RemoveMoney(int money) { sunAmount -= money; ResetImage(); }
    public bool HasEnoughtMoney(int amount) => amount <= sunAmount;
    public bool TryRemoveMoney(int amount) {
        if (!HasEnoughtMoney(amount))
            return false;
        RemoveMoney(amount);
        return true;
    }

    public Money(int sunAmount)
    {
        this.sunAmount = sunAmount;
        image = new(GetImage(sunAmount), Layers.Menue, (80, 3));
    }
    protected override void DestroyThis()
    {
        image.RemoveImage();
    }
    
}


