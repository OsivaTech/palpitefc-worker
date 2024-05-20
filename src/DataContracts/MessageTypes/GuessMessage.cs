namespace PalpiteFC.DataContracts.MessageTypes;

public class GuessMessage
{
    public int FixtureId { get; set; }
    public int UserId { get; set; }
    public int HomeId { get; set; }
    public int HomeGoals { get; set; }
    public int AwayId { get; set; }
    public int AwayGoals { get; set; }
}