using Microsoft.Extensions.DependencyInjection;
using PalpiteFC.Worker.Repository.Interfaces;
using PalpiteFC.Worker.Repository.Repositories;

namespace PalpiteFC.Worker.Repository.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services)
    {
        services.AddTransient<DbSession>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        services.AddTransient<IFixturesRepository, FixturesRepository>();
        services.AddTransient<IGuessesRepository, GuessesRepository>();
        services.AddTransient<IUserPointsRepository, UserPointsRepository>();
        services.AddTransient<ILeaguesRepository, LeaguesRepository>();
        services.AddTransient<ITeamsGamesRepository, TeamsGameRepository>();
        services.AddTransient<ITeamsRepository, TeamsRepository>();
    }
}
