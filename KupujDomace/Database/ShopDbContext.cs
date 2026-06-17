using KupujDomace.Database.EntityTypeConfigurations;
using KupujDomace.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace KupujDomace.Database
{
    public class ShopDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Farm> Farms => Set<Farm>();
        public DbSet<FarmPhoto> FarmPhotos => Set<FarmPhoto>();
        public DbSet<Certificate> Certificates => Set<Certificate>();
        public DbSet<Award> Awards => Set<Award>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Basket> Baskets => Set<Basket>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<BasketItem> BasketItems => Set<BasketItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();

        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.ApplyConfiguration(new BasketMap());
            b.ApplyConfiguration(new CategoryMap());
            b.ApplyConfiguration(new FarmMap());
            b.ApplyConfiguration(new OrderMap());
            b.ApplyConfiguration(new ProductMap());
            b.ApplyConfiguration(new UserMap());
            b.ApplyConfiguration(new UserSessionMap());
        }

    }
}
