public class Timer
{
    float curTime;

    public Timer()
    {
        curTime = 0;
    }

    public Timer(float startTime)
    {
        curTime = startTime;
    }

    public void Tick(float elapsedTime) => curTime += elapsedTime;

    public static implicit operator float(Timer timer) => timer.curTime;
}