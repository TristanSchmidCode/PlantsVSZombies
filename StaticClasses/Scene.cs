using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static PlantsVSZombies.Fight;



namespace PlantsVSZombies;
public enum MenueType
{
    Fight,
    LevelSelect,
    MainMenue,
    Settings,
    FightSettings,
    SaveScreenMenue,
    HelpMenue
}
public class Scene
{
    public static event MenueChange? OnMenueChange;
    public delegate void MenueChange(MenueType menue);
    
    
    static MenueType _menue;

    public static void Start()
    {
        ChangeMenue(MenueType.MainMenue);
        Thread d = new(Cursor.Start);
        d.Start();
        Thread.Sleep(1000);
    }


    public static void ChangeMenue(MenueType menue)
    {
        if (_menue == menue)
            return;
        _menue = menue;        
        if (menue == MenueType.LevelSelect)
        {
            ChangeBackground(Background.Map1);
            
            new ImageType(Layers.Forground, "Tital").Print((BordInfo.MapLeft / 2 - 4, 5));
            ImageType back = new("Back", (new(110, 0, 0), Colors.Red, Colors.Green));
            Cursor.Move(new MenueChanger((1, 38), (6, 0), MenueType.MainMenue, back));
        }
        if (menue == MenueType.MainMenue)
        {
            ChangeBackground(Background.MainMenue);

            MainMenue.Open();
        }
        if (menue == MenueType.Settings)
        {
            ChangeBackground(Background.SettingMenue);

            Cursor.Move(new SettingsMenue(null).gameSpeed);
        }
        if (menue == MenueType.SaveScreenMenue)
        {
            ChangeBackground(Background.PureBlack);
            Cursor.Move(new MenueChanger(MenueType.LevelSelect));
        }
        if (menue == MenueType.FightSettings)
        {
            Fight.CurrentFight!.Pause(true);
            LayerID ID = new(Layers.Inermenue);
            Screen.AddToEach(ID, new PixelType(Colors.White));
            Cursor.Move(new SettingsMenue(ID).gameSpeed);
        }
        if (menue == MenueType.HelpMenue)
        {
            ChangeBackground(Background.PureBlack);
            var d = (Colors.DarkGreen, Colors.Green, Colors.Black);
            new ImageType("You are playing plants vs zombies,PvZ", d, false).Print((2, 5));
            new ImageType("This game is about defending agains Zombies", d, false).Print((2, 9));
            new ImageType("To do so, you plant plants down, ", d, false).Print((2, 13));
            new ImageType ("each of which cost sun", d, false).Print((2, 17));
            new ImageType("Move your mouse with the arrow keys,", d, false).Print((2, 21));
            new ImageType("and select things with the enter key", d, false).Print((2, 25));
            new ImageType("Have fun", d, false).Print((2, 27));

            ImageType back = new("Leave", (new(110, 0, 0), Colors.Red, Colors.Black));
            Cursor.Move(new MenueChanger((70, 38), (6, 0), MenueType.MainMenue, back));
        }
        OnMenueChange?.Invoke(menue);
    }

    static public void Run()
    {
        if (_menue == MenueType.Settings || _menue == MenueType.FightSettings)
            Settings_CursorControle();
        else
            Base_CursorControle();

        if (_menue == MenueType.Fight)
            InFight();

        return;
        void Base_CursorControle()
        {
            var Keys = Cursor.ReadKeys();

            foreach (var key in Keys)
            {
                Cursor.Move(key);

                if (key == ConsoleKey.Enter)
                {
                    Cursor.Current.BeActedOn(new Pressed(_menue));
                }
                if (key == ConsoleKey.Escape)
                {
                    if (_menue == MenueType.Fight)
                        ChangeMenue(MenueType.FightSettings);
                    else if (_menue == MenueType.FightSettings)
                        ChangeMenue(MenueType.Fight);
                }
            }
            Cursor.CheckIntoractions();
        }
        void Settings_CursorControle()
        {
            var Keys = Cursor.ReadKeys();
            foreach (var key in Keys)
            {
                //when you activate an intoractible inside the setting menue,
                //makes sure pressing keys doesn't move your cursor
                if (SettingsMenue.Insetting)
                {
                    if (Cursor.moveKeys.Contains(key))
                        Cursor.Current.BeActedOn(new Move(key));
                    if (key == ConsoleKey.Escape)
                        SettingsMenue.Insetting = false;
                }
                else
                {
                    Cursor.Move(key);
                    if (key == ConsoleKey.Enter)
                    {
                        SettingsMenue.Insetting = true;
                        Cursor.Current.BeActedOn(new Pressed());
                    }
                }
            }

            return;
        }
    }
    static void InFight()
    {
        FightingState state = Fight.CurrentFight!.ActivateAll();

        if (state == FightingState.FightLost)
        {
            (ColorType, ColorType, ColorType) color = (new(110, 0, 0), Colors.Red, Colors.Black);
            ChangeMenue(MenueType.SaveScreenMenue);
            new ImageType("You Lost", color, true).Print((15,20));
            if (Fight.CurrentFight.FinalMessage != string.Empty)
                new ImageType(Fight.CurrentFight.FinalMessage, color).Print((15, 28));
            
        }
        else if (state == FightingState.FightWon)
        {
            (ColorType, ColorType, ColorType) color = (Colors.DarkGreen, Colors.Green, Colors.Black);
            ChangeMenue(MenueType.SaveScreenMenue);
            new ImageType("You Won", color, true).Print((15,20));
          
            if (Fight.CurrentFight.FinalMessage != string.Empty)
                new ImageType(Fight.CurrentFight.FinalMessage,color).Print((15, 28));
            
        }
    }  
}

public class MainMenue
{
    ImageHaver tital;
    ImageHaver graveStone;
    ImageHaver[] zombies;
    Intoractible start;
    Intoractible help;
    Intoractible settings;

    public static void Open()
    {
        _ = new MainMenue();
    }
    MainMenue()
    {
        tital = new(Layers.Forground, (BordInfo.MapLeft/2 -4, 5));
        tital.ChangeImage(new ImageType(Layers.Forground, "Tital"));
        graveStone = new(Layers.Forground, (BordInfo.MapLeft / 2+20, 25));
        graveStone.ChangeImage(new ImageType(Layers.Frame, "GraveStone").TrueRandomiseImage());
        zombies = [
            new(Layers.Forground, (32, 20)),
            new(Layers.Forground, (11, 25)),
            new(Layers.Forground, (120, 32)),
            new(Layers.Forground, (23, 22)),
            new(Layers.Forground, (83, 12)),
            new(Layers.Forground, (22, 37)),
            new(Layers.Forground, (105, 35)),
            new(Layers.Forground, (95, 12)),
            new(Layers.Forground, (135, 25)),
            new(Layers.Forground, (65, 19)),

        ];

        //makes a random zombe image
        foreach (var zombie in zombies)
        {
            Random rnd = new();
            int k = new Random().Next(0, 10);
            if (k < 6)
                zombie.ChangeImage(new ImageType(Layers.BZombie, "BaseZombie"));
            else if (k < 8)
                zombie.ChangeImage(new ImageType(Layers.BZombie, "FatZombie"));
            else
                zombie.ChangeImage(new ImageType(Layers.BZombie, "TractorZombie"));
        }
        //makes the river 
        CreatLine((0, 40), (BordInfo.MapLeft, 21), new(Colors.PukeBroun), 15);
        for (int i = 40; i > 33; i--)
            CreatLine((0, i), (BordInfo.MapLeft, i/2), new(Colors.Blue),  15);
        CreatLine((0, 33), (BordInfo.MapLeft, 16), new(Colors.PukeBroun), 15);

        ImageType StartImage = new("Start", (Colors.DarkGreen, Colors.Green, Colors.Gray));
        ImageType HelpImage = new("Help", (Colors.DarkGreen, Colors.Green, Colors.Green));
        start = new MenueChanger(graveStone.Pos + (-9, -2), (9,1), MenueType.LevelSelect, StartImage);
        help = new MenueChanger(graveStone.Pos + (-7, 2), (8, 1), MenueType.HelpMenue, HelpImage);
        settings = new MenueChanger(graveStone.Pos + (-3, 4), (3,2),MenueType.Settings, new ImageType("Settings",Colors.Gray,Colors.Red));

        Cursor.Move(start);
        Scene.OnMenueChange += OnMenueChange;
        
    }


    public void OnMenueChange(MenueType menue)
    {
        if (menue != MenueType.MainMenue)
        {
            tital.RemoveImage();
            graveStone.RemoveImage();
            start.Destroy();
            settings.Destroy();
            foreach (var zombie in zombies)
                zombie.RemoveImage();

            Scene.OnMenueChange -= OnMenueChange;
        }
    }

}

public class SettingsMenue
{
    public static bool Insetting { get; set; }
    public static float GameSpeed { get; private set; }
    static int _indexOfCursorColor;
    
    readonly GradientType[] _cursorColors = [new(-30,-30,-30),new(-60,-60,-60),new(100,-20,-20), new(-20,100,-20), new(-20, -20, 100)];
    
    public readonly Intoractible gameSpeed;
    readonly ImageHaver _tital;
    readonly Intoractible _cursorColor;
    readonly Intoractible _leave;
    readonly Intoractible? _leaveFight;
    
    void ChangeGameSpeed(bool up)
    {
        if (GameSpeed < 2f)
        {
            if (up)
                GameSpeed += 0.2f;
            else if (GameSpeed > 0.4f)
                GameSpeed -= 0.2f;
        }
        else if (GameSpeed >= 2f)
        {
            if (up)
                GameSpeed += 1;
            else if (GameSpeed > 2f)
                GameSpeed -= 1f;
            else
                GameSpeed -= 0.2f;
        }
        GameSpeed = MathF.Round(GameSpeed, 2);

        gameSpeed.Symol.ChangeImage(new ImageType($"{{{GameSpeed}}}", BordInfo.GreenText), true);
    }
    void ChangeCursorColor(bool up)
    {
        if (up)
        {
            if (_indexOfCursorColor == _cursorColors.Length - 1)
                _indexOfCursorColor = 0;
            else
                _indexOfCursorColor++;
        }
        else
        {
            if (_indexOfCursorColor == 0)
                _indexOfCursorColor = _cursorColors.Length - 1;
            else
                _indexOfCursorColor--;
        }
        Cursor.gradient.ChangeGradient(_cursorColors[_indexOfCursorColor]);
    }
    static SettingsMenue()
    {
        GameSpeed = 1f;        
    }


    public SettingsMenue(LayerID? FightSettings)
    {
        _isFightingMeneu = FightSettings;
        Layers layer;
        if (FightSettings.HasValue)
        {
            layer = Layers.InnerIntoractible;

            _tital = new(new ImageType(Layers.Forground, "Tital"), Layers.InnerIntoractible, (BordInfo.MapLeft / 2 - 4, 5));

            _leave = new MenueChanger((1, 40), (15, 0), MenueType.Fight,
                    new ImageType("Go Back", BordInfo.RedText), layer);
            _leaveFight = new MenueChanger((117, 40), (15, 0), MenueType.MainMenue,
                new ImageType("Main Menue", BordInfo.RedText), layer);

        }
        else
        {
            layer = Layers.Frame;
            _tital = new(new ImageType(Layers.Forground, "Tital"), Layers.Forground, (BordInfo.MapLeft / 2 - 4, 5));
            _leave = new MenueChanger((1, 40), (15, 0), MenueType.MainMenue,
                new ImageType("Leave", BordInfo.RedText));
        }


        gameSpeed = new SettingIntor((1, 15), (50, 0), ChangeGameSpeed, layer);
        //creates the image for the game speed intoractible
        gameSpeed.frame.ChangeImage(new ImageType("Game Speed ", BordInfo.GreenText), true);
        gameSpeed.Symol.ChangeImage(new ImageType($"{{{GameSpeed}}}", BordInfo.GreenText), true);
        gameSpeed.Symol.MoveBy((45, 0));

        //creates the image for the CursorColor intoractible
        _cursorColor = new SettingIntor((1, 22), (63, 1), ChangeCursorColor,layer);
        _cursorColor.frame.ChangeImage(new ImageType("Cursor Color {    }", BordInfo.GreenText), true);

       

        Cursor.Move(_leave);

        Scene.OnMenueChange += OnChangeMenue;
    }

    readonly LayerID? _isFightingMeneu;
    public void OnChangeMenue(MenueType menue)
    {
        if (menue != MenueType.Settings & menue != MenueType.FightSettings)
        {
            Scene.OnMenueChange -= OnChangeMenue;

            gameSpeed.Destroy();
            _cursorColor.Destroy();
            _leave.Destroy();
            _tital.RemoveImage();
            if (_isFightingMeneu is LayerID ID)
            {               
                _leaveFight!.Destroy();

                Screen.RemoveFromAllPixels(ID);
                CurrentFight!.Pause(false);
                if (menue != MenueType.Fight)
                    CurrentFight!.EndEarly();
                Scene.ChangeMenue(MenueType.Fight);
            }

        }
    }
}

