using PlantsVSZombies.Images;
using RBGColors;
using System.Collections.ObjectModel;
using System.Text.Json;
using static PlantsVSZombies.Image;

namespace PlantsVSZombies;


class LoadedImageLibrary
{
    private static Dictionary<(Layers, string), Image> _imageLibrary = new();
    private static Dictionary<(char,bool), Image.ImageMaker> _charLibrary = new();
    public static Image GetImage(Layers layer, string name, int randomise = 0)
    {
        if (_imageLibrary.TryGetValue((layer, name), out var savedImage)) // Fix: Changed 'out Image?' to 'out var'
        {
            if (randomise == 0)
                return savedImage;

            return savedImage.RandomiseImage(randomise);
        }

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

        var imageMaker = JsonSerializer.Deserialize<Image.ImageMaker>(Json)!;
        (Position, PixelType)[] newImage = new (Position, PixelType)[imageMaker.image.Count];
        for (int i = 0; i < imageMaker.image.Count; i++)
        {
            newImage[i] = (new Position(imageMaker.image[i].Item1.Item1, imageMaker.image[i].Item1.Item2),
                new PixelType(imageMaker.image[i].Item2.Item1, imageMaker.image[i].Item2.Item2));
        }
     
        Image imageToSave = new Image(newImage, randomise);
        _imageLibrary[(layer, name)] = imageToSave;
        if (randomise == 0)
            return imageToSave;
        else
            return imageToSave.RandomiseImage(randomise);
    }

    public static Image.ImageMaker GetImage(char letter, bool big = false)
    {
        if (_charLibrary.TryGetValue((letter,big),out Image.ImageMaker? value))
            return value;
        
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

        ImageMaker f = JsonSerializer.Deserialize<ImageMaker>(Json)!;
        return f;
    }
}
/// <summary>
/// a class for containg and mutating a array that functions as a coded image
/// </summary>
public readonly struct Image
{
    public readonly ReadOnlyCollection<(Position, PixelType)> Pixels
    {
        get
        {
            if (_gradient is not GradientType grade)
                return _pixels.AsReadOnly();

            List<(Position, PixelType)> building = [];
            foreach ((Position pos, PixelType pixel) in _pixels)
            {
                building.Add((pos, grade.Shade(pixel)));
            }
            return building.AsReadOnly();
            
        }
    }
    readonly (Position, PixelType)[] _pixels = [];
    public readonly GradientType? Gradient => _gradient;
    readonly GradientType? _gradient;
    /// <summary>
    /// Finds the image from the image library and sets this image to that 
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="name"></param>
    public static Image GetImage(Layers layer, string name, int randomise = 0)
    {
        return LoadedImageLibrary.GetImage(layer, name, randomise);
    }
    public Image() { }
    public Image((Position, PixelType)[] image, int randomise = 0)
    {
        if (randomise == 0)
            _pixels = image;
        else
            _pixels = RandomiseImage(image.AsReadOnly(), randomise);
    }
    public Image((Position, PixelType)[] image, GradientType? gradient, int randomise = 0)
    {
        if (randomise == 0)
            _pixels = image;
        else
            _pixels = RandomiseImage(image.AsReadOnly(), randomise);
        this._gradient = gradient;
    }
   
    public Image(string image, (ColorType back, ColorType front, ColorType DoubleBack) colors, bool big = false)
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
        _pixels = [.. newImage];

        void GetCharImage(char letter) {
            ImageMaker imageMaker = LoadedImageLibrary.GetImage(letter, big);

            foreach (var pixel in imageMaker.image)
            {
                newImage.Add((new Position(pixel.Item1.Item1 + distence, pixel.Item1.Item2),
                    new PixelType(Swap(pixel.Item2.Item1), Swap(pixel.Item2.Item2), pixel.Item2.Item2.Last())));
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
    public Image(string image, ColorType back, ColorType front)
    {
        (Position, PixelType)[] newImage = new (Position, PixelType)[image.Length];
        for (int i = 0; i < image.Length; i++)
            newImage[i] = ((i, 0), new PixelType(back, front, image[i]));      
        _pixels = newImage;
    }

    public void Print(Position pos, Layers layer = Layers.Forground) => _ = new ImageHaver(this, layer, pos);
    /// <summary>
    /// Changes the gradient of the image
    /// </summary>
    /// <param name="gradient"></param>
    public Image AddGradiant(GradientType? gradient)
    {
        return new Image(_pixels, gradient);
    }

    /// <summary> Randomises the Image in a controled mannor</summary>
    /// <returns> A copy of the Image with randomised colors</returns>
    public static (Position, PixelType)[] TrueRandomiseImage(ReadOnlyCollection<(Position, PixelType)> pixels, int difrence = 10)
    {
        (Position, PixelType)[] building = new (Position, PixelType)[pixels.Count];
        for (int i = 0; i < pixels.Count; i++)
        {
            (Position pos, PixelType pixel) = pixels[i];
            //randomises the pixel
            PixelType randomisedPixel = GradientType.ShadeRandom(pixel, difrence);
            building[i] = (pos, randomisedPixel);
        }
        return building;
    }
    public static (Position, PixelType)[] RandomiseImage(ReadOnlyCollection<(Position, PixelType)> pixels, int difrence = 10) {
        //key is the colors that have been seen once,
        //the value is the colors that it needs to be replaced with
        Dictionary<string, (ColorType, ColorType)> remembered = [];
        //The list used to make the return image
        (Position, PixelType)[] building = new (Position, PixelType)[pixels.Count];
        for (int i = 0; i < pixels.Count; i++)
        {
            (Position pos, PixelType pixel) = pixels[i];
            string PixelWithoutSymbol = pixel.pixel[0..(pixel.pixel.Length - 1)];
            //if the color has been seen before, changes it to the remembered one
            if (remembered.TryGetValue(PixelWithoutSymbol, out (ColorType, ColorType) colors))
            {
                PixelType d = new(
                    colors.Item1,
                    colors.Item2,
                    pixel.GetSymbol());
                building[i] = (pos, d);
            }
            //if it hasn't been remembered, adds a randomised color
            else
            {
                PixelType randomisedPixel = GradientType.ShadeRandom(pixel, difrence);
                building[i] = (pos, randomisedPixel);
                remembered.Add(PixelWithoutSymbol, randomisedPixel.GetBackAndFront());
            }
        }
        return building;
    }
    /// <summary>
    ///  randomly changes all colors in the image
    /// </summary>
    /// <param name="difrence"></param>
    /// <returns> A new image with randomised colors</returns>
    public Image TrueRandomiseImage(int difrence = 10) => new Image(TrueRandomiseImage(Pixels, difrence));
    public Image RandomiseImage(int difrence = 10) => new Image (RandomiseImage(Pixels,difrence));    

    public class ImageMaker
    {
        //ImageMaker is only made with json, so there is no error, must be lower case for json to work
#pragma warning disable IDE1006
        public required List<Tuple<Tuple<int, int>, Tuple<string, string>>> image { get; set; }
#pragma warning restore IDE1006
    }
}

