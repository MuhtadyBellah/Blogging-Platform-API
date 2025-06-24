using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Default_Project.Cores.Models;

namespace Default_Project.Repos.Data.Config
{
    public class hasConfig : IEntityTypeConfiguration<Has>
    {
        public void Configure(EntityTypeBuilder<Has> builder)
        {
            builder.Navigation(h => h.Tag)
                    .AutoInclude();

        }
    }
}

