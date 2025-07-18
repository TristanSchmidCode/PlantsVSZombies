namespace PlantsVSZombies;


public enum Background
{
    PureBlack,
    grassFight,
    MainMenue,
    SettingMenue,
    Map1,
}
public static class Screen
{
    const int waterLevel = 7;

    static Pixel[,] _map = new Pixel[BordInfo.MapLeft, BordInfo.MapUp];
    //changes the background
    public static void ChangeBackground(Background background)
    {
        if (background == Background.grassFight)
            GrassMap();
        else if (background == Background.Map1)
            MainMenue();
        else if (background == Background.MainMenue)
            MainMenue();
        else if (background == Background.SettingMenue)
            SettingsMenue();
        else if (background == Background.PureBlack)
            PureBlackMenue();
        PrintMap();
    }
    //forcess all pixels in pixelMap.Map to print their top pixel
    static void PrintMap()
    {       
        for (int i = 0; i < BordInfo.MapLeft; i++)
        {
            for (int j = 0; j < BordInfo.MapUp; j++)
            {
                _map[i, j].PrintPixel();
            }
        }
    }

    // creates a visible line between to points
    public static void CreatLine(Position start, Position end, ColorType pixel, int difrence = 0)
    {
        (float, float) comp = end - start;
        float a = comp.Item2 / comp.Item1;
        float b = start.Y - a * start.X;

        for (int i = start.X; i < end.X; i++)
        {
            int s = (int)LinjerFunction(i);
            int f = (int)LinjerFunction(i + 1);
            //swaps so that the destination is always bigger then the start
            if (s > f)
            {
                s = f;
                f = (int)LinjerFunction(i);
            }
            //prints all the pixels on a specifix x value (if this was a graff)
            for (int j = s; j <= f; j++)
            {
                GetPixel((i, j)).AddPixel(
                    new(Layers.Forground), 
                    GradientType.ShadeRandom(pixel,difrence));
            }
        }
        float LinjerFunction(int x)
        {
            return a * x + b;
        }
    }

    /// <returns>Returns the <see cref="Pixel"/> in a specific position in the stored <see cref="PixelMap"/></returns>
    public static Pixel GetPixel(this Position pos )
    {
        return _map[pos.X,pos.Y];
    }
    
    public static void AddToEach(LayerID layer, PixelType pixel)
    {
        foreach (Pixel pix in _map)
        {
            pix.AddPixel(layer, pixel);
        }
    }
    public static void RemoveFromAllPixels(LayerID layer)
    {
        foreach (Pixel pix in _map)
        {
            pix.RemovePixel(layer);
        }
    }
    //creates a new Map, for the main menue
    static void MainMenue()
    {
        _map = new Pixel[BordInfo.MapLeft, BordInfo.MapUp];

        for (int i = 0; i < _map.GetLongLength(0); i++)
        {
            for (int j = 0; j < _map.GetLongLength(1); j++)
            {
                if (j < 8)
                {
                    _map[i, j] = new(new(Colors.Gray), (i, j), 7);
                }
                else
                {
                    ColorType green = Colors.Green;
                    Random rnd = new();
                    //for adding 'floweres'
                    if (rnd.Next(0, 100) == 0)
                    {
                        ColorType other;
                        if (rnd.Next(0, 2) == 1)
                            other = Colors.DarkGreen;
                        else
                            other = Colors.Yellow;
                        char[] symbols = ['●', '^'];
                        PixelType p = new(green, other, symbols[rnd.Next(0, 2)]);
                        _map[i, j] = new(p, (i, j), 15);
                    }
                    else
                        _map[i, j] = new(green, (i, j), 15);
                }
            }
        }
    }
    //creates a new map, for the settings menue
    static void SettingsMenue()
    {
        _map = new Pixel[BordInfo.MapLeft, BordInfo.MapUp];

        for (int i = 0; i < _map.GetLongLength(0); i++)
        {
            for (int j = 0; j < _map.GetLongLength(1); j++)
            {
                if (j < 8)
                {
                    _map[i, j] = new(new(Colors.Gray), (i, j), 7);
                }
                else
                {
                    _map[i, j] = new(new(Colors.White), (i, j), 4);
                }
            }
        }
    }
    static void GrassMap()
    {
        _map = new Pixel[BordInfo.MapLeft, BordInfo.MapUp];
        for (int i = 0; i < _map.GetLongLength(0); i++)
        {
            for (int j = 0; j < _map.GetLongLength(1); j++)
            {
                //order specific
                //for top border
                if (j <= 4)
                    _map[i, j] = new(new(Colors.Gray), (i, j), 7, Layers.Forground);
                //for left and right border
                else if (i < BordInfo.LeftBorder || i >= (8, 0).PlantPosition().X + 8)
                    _map[i, j] = new(new(Colors.Gray), (i, j), 7, Layers.Forground);
                //For watter
                else if (j == 5 + waterLevel || j == 6 + waterLevel * 2
                    || j == 7 + waterLevel * 3)
                    _map[i, j] = new(new(Colors.Blue), (i, j), 15);
                //for bottem border
                else if (j >= 8 + waterLevel * 4)
                    _map[i, j] = new(new(Colors.Gray), (i, j), 7, Layers.Forground);
                else
                {
                    ColorType green = new(Colors.Green);
                    GradientType darker = new(0, -30, 0);

                    if (int.IsOddInteger((i - BordInfo.LeftBorder) / 15) && int.IsOddInteger((j + 3) / 8))
                        green = darker.Shade(green);
                    if (int.IsEvenInteger((i - BordInfo.LeftBorder) / 15) && int.IsEvenInteger((j + 3) / 8))
                        green = darker.Shade(green);

                    Random rnd = new();
                    if (rnd.Next(0, 100) == 0)
                    {
                        ColorType other;
                        if (rnd.Next(0, 2) == 1)
                            other = new(200, 50, 0);
                        else
                            other = Colors.Yellow;
                        char[] symbols = ['●', '^'];
                        PixelType p = new(green, other, symbols[rnd.Next(0, 2)]);
                        _map[i, j] = new(p, (i, j), 15);

                    }
                    else
                        _map[i, j] = new(green, (i, j), 15);
                }

            }
        }
    }
    static void PureBlackMenue()
    {
        PixelType d = new(new ColorType(0, 0, 0));
        for (int i = 0; i < _map.GetLongLength(0); i++)
        {
            for (int j = 0; j < _map.GetLongLength(1); j++)
            {
                _map[i, j] = new(d, (i, j));
            }
        }
    }
}



/// <summary>
/// Stores a list Of PixelTypes and the top pixel  
/// </summary>
public class Pixel
{
    static readonly object _writeLock = new();
    readonly Dictionary<LayerID, GradientType> _gradient = [];
    readonly Position _pos;
    
    
    readonly Dictionary<LayerID, PixelType> _all = [];


    (LayerID Layer, PixelType Pixel) _topPixel;

    public void AddGradient(LayerID Layer, GradientType gradient)
    {
        _gradient.TryAdd(Layer, gradient);
        PrintPixel();
    }
    public void RemoveGradient(LayerID Layer)
    {
        _gradient.Remove(Layer);
        PrintPixel();
    }
    public void AddPixel(LayerID Layer, PixelType Pixel)
    {
        _all.TryAdd(Layer, Pixel);
        if (Layer < _topPixel.Layer) {
            _topPixel = (Layer,Pixel);
            PrintPixel();
        }
    }
    public void RemovePixel(LayerID Layer)
    {
        //Makes sure that the pixel was there to begin with. 
        if (_all.Remove(Layer) == false)
            return;
        //if the top layer was removed, it will find the next pixel to be shown
        if (Layer == _topPixel.Layer)
        {
            LayerID placeHolder = _all.First().Key;
            foreach (LayerID Check in _all.Keys)
                if (Check < placeHolder)
                    placeHolder = Check;
            _topPixel = (placeHolder, _all[placeHolder]);
            PrintPixel();
        }
    }

    public void PrintPixel()
    {
        //makes sure that if two thread are trying to print
        //somehting at the same time, they will be foreced to wait
        lock(_writeLock)
        {
            Console.SetCursorPosition(_pos.X, _pos.Y);
            PixelType pixel = _topPixel.Pixel;
            foreach (GradientType grade in _gradient.Values)
            {
                pixel = grade.Shade(pixel);
            }
            Console.Write(pixel);
        }        
        
    }
    public LayerID[]? FindLayers(Layers layer)
    {
        List<LayerID> list = [];
        foreach (LayerID ID in _all.Keys)
            if (ID == layer)
                list.Add(ID);
       
        foreach (LayerID ID in _gradient.Keys)
            if (ID == layer)
                list.Add(ID);

        if (list.Count==0) 
            return null;

        return [.. list];
    }
    public LayerID[]? FindLayers(Layers[] layer)
    {
        List<LayerID> list = [];
        foreach (LayerID ID in _all.Keys)
            foreach (Layers Layer in layer)
                if (ID == Layer)
                    list.Add(ID);
        foreach (LayerID ID in _gradient.Keys)
            foreach (Layers Layer in layer)
                if (ID == Layer)
                    list.Add(ID);    

        if (list.Count == 0)
            return null;

        return [.. list];
    }
    /// <summary>
    /// Takes a Layer and return the lowest LayerID in that layer
    /// </summary>
    /// <param name="Layer"></param>
    public LayerID? this[Layers Layer]
    {
        get
        {
            LayerID? ID = null;
            foreach (LayerID Keylayer in _all.Keys)
                if (Keylayer < ID || ID == null)
                    if (Keylayer.layer == Layer)
                        ID = Keylayer;
            
            foreach (LayerID Keylayer in _gradient.Keys)
                if (Keylayer < ID || ID == null)
                    if (Keylayer.layer == Layer)
                        ID = Keylayer;

            return ID;
        }
    }
    /// <summary>
    /// Takes a an array of layers and return the lowest LayerID in that layer
    /// </summary>
    /// <param name="Layer"></param>
    public LayerID? this[Layers[] Layer]
    {
        get 
        {
            LayerID? ID = null;
            foreach (LayerID Keylayer in _all.Keys)
                if (Keylayer < ID || ID == null)
                    foreach (Layers layer in Layer)
                        if (Keylayer.layer == layer)
                            ID = Keylayer;
            foreach (LayerID Keylayer in _gradient.Keys)
                if (Keylayer < ID || ID == null)
                    foreach (Layers layer in Layer)
                        if (Keylayer.layer == layer)
                            ID = Keylayer;
            return ID;
        }
    }
    public Pixel(
        PixelType pixel, 
        (int,int) pos,
        byte difrence = 0, Layers layer = Layers.Background)
    {
        this._pos = pos;
        _topPixel = (new LayerID(layer), GradientType.ShadeRandom(pixel, difrence));
        _all.Add(_topPixel.Layer,_topPixel.Pixel);
    }
}

