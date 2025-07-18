using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVSZombies
{
    
    public static class Cursor
    {
        public static readonly ConsoleKey[] moveKeys =
            [
                ConsoleKey.UpArrow,
                ConsoleKey.DownArrow,
                ConsoleKey.LeftArrow,
                ConsoleKey.RightArrow
            ];
        public static readonly Gradient gradient;

        public static Intoractible Current { get; private set; }

        /// <summary>
        /// Is the cursors position in plant pos, use <see cref="BordInfo.PlantPosition(ValueTuple{int, int})"/>
        /// to get it as a <see cref="Position.Pos"/>
        /// </summary>

        public static void Move(ConsoleKey key)
        {
            if (key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow ||
                key == ConsoleKey.LeftArrow|| key == ConsoleKey.RightArrow)
            { 
                if (Intoractible.NextIntor(key) is not Intoractible intor)
                    return;

                Move(intor);
                Current = intor;
            }
        }
        public static void Move(Intoractible moveTo)
        {
            if (moveTo != Current)
            {
                gradient.MoveGradient(new Position(moveTo.frame.Pos + moveTo.Displacement));
                Current = moveTo;
            }
        }

        /// <summary>
        /// the cursor will pick up a sun if it hoveres over
        /// </summary>
        public static void CheckIntoractions()
        {
            foreach (var d in gradient.GradientPoses)
                if (!(d+gradient.Pos).IsOutOfBounds())
                    if ((d+gradient.Pos).GetPixel()[Layers.Sun] is LayerID sun)
                    {
                        AllEntitys.Get<Sun>()[sun].BeActedOn(new PickUP());
                    }
              
        }

        static bool end = false;
        public static void End()
        {
            end = true;
        }
        /// <summary>
        /// Continuosly Reads the Key(s) pressed
        /// </summary>
        public static void Start()
        { 
            while (true)
            {
                if (end)
                    return;
                
                ConsoleKey key = Console.ReadKey(true).Key;
                //checkes if the key is an arrow
                if (moveKeys.Contains(key))
                    pressedKeys.Add(key);
                
                if (key == ConsoleKey.Enter || key == ConsoleKey.P || key == ConsoleKey.Escape)
                    pressedKeys.Add(key);
                

            }
        }
        /// <summary>
        /// Contains a list of all keys pressed. 
        /// Is reset when The actions are taken
        /// </summary>
        static readonly List<ConsoleKey> pressedKeys = [];

        public static List<ConsoleKey> ReadKeys()
        {
            List<ConsoleKey> keys = [];
            if (pressedKeys.Count == 0)
                return keys;

            keys.AddRange(pressedKeys);
            pressedKeys.Clear();
            return keys;
        }
#pragma warning disable CS8618
        static Cursor()
#pragma warning restore CS8618
        {
            List<Position> building = [];
            for (int i = -7; i <= 7; i++)
                for (int j = -5; j <=1; j++)
                    building.Add((i, j));
                
            GradientType cursorColor = new(-40, -40, -40);
            gradient = new(Layers.Cursor, (0, 0).PlantPosition(), building, cursorColor);

        }
    }
}
