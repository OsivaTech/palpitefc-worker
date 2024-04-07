namespace PalpiteFC.Worker.Repository.Connection;
public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}
