using System.ComponentModel.DataAnnotations;

namespace Default_Project.Cores.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
