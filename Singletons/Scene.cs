using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static PlantsVSZombies.Fight;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace PlantsVSZombies;
public enum MenueType
{
    FightMenue          = 0,
    LevelSelectMenue    = 1,
    MainMenue           = 2,
    SaveScreenMenue     = 3,
    HelpMenue           = 4,
}
public enum Message
{
    /// <summary>
    /// requires a data of type LevelType
    /// </summary>
    OpenFightMenue          = 0,
    OpenLevelSelectMenue    = 1,
    OpenMainMenue           = 2,
    OpenSaveScreenMenue     = 3,
    OpenHelpMenue           = 4,
    OpenSettings,
    CloseSettings,
    /// <summary>
    /// requires a data of type FightEndState
    /// </summary>
    EndFight,
    EscapePressed,
}
public enum SettingsMessage
{
    Open,
    CloseReturnToMenue,
    Close
}
public enum FightEndState
{
    FightWon,
    FightLost,
    FightLeft
}

public class Scene: SingleTone<Scene>
{
    Menue? _currentMenue;
    SettingsMenue? settings;
    public readonly LevelHanderler  LevelHanderler = new();

    void ChangeMenue(MenueType menue, object? data = null)
    {
        if (menue == Menue)
            return;
        if (settings != null)
            HandleMessage(Message.CloseSettings, null);
        if (Menue == menue)
            return;
        Instance._currentMenue?.Close();
        Instance._currentMenue = null;
        switch (menue)
        {
            case MenueType.LevelSelectMenue:
                Instance._currentMenue = LevelSelectMenue.OpenMenue();
                break;
            case MenueType.MainMenue:
                Instance._currentMenue = MainMenue.OpenMenue();
                break;
            case MenueType.SaveScreenMenue:
                Instance._currentMenue = BlankMenue.OpenMenue();
                break;
            case MenueType.HelpMenue:
                Instance._currentMenue = HelpMenue.OpenMenue();
                break;
            case MenueType.FightMenue:
                if (data is not LevelType level)
                    throw new ArgumentException("Data must be of type LevelType", nameof(data));
                Instance._currentMenue = FightMenue.OpenMenue(level);
                break;
        }
    }    
    void HandleMessage(Message message, object? data = null)
    {
        switch (message)
        {
            case Message.OpenFightMenue:   
            case Message.OpenLevelSelectMenue:
            case Message.OpenMainMenue:
            case Message.OpenSaveScreenMenue:
            case Message.OpenHelpMenue:
                ChangeMenue((MenueType)message, data);
                break;
            case Message.OpenSettings:
                if (settings != null)
                    return;
                _currentMenue?.SetShow(false);
                settings = SettingsMenue.OpenMenue();
                break;
            case Message.CloseSettings:
                if (settings == null)
                    return;    
                _currentMenue?.SetShow(true);
      
                settings.Close();  
                settings = null;
                break;
            case Message.EndFight:
                if (data is not FightEndState endState)
                    throw new ArgumentException("Data must be of type FightEndState", nameof(data));

                Fight fight = Fight.Instance;
                if (endState == FightEndState.FightWon)
                {
                    fight.End(fightWon: true);
                    (ColorType, ColorType, ColorType) color = (Colors.DarkGreen, Colors.Green, Colors.Black);
                    ChangeMenue(MenueType.SaveScreenMenue);
                    new Image("You Won", color, true).Print((15, 20));
                    if (fight.FinalMessage != string.Empty)
                        new Image(fight.FinalMessage, color).Print((15, 28));
                }
                else if (endState == FightEndState.FightLost)
                {

                    fight.End(fightWon: false);
                    (ColorType, ColorType, ColorType) color = (new(110, 0, 0), Colors.Red, Colors.Black);
                    ChangeMenue(MenueType.SaveScreenMenue);
                    new Image("You Lost", color, true).Print((15, 20));
                    if (fight.FinalMessage != string.Empty)
                        new Image(fight.FinalMessage, color).Print((15, 28));
                }
                else if (endState == FightEndState.FightLeft) {
                    fight.End(fightWon: false);
                    ChangeMenue(MenueType.MainMenue);
                }
                break;
            case Message.EscapePressed:
                if (settings == null)
                    HandleMessage(Message.OpenSettings);
                else
                    HandleMessage(Message.CloseSettings);
                break;

        }
    }
    void HandleMessages()
    {
        while (_messages.Count != 0)
        {
            Message message;
            object? data;

            lock (_lock)
            {
                (message, data) = _messages.Dequeue();
            }
            HandleMessage(message,data);
        }
    }



    readonly Queue<(Message message, object? data)> _messages = [];
    static readonly object _lock = new object();
    public void SendMessage(Message message, object? data = null) {
        lock(_lock)
        {
            _messages.Enqueue((message, data));
        }
    }
    public void SendChangeMenueMessage(MenueType menue, object? data = null)
    {
        SendMessage((Message)menue, data);
    }
    public MenueType Menue => _currentMenue!.MenueType;
    public void Start()
    {
        _currentMenue = MainMenue.OpenMenue();
        Thread thread = new(Cursor.Start);
        thread.Start();
        Thread.Sleep(1000);
    }



    public void Run()
    {        
        Cursor.HandleKeys();

        while (_messages.Count != 0)
            HandleMessages();

        if (Menue == MenueType.FightMenue)
        {
            EntityHanderler.Instance.RegisterAndUnrgisterAll();
            Cursor.CheckSunCollisions();
            Fight.Instance.ActivateAll();
        }
    }
    protected override void DestroyThis()
    {
        _currentMenue?.Close();
        _currentMenue = null;
    }
}
public abstract class Menue
{
    public abstract MenueType MenueType { get; }
    protected List<ImageHaver> _images = [];

    protected List<Intoractible> _intoractibles = [];
    public virtual void Close()
    {
        foreach (var image in _images)
            image.RemoveImage();
        foreach (var intoractible in _intoractibles)
            intoractible.Destroy();
    }
    public virtual void SetShow(bool show)
    {
        if (show) {
            foreach (var image in _images)
                image.PrintImage();
            foreach (var intoractible in _intoractibles)
                intoractible.Show();
        }
        else  {
            foreach (var image in _images)
                image.RemoveImage();
            foreach (var intoractible in _intoractibles)
                intoractible.Hide();
        }
    }
}
public class MainMenue : Menue
{
    public override MenueType MenueType => MenueType.MainMenue;
    static Position GraveStonePos => (BordInfo.MapLeft / 2 + 20, 25);
    public static MainMenue OpenMenue()
    {
        return new MainMenue();
    }
    void MakeImages()
    {
        ImageHaver MakeZombie(Position pos)
        {
            Random rnd = new();
            int k = new Random().Next(0, 10);
            Image image;
            if (k < 6)
                image = Image.GetImage(Layers.BZombie, "BaseZombie");
            else if (k < 8)
                image = Image.GetImage(Layers.BZombie, "FatZombie");
            else
                image = Image.GetImage(Layers.BZombie, "TractorZombie");
            return new ImageHaver(image, Layers.Forground, pos);
        }
        _images = [
            new ImageHaver(Image.GetImage(Layers.Forground, "Tital"),
               Layers.Forground, (BordInfo.MapLeft / 2 - 4, 5)),
            new ImageHaver(Image.GetImage(Layers.Frame, "GraveStone").TrueRandomiseImage(),
                Layers.Forground, GraveStonePos),
            MakeZombie((32, 20)),
            MakeZombie((11, 25)),
            MakeZombie((120, 32)),
            MakeZombie((23, 22)),
            MakeZombie((83, 12)),
            MakeZombie((22, 37)),
            MakeZombie((105, 35)),
            MakeZombie((95, 12)),
            MakeZombie((135, 25)),
            MakeZombie((65, 19)),
        ];

    }
    void MakeRiver()
    {
        CreatLine((0, 40), (BordInfo.MapLeft, 21), new(Colors.PukeBroun), 15);
        for (int i = 40; i > 33; i--)
            CreatLine((0, i), (BordInfo.MapLeft, i / 2), new(Colors.Blue), 15);
        CreatLine((0, 33), (BordInfo.MapLeft, 16), new(Colors.PukeBroun), 15);
    }
    void MakeIntoractibles()
    {
        _intoractibles = [
            new MenueChanger(GraveStonePos + (-9, -2), (9,1), MenueType.LevelSelectMenue, 
                new Image("Start", (Colors.DarkGreen, Colors.Green, Colors.Gray))),
            new MenueChanger(GraveStonePos + (-7, 2), (8, 1), MenueType.HelpMenue, 
                new Image("Help", (Colors.DarkGreen, Colors.Green, Colors.Green))),
            new MessageSender(GraveStonePos + (-3, 4), (3,2),(Message.OpenSettings,null), 
                new Image("Settings",Colors.Gray,Colors.Red))
        ];
    }
    MainMenue()
    {        
        ChangeBackground(Background.MainMenue);
        MakeImages();
        MakeRiver();
        MakeIntoractibles();
        Cursor.MoveCursor(_intoractibles![0]);
    }
}
public class LevelSelectMenue :Menue
{
    public override MenueType MenueType => MenueType.LevelSelectMenue;
    public static LevelSelectMenue OpenMenue()
    {
       return new LevelSelectMenue();
    }

    void AddSelectors()
    {
        var levelHanderler = Scene.Instance.LevelHanderler;
        LevelType level = new(levelHanderler.Map, 1);
        if (!level.IsValid())
            return;

        bool stop = false;
        LevelSelector? previos = null;
        MakeSelector(new(30, 15));
        MakeSelector(new(50, 30));
        MakeSelector(new(55, 20));
        void MakeSelector(Position pos)
        {
            if (stop)
                return;
            LevelSelector current = new(pos, level, levelHanderler[level]);
            if (previos is LevelSelector prev)
            {
                Screen.CreatLine(previos.Frame.Pos, current.Frame.Pos, new(Colors.OddArmour));
            }

            previos = current;
            level = level + 1;
            if (!level.IsValid())
                stop = true;
            if (level.Map != levelHanderler.Map)
                stop = true;
            _intoractibles.Add(current);
        }
    }
    LevelSelectMenue()
    { 
        ChangeBackground(Background.Map1);

        _images.Add(new ImageHaver(
            Image.GetImage(Layers.Forground, "Tital"), Layers.Forground, (BordInfo.MapLeft / 2 - 4, 5)));

        _intoractibles.Add(new MenueChanger((1, 38), (6, 0), MenueType.MainMenue, 
            new Image("Back", ( new(110, 0, 0), Colors.Red, Colors.Green ) ) ) );

        AddSelectors();
        Cursor.MoveCursor(_intoractibles[0]);
    }

}
public class SettingsMenue 
{
    protected List<ImageHaver> _images = [];

    protected List<Intoractible> _intoractibles = [];

    static int _indexOfCursorColor;
    
    readonly GradientType[] _cursorColors = [new(-30,-30,-30),new(-60,-60,-60),new(100,-20,-20), new(-20,100,-20), new(-20, -20, 100)];

    Intoractible gameSpeed = null!;
    
    void ChangeGameSpeed(bool up)
    {
        if (Time.GameSpeed < 2f)
        {
            if (up)
                Time.GameSpeed += 0.2f;
            else if (Time.GameSpeed > 0.4f)
                Time.GameSpeed -= 0.2f;
        }
        else if (Time.GameSpeed >= 2f)
        {
            if (up)
                Time.GameSpeed += 1;
            else if (Time.GameSpeed > 2f)
                Time.GameSpeed -= 1f;
            else
                Time.GameSpeed -= 0.2f;
        }
        Time.GameSpeed = MathF.Round(Time.GameSpeed, 2);
        gameSpeed.Symol.ChangeImage(new Image($"{{{Time.GameSpeed}}}", BordInfo.GreenText), true);
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
        Cursor.Gradient = _cursorColors[_indexOfCursorColor];
    }

    public static SettingsMenue OpenMenue()
    {
        return new SettingsMenue();
    }
    void IsFightingMenue()
    {
        Fight.Instance.Pause(true);
        _intoractibles.Add(new MessageSender((1, 40), (15, 0), (Message.CloseSettings,null),
            new Image("Go Back", BordInfo.RedText), Layers.InnerIntoractible));
        _intoractibles.Add(new MessageSender((117, 40), (15, 0), (Message.EndFight,FightEndState.FightLeft),
            new Image("Main Menue", BordInfo.RedText),      Layers.InnerIntoractible));
    }
    void MakeGameSpeedIntoractible()
    {
        gameSpeed = new SettingIntor(
            pos: (1, 15),
            fromCenter: (50, 0), //makes sure the cursor is in the blank space
            @delegate: ChangeGameSpeed,
            layer: Layers.InnerIntoractible,
            symol: new Image($"{{{Time.GameSpeed}}}", BordInfo.GreenText),
            frame: new Image("Game Speed ", BordInfo.GreenText)
        );
        _intoractibles.Add(gameSpeed);
        gameSpeed.Symol.MoveBy((45, 0));// makes sure the symbol is where it should be
    }
    void MakeChangeCursorColorIntoractible()
    {
        (Position, PixelType)[] Space = new (Position, PixelType)[15 * 7];
        for (int x = 0; x < 15; x++)
            for (int y = 0; y < 7; y++)
                Space[y * 15 + x] = (new(x - 56, y - 4), new(Colors.White, Colors.Black, ' '));
        _intoractibles.Add(new SettingIntor(
            pos: (1, 22),
            fromCenter: (63, 1), //makes sure the cursor is in the blank space
            @delegate: ChangeCursorColor,
            layer: Layers.InnerIntoractible,
            symol: new Image(Space),
            frame: new Image("Cursor Color {    }", BordInfo.GreenText)
        ));
    }
    public SettingsMenue()
    {
        ChangeBackground(Background.SettingMenue);

        if (Fight.TryGetInstance(out _)) 
            IsFightingMenue();
        else
            _intoractibles.Add(new MessageSender((1, 40), (15, 0), (Message.CloseSettings,null),
                new Image("Leave", BordInfo.RedText), Layers.InnerIntoractible));

        _images.Add(new ImageHaver(Image.GetImage(Layers.Forground, "Tital"), Layers.InnerIntoractible,
            (BordInfo.MapLeft / 2 - 4, 5)));
        MakeGameSpeedIntoractible();
        MakeChangeCursorColorIntoractible();
    }

    public void Close()
    {
        foreach (var image in _images)
            image.RemoveImage();
        foreach (var intoractible in _intoractibles)
            intoractible.Destroy();
        Screen.CloseOverTheTopBackGround();
        if (Fight.TryGetInstance(out Fight fight))
            fight.Pause(false);
    }
}
public class BlankMenue : Menue
{
    public override MenueType MenueType => MenueType.SaveScreenMenue;
    public static BlankMenue OpenMenue() => new BlankMenue();
    BlankMenue()
    {
        ChangeBackground(Background.PureBlack);
        _images.Add(new ImageHaver(Image.GetImage(Layers.Forground, "Tital"), Layers.Forground, (BordInfo.MapLeft / 2 - 4, 5)));
        _intoractibles.Add(new MenueChanger(MenueType.LevelSelectMenue));
        Cursor.MoveCursor(_intoractibles[0]);
    }
}
public class HelpMenue : Menue
{
    public override MenueType MenueType => MenueType.HelpMenue;
    public static HelpMenue OpenMenue() => new HelpMenue();
    HelpMenue()
    {
        ChangeBackground(Background.PureBlack);
        var d = (Colors.DarkGreen, Colors.Green, Colors.Black);
        new Image("You are playing Plants vs. Zombies,PvZ", d, false).Print((2, 5));
        new Image("This game is about defending agains Zombies", d, false).Print((2, 9));
        new Image("To do so, you plant plants down, ", d, false).Print((2, 13));
        new Image("each of which cost sun", d, false).Print((2, 17));
        new Image("Move your mouse with the arrow keys,", d, false).Print((2, 21));
        new Image("and select things with the enter key", d, false).Print((2, 25));
        new Image("Have fun", d, false).Print((2, 27));

        _intoractibles.Add(new MenueChanger((70, 38), (6, 0), MenueType.MainMenue,
            new("Leave", (new(110, 0, 0), Colors.Red, Colors.Black))));

        Cursor.MoveCursor(_intoractibles[0]);
    }
}



public class FightMenue : Menue{
    public override MenueType MenueType => MenueType.FightMenue;
    readonly PlantHanderler handerler;    
    readonly Money money;

    readonly EntityHanderler entityHanderler;
    readonly Fight fight;
    public static FightMenue OpenMenue(LevelType level)
    {
        return new FightMenue(level);
    }
   
    FightMenue(LevelType level)
    {
        Screen.ChangeBackground(Background.grassFight);
        fight = LevelHanderler.CreateFight(level);
        entityHanderler = new();
        handerler = new();
        money = new(sunAmount: 100);
        fight.Start();
    }
    public override void Close()
    {
        base.Close();
        handerler.Destroy();
        fight.Destroy();
        entityHanderler.Destroy();
        money.Destroy();
    }
    //so that the cursor can be moved to the last target when the settings are closed
    Intoractible? previosTarget;

    public override void SetShow(bool show)
    {
        base.SetShow(show);
        if (show) {
            handerler.SetShow(show);
            if (previosTarget != null) 
                Cursor.MoveCursor(previosTarget);
            
        
        }
        else {
            handerler.SetShow(false);
            previosTarget = Cursor.Target;
        }
    }
}
