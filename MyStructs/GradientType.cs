using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

/// <summary>
///  A struct that contains and mutates the information that corrasponds to a change of a color
/// </summary>
public readonly struct GradientType
{
    public readonly (int r, int g, int b) RGB;
    public readonly int R => RGB.r;
    public readonly int G => RGB.g;
    public readonly int B => RGB.b;

    public readonly ColorType Shade(ColorType color)
    {
        return new(Change(color.R, R), Change(color.G, G), Change(color.B, B));

        static byte Change(byte part, int part2)
        {
            int total = (part + part2);
            if (total > 255)
                return 255;
            if (total < 0)
                return 0;

            return (byte)total;
        }
    }
    /// <param name="pixel"></param>
    /// <returns> a new <see cref="PixelType"/>that has been shaded</returns>
    public readonly PixelType Shade(PixelType pixel)
    {
        (ColorType back, ColorType front) = pixel.GetBackAndFront();
        return new(Shade(back), Shade(front), pixel.GetSymbol());
    }

    public static ColorType ShadeRandom(ColorType pixel, int difrence = 10)
    {
        Random rnd = new();
        return new GradientType(
            rnd.Next(-difrence, difrence),
            rnd.Next(-difrence, difrence),
            rnd.Next(-difrence, difrence)).Shade(pixel);
    }
    public static PixelType ShadeRandom(PixelType pixel, int difrence = 10)
    {
        Random rnd = new();
        return new GradientType(
            rnd.Next(-difrence, difrence),
            rnd.Next(-difrence, difrence),
            rnd.Next(-difrence, difrence)).Shade(pixel);
    }
    /// <summary>
    ///  creates a <see cref="GradientType"/> with the colors randomised
    /// </summary>
    /// <param name="difrence"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public GradientType(byte difrence)
    {
        Random rnd = new();
        this = new GradientType(
            rnd.Next(-difrence, difrence),
            rnd.Next(-difrence, difrence),
            rnd.Next(-difrence, difrence));
    }
    public GradientType(int r, int g, int b) : this((r, g, b)) { }
    public GradientType(Colors color) : this(Extentions.ColorConverter[color]) { }
    public GradientType((int r, int g, int b) color)
    {
        if (color.r < -255 | color.r > 255)
            throw new ArgumentOutOfRangeException(nameof(color));
        if (color.g < -255 | color.g > 255)
            throw new ArgumentOutOfRangeException(nameof(color));
        if (color.b < -255 | color.b > 255)
            throw new ArgumentOutOfRangeException(nameof(color));
        RGB = color;
    }

    public static implicit operator (int, int, int)(GradientType d) => (d.R, d.B, d.G);
    public static explicit operator GradientType((int, int, int) d) => new(d);
}

