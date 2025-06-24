using Default_Project.Cores.Interfaces;
using Default_Project.Cores.Models;
using Default_Project.Cores.Specifications;
using Default_Project.Repos.Data;
using Microsoft.EntityFrameworkCore;

namespace Default_Project.Repos
{
    public class GenericRepo<T> : IGenericRepo<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;
        public GenericRepo(StoreContext dbContext) => _dbContext = dbContext;

        private IQueryable<T> Spec(ISpecific<T> spec)
            => SpecificEvalutor<T>.GetQuery(_dbContext.Set<T>(), spec);

        public async Task<IEnumerable<T>> GetAllAsync(ISpecific<T> spec)
            => await Spec(spec).ToListAsync();

        public async Task<T?> GetBySpecAsync(ISpecific<T> spec)
            => await Spec(spec).FirstOrDefaultAsync();

        public async Task<int> GetCountAsync(ISpecific<T> spec)
            => await Spec(spec).CountAsync();

        public async Task AddAsync(T item) => await _dbContext.Set<T>().AddAsync(item);

        public async Task AddRangeAsync(ICollection<T> items) => await _dbContext.Set<T>().AddRangeAsync(items);
        public void Update(T item) => _dbContext.Set<T>().Update(item);
        public void Delete(T item) => _dbContext.Set<T>().Remove(item);

        #region Without Specification
        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbContext.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(int id)
            => await _dbContext.Set<T>().FindAsync(id);
        #endregion
    }
}
