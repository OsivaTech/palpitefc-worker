using PalpiteFC.Worker.Guesses.Enums;

namespace PalpiteFC.Worker.Guesses.DataContracts;

public class PointsResult
{
    public PointType? Type { get; set; }
    public int Points { get; set; }
}