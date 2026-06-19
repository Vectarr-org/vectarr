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
    public class JobConfig : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasOne(j => j.Creator)
         .WithMany(c => c.Jobs)
         .HasForeignKey(j => j.CreatorId)
         .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(j => j.Bids)
          .WithOne(b => b.Job)
          .HasForeignKey(b => b.JobId)
          .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.QualityChecks)
 .WithOne(b => b.Job)
 .HasForeignKey(b => b.JobId)
 .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(j => j.WinningBid)
            .WithOne()
           .HasForeignKey<Job>(j => j.WinningBidId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(j => j.WorkOrder)
        .WithOne(w => w.Job)
        .HasForeignKey<WorkOrder>(w => w.JobId)
        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
