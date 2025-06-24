using Default_Project.Cores.Interfaces;
using Default_Project.Cores.Models;

namespace Default_Project.Cores
{
    public interface IUnitWork : IAsyncDisposable
    {
        IGenericRepo<TEntity> Repo<TEntity>() where TEntity : BaseEntity;
        Task<int> CompleteAsync();
    }
}
