using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    public class SiteFunnelDailyCountConfig : IEntityTypeConfiguration<SiteFunnelDailyCount>
    {
        public void Configure(EntityTypeBuilder<SiteFunnelDailyCount> builder)
        {
            builder.HasKey(x => new { x.Date, x.Bucket });
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Bucket).IsRequired().HasMaxLength(32);
            builder.Property(x => x.Count).IsRequired();
        }
    }
}
