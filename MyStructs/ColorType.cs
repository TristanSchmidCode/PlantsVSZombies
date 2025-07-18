using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;

/// <summary>
/// A class that contians the information of a color using RGB 
/// </summary>
/// <param name="color"></param>
public readonly struct ColorType((byte r, byte g, byte b) color)
{
    public readonly (byte r, byte g, byte b) RGB = color;
    public readonly byte R => RGB.r;
    public readonly byte G => RGB.g;
    public readonly byte B => RGB.b;


    /// <returns> The string for this color as a background</returns>
    public readonly string Bg() => $"\x1b[48;2;{R};{G};{B}m";



    /// <returns>The string for this color as a background</returns>
    public readonly string Fg() => $"\x1b[38;2;{R};{G};{B}m";



    /// <summary>
    /// Creats black
    /// </summary>
    public ColorType() : this((0, 0, 0)) { }

    public ColorType(byte r, byte g, byte b) : this((r, g, b)) { }

    public ColorType(Colors color) : this(Extentions.ColorConverter[color]) { }


    public static implicit operator (byte, byte, byte)(ColorType d) => d.RGB;
    public static implicit operator ColorType(Colors d) => new(d);

}

