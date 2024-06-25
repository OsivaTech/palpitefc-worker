using Microsoft.Extensions.Options;
using PalpiteFC.Libraries.Persistence.Abstractions.Entities;
using PalpiteFC.Worker.Guesses.DataContracts;
using PalpiteFC.Worker.Guesses.Enums;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Settings;
using PalpiteFC.Worker.Integrations.Providers.Responses;

namespace PalpiteFC.Worker.Guesses.Services;

public class PointsService : IPointsService
{
    #region Fields

    private readonly IOptions<WorkerSettings> _workerSettings;

    #endregion

    #region Constructor

    public PointsService(IOptions<WorkerSettings> workerSettings)
    {
        _workerSettings = workerSettings;
    }

    #endregion

    #region Public Methods

    public IEnumerable<PointsResult> CalculatePoints(Guess guess, FixtureResponse fixture)
    {
        var pointsResult = new List<PointsResult>();

        var isExactScore = IsExactScorePrediction(guess, fixture);
        var isCorrectGoalDiff = IsCorrectGoalDifferencePrediction(guess, fixture);
        var isValidResult = IsMatchWinnerPrediction(guess, fixture);
        var isEarlierGuess = IsEarlyPrediction(guess, fixture);

        if (isExactScore)
        {
            pointsResult.Add(new PointsResult
            {
                Type = PointType.ES,
                Points = _workerSettings.Value.Points!.ExactScore
            });
        }
        else if (isCorrectGoalDiff)
        {
            pointsResult.Add(new PointsResult
            {
                Type = PointType.GD,
                Points = _workerSettings.Value.Points!.GoalDifference
            });
        }
        else if (isValidResult)
        {
            pointsResult.Add(new PointsResult
            {
                Type = PointType.MW,
                Points = _workerSettings.Value.Points!.MatchWinner
            });
        }

        if (isEarlierGuess)
        {
            pointsResult.Add(new PointsResult
            {
                Type = PointType.EB,
                Points = _workerSettings.Value.Points!.EarlyBonus
            });
        }

        return pointsResult;
    }

    #endregion

    #region Non-Public Methods

    private static bool IsExactScorePrediction(Guess guess, FixtureResponse fixture)
    {
        var actualHomeGoals = fixture.Goals?.Home ?? 0;
        var actualAwayGoals = fixture.Goals?.Away ?? 0;

        return guess.HomeGoals == actualHomeGoals && guess.AwayGoals == actualAwayGoals;
    }

    private static bool IsCorrectGoalDifferencePrediction(Guess guess, FixtureResponse fixture)
    {
        var actualHomeGoals = fixture.Goals?.Home ?? 0;
        var actualAwayGoals = fixture.Goals?.Away ?? 0;

        var guessGoalDifference = Math.Abs(guess.HomeGoals - guess.AwayGoals);
        var actualGoalDifference = Math.Abs(actualHomeGoals - actualAwayGoals);

        return (guessGoalDifference != 0 || actualGoalDifference != 0) && guessGoalDifference == actualGoalDifference;
    }

    private static bool IsMatchWinnerPrediction(Guess guess, FixtureResponse fixture)
    {
        var actualHomeGoals = fixture.Goals?.Home ?? 0;
        var actualAwayGoals = fixture.Goals?.Away ?? 0;

        var guessHomeWin = guess.HomeGoals > guess.AwayGoals;
        var guessAwayWin = guess.HomeGoals < guess.AwayGoals;
        var guessDraw = guess.HomeGoals == guess.AwayGoals;

        var actualHomeWin = actualHomeGoals > actualAwayGoals;
        var actualAwayWin = actualHomeGoals < actualAwayGoals;
        var actualDraw = actualHomeGoals == actualAwayGoals;

        return (guessHomeWin && actualHomeWin) || (guessAwayWin && actualAwayWin) || (guessDraw && actualDraw);
    }

    private bool IsEarlyPrediction(Guess guess, FixtureResponse fixture)
    {
        var fixtureStartTime = fixture.Fixture?.Date ?? DateTime.MinValue;

        return guess.GuessDate <= fixtureStartTime.Add(-_workerSettings.Value.EarlyBonusTime);
    }

    #endregion
}