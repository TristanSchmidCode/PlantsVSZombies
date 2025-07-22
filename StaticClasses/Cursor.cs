using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies;


public static class Cursor
{
    public static readonly ConsoleKey[] moveKeys =
        [
            ConsoleKey.UpArrow,
            ConsoleKey.DownArrow,
            ConsoleKey.LeftArrow,
            ConsoleKey.RightArrow
        ];
    static GradientType _gradient = new(-40, -40, -40);
    public static GradientType Gradient {
        get => _gradient;
        set {
            _gradient = value;
            if (Target != null)
            {
                Target.Frame.ChangeImage(_gradient);
                Target.Symol.ChangeImage(_gradient);
            }
        }
    }
    static Intoractible target;        
    public static Intoractible Target { 
        get => target;
        private set {
            if (target == value)
                return;
            target = value;
            _targetTakenKeybordFocus = false;
        } 
    }
    static bool _targetTakenKeybordFocus;
    /// <summary>
    ///  Moves the Cursor to the given Interactible, and takes the keybord focus
    /// </summary>
    public static void TakeKeybordFocus(Intoractible intor)
    {
        MoveCursor(intor);
        _targetTakenKeybordFocus = true;
    }
    /// <summary>
    /// Is the cursors position in plant pos, use <see cref="BordInfo.PlantPosition(ValueTuple{int, int})"/>
    /// to get it as a <see cref="Position.Pos"/>
    /// </summary>
    public static void MoveCursor(ConsoleKey key)
    {
        if (!moveKeys.Contains(key))
            return;
        if (Intoractible.NextIntor(key) is not Intoractible intor)
            return;
        MoveCursor(intor);
    }
    /// <summary> moves the cursor onto the given Intoractible. </summary>
    public static void MoveCursor(Intoractible moveTo)
    {
        if (moveTo == Target)
            return;
        GradientType? GradientColor = null;
        if (Target != null)
        {
            Target.Frame.ChangeImage(GradientColor, Target.shown);
            Target.Symol.ChangeImage(GradientColor, Target.shown);
        }

        Target = moveTo;
        Target.Frame.ChangeImage(Gradient);
        Target.Symol.ChangeImage(Gradient);

    }

    /// <summary>
    /// the cursor will pick up a sun if it hoveres over
    /// </summary>
    public static void CheckSunCollisions()  {
        if (Target is not PlantSpace plantSpace)
            return;
        foreach ((Position pos, _) in plantSpace.Frame.Image.Pixels)
        {
            if (!(pos + plantSpace.Frame.Pos).TryGetPixel(out Pixel pixel))
                continue;
            if (pixel[Layers.Sun] is not LayerID sunID)
                continue;

            if (EntityHanderler.Instance.GetEntity(sunID) is not Sun sun)
                throw new InvalidOperationException("The sunID should be a Sun, but it isn't.");
            sun.BeActedOn(new PickUP());
            return;
        }
    }

    static bool running = true;
    public static void End() { running = false; }
    /// <summary>
    /// Contains a list of all keys pressed. 
    /// Is reset when The actions are taken
    /// </summary>
    static readonly Queue<ConsoleKey> pressedKeys = [];
    /// <summary> Lock to prevent multiple threads from accessing the <see cref="pressedKeys"/> at the same time. </summary>
    static readonly object thisLock = new();
    /// <summary>
    /// Continuosly Reads the Key(s) pressed
    /// </summary>
    public static void Start()
    { 
        while (running)
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            //checkes if the key is an arrow
            if (moveKeys.Contains(key))
                lock (thisLock)
                {
                    pressedKeys.Enqueue(key);
                }                
            if (key == ConsoleKey.Enter || 
                    key == ConsoleKey.P || 
                    key == ConsoleKey.Escape)
                lock (thisLock)
                {
                    pressedKeys.Enqueue(key);
                }
        }
    }
    static void HandleKey(ConsoleKey key)
    {
        //if the target has taken the keybord focus, it will handle the key press (except for escape)
        if (_targetTakenKeybordFocus) {
            // on escape, losses the keybord focus
            if (key == ConsoleKey.Escape) {
                _targetTakenKeybordFocus = false;
                return;
            }
            Target.BeActedOn(new KeyPress(key));
        }
        else if (moveKeys.Contains(key)) {
            MoveCursor(key);
            return;
        }
        else if (key == ConsoleKey.Enter) {
            Target.BeActedOn(new Pressed());
        }
        else if (key == ConsoleKey.P) {
            //No implimintation for P key yet, 
        }
        else if (key == ConsoleKey.Escape)
        {
            Scene.Instance.SendMessage(Message.EscapePressed, new object());
        }
    }
    public static void HandleKeys()  
    {
        while (pressedKeys.Count != 0)
        {
            ConsoleKey key;
            lock (thisLock)
            {
                key = pressedKeys.Dequeue();
            }
            HandleKey(key);
        }
    }


 


#pragma warning disable CS8618
    static Cursor()
#pragma warning restore CS8618
    {
        List<Position> building = [];
        for (int i = -7; i <= 7; i++)
        for (int j = -5; j <=1; j++)
            building.Add((i, j));
        _gradient = new(-40, -40, -40);
    }
}
