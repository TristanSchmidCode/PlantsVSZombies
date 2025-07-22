using System.Diagnostics;

namespace PlantsVSZombies;


public static class Time
{
    public static event CycleDeligate? OnOneCycle;
    public delegate void CycleDeligate(float Time);
    public static float GameSpeed = 1;

    public static float DeltaTime 
    { 
        get { return deltaTime / 1000; } 
        private set { deltaTime = value; } 
    }
    static float deltaTime;
    static Time()
    {
        DeltaTime = 0;
    }

    public static void Ticker()
    {
        Scene scene = new Scene();
        scene.Start();
        while (true)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //The base of everything
            scene.Run();


            //I the game runs too fast, it can break;
            if (stopwatch.ElapsedMilliseconds < 2)
                Thread.Sleep(2);
            DeltaTime = stopwatch.ElapsedMilliseconds * GameSpeed;
            OnOneCycle?.Invoke(DeltaTime);
        }
    }
}

/// <summary>
///  A class used to inform its holder when amount of time has passed. 
///  Containes methods for menipulating
/// </summary>
public class Timer
{
    bool _paused;
    float _interval;
    public float timeSinceLast;
    /// <summary>
    /// this method is ran every frame
    /// </summary>
    /// <param name="Time"></param>
    private void Time_OnOneCycle(float Time)
    {
        if (!_paused) 
            timeSinceLast += Time;
    }
    private void Fight_OnFightPause(object? obj,bool Pause)
    {
        if (Pause)
            Stop();
        else
            Start();
    }
    /// <summary> Creats a new <see cref="Timer"/>, and Starts it </summary>
    public static Timer StartNew(float interval)
    {
        Timer t = new(interval);
        t.Start();
        return t;
    }
    /// <summary> Resets the timer, and starts it again </summary>
    public void ReStart()
    {
        Reset();
        Start();
    }
    public void Start() => _paused = false;
    public void Reset() => timeSinceLast = 0;
    
    /// <summary> Pauses the timer</summary>
    public void Stop() => _paused = true;
    
    /// <summary>
    ///  if the interval has passed, will return true, and change the interval 
    ///  to <paramref name="newInterval"/>
    ///  </summary>
    /// <returns>true if elapsed seconds is larger then interval, else false</returns>
    public bool Check(float newInterval)
    {
        if (timeSinceLast > _interval)
        {
            timeSinceLast -= _interval;
            _interval = newInterval;
            return true;
        }
        return false;
    }
    /// <summary>
    /// If <paramref name="change"/> is true, will set the timer back, else it won't do anything
    /// </summary>
    /// <param name="change"></param>
    /// <returns>true if elapsed seconds is larger then interval, else false</returns>
    public bool Check(bool change = true)
    {
        if (timeSinceLast <= _interval)
            return false; 
        if (change)
            timeSinceLast -= _interval;
        return true;
    }
    /// <summary>
    /// changes the interval to the given one
    /// </summary>
    public void NewInterval(float newInterval) => _interval = newInterval;
    

    public Timer(float interval, bool fighting = true)
    {
        this._interval = interval;
        timeSinceLast = 0;
        _paused = true;
        Time.OnOneCycle += Time_OnOneCycle;
        if (fighting)
            Fight.Instance.FightPause += Fight_OnFightPause;
    }

    

}
