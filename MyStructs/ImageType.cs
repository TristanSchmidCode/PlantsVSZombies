using PlantsVSZombies.Images;
using System.Text.Json;

namespace PlantsVSZombies;

/// <summary>
/// a class for containg and mutating a array that functions as a coded image
/// </summary>
public struct ImageType
{
    public readonly (Position, PixelType)[] Image
    {
        get
        {
            if (_gradient is GradientType grade)
            {
                List<(Position, PixelType)> building = [];
                foreach ((Position pos, PixelType pixel) in _image)
                {
                    building.Add((pos, grade.Shade(pixel)));
                }
                return [.. building];
            }
            else
                return [.. _image];
        }
    }
    readonly (Position, PixelType)[] _image;
    GradientType? _gradient;
    /// <summary>
    /// creats an empty image
    /// </summary>
    public ImageType()
    {
        _image = [];
    }
    /// <summary>
    /// Finds the image from the image library and sets this image to that 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="name"></param>
    public ImageType(Layers layer, string name, int randomise = 0)
    {
        string path = "";
        switch (layer)
        {
            case Layers.Plant:
                path = "Plants";
                break;
            case Layers.SZombie:
            case Layers.BZombie:
                path = "Zombies";
                break;
            case Layers.Attack:
                path = "Attacks";
                break;
            case Layers.Sun:
                path = "Suns";
                break;
            case Layers.Frame:
                path = "Intor";
                break;
            case Layers.Forground:
                path = "Other";
                break;
            case Layers.Special:
                path = "Special";
                break;
        }
        string fullPath = MyPaths.ImagesPath + path + "\\" + name;

        string Json = File.ReadAllText(fullPath);

        var f = JsonSerializer.Deserialize<ImageMaker>(Json)!;
        List<(Position, PixelType)> newImage = [];

        foreach (var d in f.image)
        {
            newImage.Add((new Position(d.Item1.Item1, d.Item1.Item2),
                new PixelType(d.Item2.Item1, d.Item2.Item2)));
        }
        _image = [.. newImage];

        this = RandomiseImage(randomise);
    }
    public ImageType(List<(Position, PixelType)> image, int randomise = 0)
    {
        _image = [.. image];
        if (randomise != 0)
            this = RandomiseImage(randomise);
    }
    public ImageType(string image, (ColorType back, ColorType front, ColorType DoubleBack) colors, bool big = false)
    {
        char[] symbols = ['{', '}', ' ', '.', ','];
        Dictionary<char, int> StandOut;

        int distence = 1;
        int baseDistance;
        List<(Position, PixelType)> newImage = [];
        if (!big)
        {
            baseDistance = 4;
            StandOut = new()
            {
                {'m',2 },
                {'w',2 },
                {'q',1 },
                {'n',1 },
                {'g',1 },
                {'.',-1},
                {',',-1}

            };
        }
        else
        {
            distence = 0;
            baseDistance = 11;
            StandOut = new()
            {
                {'w',5 },
                {'n',2 },
                {'y',1 },
                {'o',2 },
            };
        }
        foreach (char letter in image)
        {
            if (!char.IsDigit(letter) & !char.IsLetter(letter) & !symbols.Contains(letter))
                throw new ArgumentOutOfRangeException(nameof(image), "This char wandoesn't have an immage equvilant");
            
            if (letter != ' ')
                GetCharImage(letter);

            if (StandOut.TryGetValue(char.ToLower(letter), out int larg))
                distence += larg;
            
            distence += baseDistance;
        }
        _image = [.. newImage];

        void GetCharImage(char letter)
        {
            string name = "";
            if (big)
                name = "Big";

            if (letter == '{')
                name += "FunkBracketLeft";
            else if (letter == '}')
                name += "FunkBracketRight";
            else if (letter == '.' || letter == ',')
                name += "FullStop";
            else
                name += letter;

            string fullPath = MyPaths.ImagesPath + "Symbols\\" + name;

            string Json = File.ReadAllText(fullPath);

            var f = JsonSerializer.Deserialize<ImageMaker>(Json)!;


            foreach (var d in f.image)
            {
                newImage.Add((new Position(d.Item1.Item1 + distence, d.Item1.Item2),
                    new PixelType(Swap(d.Item2.Item1), Swap(d.Item2.Item2), d.Item2.Item2.Last())));
            }

        }
        ColorType Swap(string input)
        {
            if (input.Contains(";2;0;255;0m"))
                return colors.front;
            if (input.Contains(";2;0;110;10m"))
                return colors.back;

            if (input.Contains(";2;255;255;255m"))
                return colors.DoubleBack;

            return new(255, 0, 0);
        }
    }
    public ImageType(string image, ColorType back, ColorType front)
    {
        char[] charictors = [.. image];

        List<(Position, PixelType)> newImage = [];

        int counter = 0;
        foreach (var charictor in charictors)
        {
            newImage.Add(((counter, 0), new PixelType(back, front, charictor)));
            counter++;
        }
        _image = [.. newImage];
    }

    public readonly void Print(Position pos, Layers layer = Layers.Forground) =>
        _ = new ImageHaver(this, layer, pos);
    
    /// <summary>
    /// Changes the gradient of the image
    /// </summary>
    /// <param name="gradient"></param>
    public ImageType ChangeGradient(GradientType? gradient)
    {
        _gradient = gradient;
        return this;
    }

    /// <summary> Randomises the Image in a controled mannor</summary>
    /// <returns> A copy of the Image with randomised colors</returns>
    public readonly ImageType RandomiseImage(int difrence = 10)
    {
        //key is the colors that have been seen once,
        //the value is the colors that it needs to be replaced with
        Dictionary<string, (ColorType, ColorType)> remembered = [];

        //The list used to make the return image
        List<(Position, PixelType)> building = [];
        foreach (((int, int) pos, PixelType pixel) in Image)
        {
            string PixelWithoutSymbol = pixel.pixel[0..(pixel.pixel.Length - 1)];
            //if the color has been seen before, changes it to the remembered one
            if (remembered.TryGetValue(PixelWithoutSymbol, out (ColorType, ColorType) colors))
            {
                PixelType d = new(
                    colors.Item1,
                    colors.Item2,
                    pixel.GetSymbol());
                building.Add((pos, d));
            }
            //if it hasn't been remembered, adds a randomised color
            else
            {
                PixelType d = GradientType.ShadeRandom(pixel, difrence);
                building.Add((pos, d));
                remembered.Add(PixelWithoutSymbol, d.GetBackAndFront());
            }
        }
        return new ImageType(building);
    }

    /// <summary>
    ///  randomly changes all colors in the image
    /// </summary>
    /// <param name="difrence"></param>
    /// <returns> A new image with randomised colors</returns>
    public readonly ImageType TrueRandomiseImage(int difrence = 10)
    {
        List<(Position, PixelType)> building = [];
        foreach (((int, int) pos, PixelType pixel) in Image)
        {
            building.Add((pos, GradientType.ShadeRandom(pixel, difrence)));
        }
        return new(building);
    }

    class ImageMaker
    {
        //ImageMaker is only made with json, so there is no error, must be lower case for json to work
#pragma warning disable IDE1006
        public required List<Tuple<Tuple<int, int>, Tuple<string, string>>> image { get; set; }
#pragma warning restore IDE1006
    }
}

