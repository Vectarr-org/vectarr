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
    public class BidConfig : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            // Relationship: Bid -> Job (many-to-one)
            builder.HasOne(b => b.Job)
                .WithMany(j => j.Bids)
                .HasForeignKey(b => b.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship: Bid -> Workshop (many-to-one)
            builder.HasOne(b => b.Workshop)
                .WithMany(w => w.Bids)
                .HasForeignKey(b => b.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
