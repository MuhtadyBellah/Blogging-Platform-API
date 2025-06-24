using Default_Project.Cores.Models;

namespace Default_Project.DTO
{
    public record BlogRequest(string title, string content, string category, List<string> tags)
    {}
}
