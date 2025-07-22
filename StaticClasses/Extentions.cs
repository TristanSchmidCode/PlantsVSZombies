

namespace RBGColors
{
    public enum Colors
    {
        White,
        DarkGreen,
        PukeBroun,
        NormalBlue,
        Yellow,
        Blue,
        Green,
        Red,
        Gray,
        Black,


        HeroColor,
        VillainColor,

        BaseWriting,
        CursorColor,
        CursorTxtColor,
        BackgroundColor,
        SpokenColor,




        //DesctiptionColors
        BaseItemColor,
        DamageColor,
        DefenceColor,
        MoneyColor,

        NoEncahntments,

        LightArmour,
        HeavyArmour,
        OddArmour,




        //Settings UI
        SettingColor,
        HealthColor,
        LevelColor,
        CompanionColor,
        ToolSColor,
        MagicColor,
        ItemsColor,
        MapColor,

        //Names
        Kathstar,
        //Error
        Error,



    }
    public static class Extentions
    {
        public readonly static Dictionary<Colors, (byte r, byte b, byte g)> ColorConverter = new()
        {
            {Colors.White, (255,255,255) },
            {Colors.DarkGreen,(0,110,10) },
            {Colors.Green,(0,255,0) },
            {Colors.Red,  (255,0,0) },
            {Colors.PukeBroun,(129,55,12) },
            {Colors.NormalBlue,(20,55,220) },
            {Colors.Yellow, (240,240,60) },
            {Colors.Blue, (0,0,255) },
            {Colors.Gray, (50,50,50) },
            {Colors.Black, (0,0,0) },


            {Colors.HeroColor,(0,240,40) },//green
            {Colors.VillainColor,(255,40,0)},//red
       
            {Colors.BaseWriting,(255,255,255) },
            {Colors.CursorColor,(20,80,130) },
            {Colors.CursorTxtColor,(0,0,0) },
            {Colors.BackgroundColor,(0,0,0) },
            {Colors.SpokenColor,(80,70,130) },
            
       
            
            //itemsColors
            {Colors.BaseItemColor,(80,80,80) },//darkgray
            {Colors.DamageColor,(210,50,0) },//redish
            {Colors.DefenceColor,(90,90,150) },//blueishgray
            
            {Colors.MoneyColor,(230,240,0) },//goldenish
            {Colors.NoEncahntments,(110,110,110) }, //gray
       
            {Colors.LightArmour,(170,170,170)},//lightgray
            {Colors.HeavyArmour,(50,50,50) }, //darkGray
            {Colors.OddArmour,(0,160,210) }, //cyanish
       
       
            //UI Settings
            { Colors.SettingColor,(222,220,30) },//yellowesk
            { Colors.HealthColor,(240,40,40)}, //red
            { Colors.LevelColor,(255,255,100)}, //yellowish
            { Colors.CompanionColor,(150,60,250) }, //purple
            { Colors.ToolSColor,(80,125,0)},  //
            { Colors.MagicColor,(200,0,170)}, //Purple
            { Colors.ItemsColor,(0,200,170)}, //
            { Colors.MapColor,(200,170,0)}, //brounish
       
            //Names
            { Colors.Kathstar, (60,60,60) },
       
            //Error
            { Colors.Error, (155,0,180) } //pinkish
        };


        public static string BackgroundStringColor(this Colors color)
        {
            byte red;
            byte green;
            byte blue;
            (red, green, blue) = ColorConverter[color];

            return $"\x1b[48;2;{red};{green};{blue}m";
        }

        public static string BackgroundStringColor(this (byte r, byte g, byte b) colors)
        {
            byte red;
            byte green;
            byte blue;
            (red, green, blue) = colors;

            return $"\x1b[48;2;{red};{green};{blue}m";
        }
        public static string ForgroundStringColor(this Colors color)
        {
            byte red;
            byte green;
            byte blue;
            (red, green, blue) = ColorConverter[color];

            return $"\x1b[38;2;{red};{green};{blue}m";
        }

        public static byte ChangeColor(this byte color, int changeBye)
        {
            if (color + changeBye > 255)
            {
                return 255;
            }
            else if (color + changeBye < 0)
            {
                return 0;
            }
            return (byte)((byte)changeBye + color);
        }
     
        /// <returns> A new byte with a +- a random number within <see href="difrence"/> </returns>
        public static string RandomiseColor(this string Pixel, int difrence)
        {
            if (difrence == 0)
                return Pixel;
            Random rnd = new();
            int r, g, b;
            r = rnd.Next(-difrence, difrence);
            g = rnd.Next(-difrence, difrence);
            b = rnd.Next(-difrence, difrence);

            return Pixel.ChangeColor(r, g, b);
        }
        public static string ChangeColor(this string pixel, int r = -10, int g = -10, int b = -10)
        {
            int[] colors= [r, g, b];
            string PlaceHolder = pixel["\x1b[48;2".Length..];

            PlaceHolder = PlaceHolder[..(pixel.IndexOf('m') - 6)];

            string[] splits = PlaceHolder.Split(";", StringSplitOptions.RemoveEmptyEntries);
            
            string Replacement = "";
            for (int i = 0; i < 3; i++)
            {
                Replacement += ";" + Convert.ToByte(splits[i]).ChangeColor(colors[i]);
            }
            
            string F = pixel.Replace(PlaceHolder, Replacement);

            return F;

        }


    }
}
namespace PlantsVSZombies
{
   
   
    /// <summary>
    /// Contains Basic information such as mapsize and contains
    /// functions for positions "(int,int)"
    /// </summary>
    public static class BordInfo
    {
        static BordInfo()
        {
            Console.WriteLine("You must make go full screen for the program to work");
            while (Console.WindowHeight < 38 & Console.WindowHeight <170)
                Thread.Sleep(1);
            
            MapSize = (Console.WindowWidth, Console.WindowHeight); 
        }

        static public readonly (int left, int up) MapSize;
        //the horizontal position of the 4 rows
        static public readonly int[] rows = [10, 18, 26, 34];

        static public readonly (int left, int up) PlantMapSize = (8, 3);
        public const int LeftBorder = 21;
       
        static public int MapLeft { get { return MapSize.left; } }
        static public int MapUp { get { return MapSize.up; } }
        static readonly public (ColorType, ColorType, ColorType) GreenText = (Colors.DarkGreen, Colors.Green, Colors.White);
        static readonly public (ColorType, ColorType, ColorType) RedText = (new ColorType(), Colors.Red, Colors.White);

        /// <param name="plantPos"></param>
        /// <returns>A plant Position as a position on the pixel map</returns>
        static public Position PlantPosition(this (int column, int row) plantPos)
        {
            Position d = (LeftBorder +7 + plantPos.column * 15, rows[plantPos.row]);
            return d;
        }
        static public bool IsOutOfBounds(this Position d)
        {
            if (d.X > MapLeft-1 || d.X < 0)
                return true;
            else if (d.Y > MapUp-1 || d.Y < 0)
                return true;
            return false;
        }
        static public (int,int) Add(this (int,int) pos1, (int,int) pos2)
        {
            (int,int) building = (pos1.Item1 + pos2.Item1, pos1.Item2 +pos2.Item2);

            return building;
        }
        static public (float, float) Add(this (float, float) pos1, (float, float) pos2)
        {
            return (pos1.Item1 + pos2.Item1, pos1.Item2 + pos2.Item2);
        }
        static public (int,int) Minus(this (int,int) pos1, (int,int) pos2)
        {
            (int, int) building = (pos1.Item1 - pos2.Item1, pos1.Item2 - pos2.Item2);

            return building;
        }
        static public (float, float) Minus(this (float, float) pos1, (float, float) pos2)
        {
            return (pos1.Item1 - pos2.Item1, pos1.Item2 - pos2.Item2);
        }
        static public bool Equals(this (int, int) pos1, (int, int) pos2)
        {
            if (pos1.Item1 != pos2.Item1)
                return false;
            if (pos1.Item2 != pos2.Item2)
                return false;
            return true;
        }
    
       
        /// <returns>Returns a random starting position for a zombie.</returns>
        static public (int,int) RandomZombiePos()
        {
            return (MapLeft -4, rows[new Random().Next(4)] - 1);
        }
    }
}