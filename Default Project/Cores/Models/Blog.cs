
namespace Default_Project.Cores.Models
{
    public class Blog : BaseEntity
    {
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string Category { get; set; }
        public DateTimeOffset createdAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset updatedAt { get; set; } = DateTimeOffset.UtcNow;
        public virtual ICollection<Has>? Has { get; set; }
    }
}
