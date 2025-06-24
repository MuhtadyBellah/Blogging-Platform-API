using Default_Project.Cores.Models;
using Newtonsoft.Json.Linq;

namespace Default_Project.Cores.Interfaces
{
    public interface IGenericRepo<T> where T : BaseEntity
    {
        public Task<IEnumerable<T>> GetAllAsync(ISpecific<T> spec);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T?> GetBySpecAsync(ISpecific<T> spec);
        public Task<T?> GetByIdAsync(int id);
        public Task<int> GetCountAsync(ISpecific<T> spec);


        public Task AddAsync(T item);
        public Task AddRangeAsync(ICollection<T> items);
        public void Update(T item);
        public void Delete(T item);
    }
}
