using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    public class ReferralConfig : IEntityTypeConfiguration<Referral>
    {
        public void Configure(EntityTypeBuilder<Referral> builder)
        {
            builder
.HasOne(c => c.ReferredUser)
.WithMany() // If there isn't a collection of conversations on the ConfirmationConversation
.HasForeignKey(c => c.ReferredUserId)
.OnDelete(DeleteBehavior.Restrict); // Prevents cascading deletes
        }
    }
}
