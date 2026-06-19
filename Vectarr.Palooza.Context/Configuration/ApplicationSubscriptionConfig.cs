using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    public class ApplicationSubscriptionConfig : IEntityTypeConfiguration<ApplicationSubscription>
    {
        public void Configure(EntityTypeBuilder<ApplicationSubscription> builder)
        {

        }
    }
}
