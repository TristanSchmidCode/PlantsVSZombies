global using RBGColors;
using System.Runtime.InteropServices;





namespace PlantsVSZombies;

public class Program
{
    const int STD_OUTPUT_HANDLE = -11;
    const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    static void EnableANSI()
    {
        var handle = GetStdHandle(STD_OUTPUT_HANDLE);
        if (!GetConsoleMode(handle, out uint mode))
            return;
        SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
    }

    static void Main()
    {
        EnableANSI();
        Console.OutputEncoding = System.Text.Encoding.Unicode;

        Console.CursorVisible = false;
        Console.CursorVisible = false;

       

        LevelSelector.Nothing();
        PlantSpace.Nothing();

        
        
        Time.Ticker();
        Cursor.End();
    }



}











