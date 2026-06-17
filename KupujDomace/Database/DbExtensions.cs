using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

namespace KupujDomace.Database
{
    public static class DbExtensions
    {
        public static IServiceCollection RegisterShopContext(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (String.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured");

            }
            builder.Services.AddDbContext<ShopDbContext>(o => o.UseSqlServer(connectionString));

            return builder.Services;
        }
    }
}
