using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

public readonly struct PixelType
{
    public readonly string pixel;

    public readonly ColorType GetBackground()
    {
        return GetBackAndFront().Item1;
    }
    public readonly (ColorType, ColorType) GetBackAndFront()
    {
        if (pixel == null)
            return (default,default);
        string pix = this.pixel;
        pix = pix.Replace("\u001B[48;2;", null);
        pix = pix.Replace("\u001B[38;2;", null);
        char[] parm = [';', 'm'];
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries;
        string[] stri = pix.Split(parm, options);
        byte[] nums = new byte[6];

        for (int i = 0; i < 6; i++)
        {
            nums[i] = Convert.ToByte(stri[i]);
        }

        return (new(nums[0], nums[1], nums[2]),
                new(nums[3], nums[4], nums[5]));
    }
    public readonly ColorType GetForground()
    {
        return GetBackAndFront().Item2;
    }
    public readonly char GetSymbol()
    {

        return pixel.Last();

    }
    /// <summary>
    /// creates a black pixel
    /// </summary>
    public PixelType() : this(new ColorType(), new ColorType(), ' ') { }
    public PixelType(ColorType back) : this(back, new ColorType(), ' ') { }
    /// <summary>
    ///  Creates a pixel, asuming both strings have "\u001B[48;2;" and "\u001B[38;2;"
    /// respectivly. 
    /// Also asumes that front includes the symbole
    /// </summary>
    /// <param name="back"></param>
    /// <param name="front"></param>
    public PixelType(string back, string front)
    {
        pixel = back + front;
    }
    public PixelType(ColorType back, ColorType front) : this(back, front, ' ') { }
    /// <summary>
    /// Creates a pixel with a given forgrownd and background color, along with the symbol in the pixel
    /// </summary>
    public PixelType(ColorType back, ColorType front, char symbol)
    {
        pixel = back.Bg() + front.Fg() + symbol;
    }
    public static implicit operator string(PixelType d) => d.pixel;
    public static implicit operator PixelType(ColorType d) => new(d);
}

