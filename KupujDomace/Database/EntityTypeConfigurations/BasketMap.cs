using KupujDomace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KupujDomace.Database.EntityTypeConfigurations;

public class BasketMap : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.ToTable("Baskets");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId).IsUnique();

        builder
            .HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

    }

}
