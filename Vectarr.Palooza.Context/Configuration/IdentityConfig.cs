using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    public class IdentityConfig : IEntityTypeConfiguration<Identity>
    {
        public void Configure(EntityTypeBuilder<Identity> builder)
        {

            builder
.Property(p => p.TaxRate)
.HasPrecision(18, 4);
        }
    }
}
