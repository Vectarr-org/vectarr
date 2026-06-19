using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    public class ImageConfig : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            // Existing relationships
            builder.HasOne(i => i.Workshop)
                .WithMany(w => w.Images)
                .HasForeignKey(i => i.WorkshopId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(i => i.Job)
                .WithMany(j => j.Files)
                .HasForeignKey(i => i.JobId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // NEW: WorkOrder relationships
            builder.HasOne(i => i.WorkOrder)
                .WithMany(w => w.AdditionalFiles)
                .HasForeignKey(i => i.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(i => i.ProgressStage)
                .WithMany(p => p.StageImages)
                .HasForeignKey(i => i.ProgressStageId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(i => i.ProgressUpdate)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProgressUpdateId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.HasOne(i => i.WorkOrderQualityCheckRequest)
          .WithMany(p => p.Attachments)
          .HasForeignKey(i => i.WorkOrderQualityCheckRequestId)
          .OnDelete(DeleteBehavior.Cascade)
          .IsRequired(false);
        }
    }
}
