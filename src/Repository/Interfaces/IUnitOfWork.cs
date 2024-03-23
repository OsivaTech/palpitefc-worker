namespace PalpiteFC.Worker.Repository.Interfaces;
public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}
