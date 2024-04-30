namespace PalpiteFC.DataContracts.MessageTypes;
public class GuessMessage
{
    public int FixtureId { get; set; }
    public int UserId { get; set; }
    public int HomeTeamId { get; set; }
    public int HomeTeamGoals { get; set; }
    public int AwayTeamId { get; set; }
    public int AwayTeamGoals { get; set; }
}
