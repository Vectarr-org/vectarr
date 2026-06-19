using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vectarr.Palooza.Context.Configuration
{
    public class CreatorConfig : IEntityTypeConfiguration<Creator>
    {
        public void Configure(EntityTypeBuilder<Creator> builder)
        {
            builder.HasOne(c => c.UserAccount)
           .WithOne(u => u.Creator)
           .HasForeignKey<Creator>(c => c.UserAccountId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Jobs)
          .WithOne(j => j.Creator)
          .HasForeignKey(j => j.CreatorId)
          .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.WorkOrders)
       .WithOne(w => w.Creator)
       .HasForeignKey(w => w.CreatorId)
       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
