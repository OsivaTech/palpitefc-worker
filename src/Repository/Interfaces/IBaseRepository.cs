using PalpiteFC.Worker.Repository.Entities;

namespace PalpiteFC.Worker.Repository.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> Select();
    Task<TEntity> Select(int id);
    Task Insert(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(int id);
}