using Default_Project.Cores.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Default_Project.DTO
{
    public record BlogDTO(int Id, string Title, string Content, string Category, string createdAt, string updatedAt)
    {
        public List<string> tagNames { get; set; } = new List<string>();
    }
}
