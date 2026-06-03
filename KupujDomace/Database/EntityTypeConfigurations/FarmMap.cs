using Microsoft.EntityFrameworkCore;
using KupujDomace.Database.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KupujDomace.Database.EntityTypeConfigurations;

public class FarmMap: IEntityTypeConfiguration<Farm>
{
    public void Configure(EntityTypeBuilder<Farm> builder)
    {
        builder.ToTable("Farms");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => x.Name);

        builder.HasMany(x => x.Photos).WithOne().HasForeignKey(p => p.FarmId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Certificates).WithOne().HasForeignKey(c => c.FarmId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Awards).WithOne().HasForeignKey(a => a.FarmId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Products).WithOne(p => p.Farm!).HasForeignKey(p => p.FarmId).OnDelete(DeleteBehavior.Cascade);


    }
}
