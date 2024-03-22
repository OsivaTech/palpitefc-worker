using Microsoft.Extensions.DependencyInjection;
using PalpiteFC.Worker.Repository.Interface;
using PalpiteFC.Worker.Repository.Repositories;

namespace PalpiteFC.Worker.Repository;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services)
    {
        services.AddTransient<DbSession>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IFixturesRepository, FixturesRepository>();
        services.AddTransient<IGuessesRepository, GuessesRepository>();
        services.AddTransient<IUserPointsRepository, UserPointsRepository>();
    }
}
