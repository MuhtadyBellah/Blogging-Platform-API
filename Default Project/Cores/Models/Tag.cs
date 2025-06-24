
namespace Default_Project.Cores.Models
{
    public class Tag : BaseEntity
    {
        public required string Name {  get; set; }
        public virtual ICollection<Has>? Has { get; set; }
    }
}
