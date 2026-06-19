using Vectarr.Palooza.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vectarr.Palooza.Context.Configuration
{
    public class UserAccountConfig : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder
            .HasOne(a => a.Application)
            .WithOne(a => a.User)
            .HasForeignKey<Application>(c => c.UserId);

            builder
            .HasOne(c => c.Referral)
            .WithMany()
            .HasForeignKey(c => c.ReferralId)
            .OnDelete(DeleteBehavior.Restrict); // Prevents cascading deletes
        }
    }
}
