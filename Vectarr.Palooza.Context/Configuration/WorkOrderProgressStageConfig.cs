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
    public class WorkOrderProgressStageConfig : IEntityTypeConfiguration<WorkOrderProgressStage>

    {
        public void Configure(EntityTypeBuilder<WorkOrderProgressStage> builder)
        {
            // WorkOrder relationship
            builder.HasOne(p => p.WorkOrder)
                .WithMany(w => w.ProgressStages)
                .HasForeignKey(p => p.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Stage Images relationship
            builder.HasMany(p => p.StageImages)
                .WithOne(i => i.ProgressStage)
                .HasForeignKey(i => i.ProgressStageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Progress Updates relationship
            builder.HasMany(p => p.ProgressUpdates)
                .WithOne(u => u.ProgressStage)
                .HasForeignKey(u => u.ProgressStageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
