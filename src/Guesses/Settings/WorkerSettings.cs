namespace PalpiteFC.Worker.Guesses.Settings;

public sealed class WorkerSettings
{
    public TimeSpan ProcessGuessesAfter { get; set; }
    public TimeSpan LoopDelay { get; set; }
    public TimeSpan RestartDelay { get; set; }
    public TimeSpan CheckGameDelay { get; set; }
    public Points? Points { get; set; }

}

public sealed class Points
{
    public int HitResult { get; set; }
}