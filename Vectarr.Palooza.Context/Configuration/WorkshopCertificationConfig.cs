using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    internal class WorkshopCertificationConfig : IEntityTypeConfiguration<WorkshopCertification>
    {
        public void Configure(EntityTypeBuilder<WorkshopCertification> builder)
        {
            builder.HasOne(c => c.Document)
                .WithMany()
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.ReviewedBy)
                .WithMany()
                .HasForeignKey(c => c.ReviewedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(c => c.RejectionReason)
                .HasMaxLength(2000);
        }
    }
}
