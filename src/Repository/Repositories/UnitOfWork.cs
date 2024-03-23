using PalpiteFC.Worker.Repository.Interfaces;

namespace PalpiteFC.Worker.Repository.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    #region Fields

    private readonly DbSession _session;

    #endregion

    #region Constructor

    public UnitOfWork(DbSession session)
    {
        _session = session;
    }

    #endregion

    #region Public Methods

    public void BeginTransaction()
    {
        _session.Transaction = _session.Connection.BeginTransaction();
    }

    public void Commit()
    {
        _session.Transaction.Commit();
        Dispose();
    }

    public void Rollback()
    {
        _session.Transaction.Rollback();
        Dispose();
    }

    public void Dispose() => _session.Transaction?.Dispose();

    #endregion
}