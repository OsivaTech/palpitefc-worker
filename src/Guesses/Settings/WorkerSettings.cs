namespace PalpiteFC.Worker.Guesses.Settings;

public sealed class WorkerSettings
{
    public TimeSpan ProcessGuessesAfter { get; set; }
    public TimeSpan ReprocessAfter { get; set; }
    public TimeSpan LoopDelay { get; set; }
    public TimeSpan RestartDelay { get; set; }
    public TimeSpan EarlyBonusTime { get; set; }
    public Points? Points { get; set; }
}

public sealed class Points
{
    public int ExactScore { get; set; }
    public int GoalDifference { get; set; }
    public int MatchWinner { get; set; }
    public int EarlyBonus { get; set; }
}