using System.ComponentModel;

namespace PalpiteFC.Worker.Guesses.Enums;

public enum PointType
{
    [Description("ExactScore")] ES,
    [Description("GoalDifference")] GD,
    [Description("MatchWinner")] MW,
    [Description("EarlyBonus")] EB
}