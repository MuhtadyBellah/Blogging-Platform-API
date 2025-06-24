using System.Linq.Expressions;
using Default_Project.Cores.Models;

namespace Default_Project.Cores.Interfaces
{
    public interface ISpecific<T> where T : BaseEntity
    {
        //where(p=>p.)
        public Expression<Func<T, bool>> Creiteria { get; set; }

        // include(p=>p.).include(p=>p.)
        public List<Expression<Func<T, object>>> Includes { get; set; }

        // orderBy(p=>p.)
        public Expression<Func<T, object>> Order { get; set; }

        // orderByDesc(p=>p.)
        public Expression<Func<T, object>> OrderDesc { get; set; }

        // Take(2)
        public int Take { get; set; }

        // Skip(2)
        public int Skip { get; set; }
        public bool IsPagination { get; set; }
    }
}
