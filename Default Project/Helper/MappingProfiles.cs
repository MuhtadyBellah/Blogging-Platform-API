using AutoMapper;
using Default_Project.Cores.Models;
using Default_Project.DTO;
using System.Reflection.Metadata;

namespace Default_Project.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Blog, BlogDTO>()
                .ForMember(d => d.tagNames, o => o.MapFrom(s => s.Has.Select(x => x.Tag.Name).ToList()))
                .ForMember(d => d.createdAt, o => o.MapFrom(s => s.createdAt.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")))
                .ForMember(d => d.updatedAt, o => o.MapFrom(s => s.updatedAt.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")));
        }
    }
}
