using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Default_Project.Cores.Models
{
    public class Has : BaseEntity
    {
        [ForeignKey(nameof(Blog))]
        public required int BlogId { get; set; }
        public virtual Blog? Blog { get; set; }

        [ForeignKey(nameof(Tag))]
        public required int TagId { get; set; }
        public virtual Tag? Tag { get; set; }
    }
}
