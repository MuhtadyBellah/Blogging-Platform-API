
using Default_Project.Cores.Models;

namespace Default_Project.Cores.Specifications
{
    public class BlogSpecific : BaseSpecification<Blog>
    {
        public BlogSpecific(BlogSpecParams param) :
            base(p =>
                (string.IsNullOrEmpty(param.term) || p.Has.Any(x => x.Tag != null &&
                                                                x.Tag.Name != null && 
                                                                x.Tag.Name.ToLower().StartsWith(param.term.ToLower())))
            )
        {
            Includes.Add(p => p.Has);
            switch (param.Sort)
            {
                case SortOptions.IdDesc:
                    OrderByDesc(p => p.Id);
                    break;
                case SortOptions.Category:
                    OrderBy(p => p.Category);
                    break;
                case SortOptions.CategoryDesc:
                    OrderByDesc(p => p.Category);
                    break;
                case SortOptions.Created:
                    OrderBy(p => p.createdAt);
                    break;
                case SortOptions.Modified:
                    OrderBy(p => p.updatedAt);
                    break;
                default:
                    OrderBy(p => p.Id); // Default case if no sort option is provided
                    break;
            }
        }

        public BlogSpecific(int id) : base(p => p.Id == id)
        {
            Includes.Add(p => p.Has);
        }
        public BlogSpecific(List<int> ids) : base(p => ids.Contains(p.Id))
        {
            Includes.Add(p => p.Has);
        }
    }
}
