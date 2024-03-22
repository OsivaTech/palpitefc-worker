using PalpiteFC.Api.Domain.Entities.Database;

namespace PalpiteFC.Worker.Repository.Interface;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> Select();
    Task<TEntity> Select(int id);
    Task Insert(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(int id);
}