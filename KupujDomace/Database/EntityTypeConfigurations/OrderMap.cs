using KupujDomace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KupujDomace.Database.EntityTypeConfigurations
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.FarmId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CreatedAt);
            builder.HasMany(x => x.Items).WithOne().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
