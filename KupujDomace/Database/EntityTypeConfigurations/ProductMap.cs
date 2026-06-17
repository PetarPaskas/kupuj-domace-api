using KupujDomace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KupujDomace.Database.EntityTypeConfigurations
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.FarmId);
            builder.HasIndex(x => x.CategoryId);
            builder.HasIndex(x => x.SubcategoryId);
            builder.HasIndex(x => x.Name);

            builder.HasOne(x=>x.Category)
                .WithMany(x=>x.Products)
                .HasForeignKey(x=>x.CategoryId);

            builder.HasOne(x=>x.SubCategory)
                .WithMany(x=>x.Products)
                .HasForeignKey(x=>x.SubcategoryId);
        }
    }
}
