using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    public class WorkOrderConfig : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            // Job relationship (one-to-one)
            builder.HasOne(w => w.Job)
                .WithOne(j => j.WorkOrder)
                .HasForeignKey<WorkOrder>(w => w.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            // Accepted Bid relationship
            builder.HasOne(w => w.AcceptedBid)
                .WithMany()
                .HasForeignKey(w => w.AcceptedBidId)
                .OnDelete(DeleteBehavior.Restrict);

            // Creator relationship
            builder.HasOne(w => w.Creator)
                .WithMany(c => c.WorkOrders)
                .HasForeignKey(w => w.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Workshop relationship
            builder.HasOne(w => w.Workshop)
                .WithMany(ws => ws.WorkOrders)
                .HasForeignKey(w => w.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict);

            // Collections
            builder.HasMany(w => w.ChatMessages)
                .WithOne(m => m.WorkOrder)
                .HasForeignKey(m => m.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.QuoteRevisions)
                .WithOne(q => q.WorkOrder)
                .HasForeignKey(q => q.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.ProgressStages)
                .WithOne(p => p.WorkOrder)
                .HasForeignKey(p => p.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.QualityCheckRequests)
           .WithOne(p => p.WorkOrder)
           .HasForeignKey(p => p.WorkOrderId)
           .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.AdditionalFiles)
                .WithOne(i => i.WorkOrder)
                .HasForeignKey(i => i.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
