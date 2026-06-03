using KupujDomace.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KupujDomace.Database.EntityTypeConfigurations;

public class UserSessionMap:IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("UserSessions");

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.SessionToken).IsUnique();
        builder.HasIndex(x => x.UserId);
    }
}
