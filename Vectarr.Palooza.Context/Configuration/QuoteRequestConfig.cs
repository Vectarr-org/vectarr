using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vectarr.Palooza.Model;

namespace Vectarr.Palooza.Context.Configuration
{
    public class QuoteRequestConfig : IEntityTypeConfiguration<QuoteRequest>
    {
        public void Configure(EntityTypeBuilder<QuoteRequest> builder)
        {
            builder.Property(q => q.ReferenceNumber).IsRequired().HasMaxLength(32);
            builder.HasIndex(q => q.ReferenceNumber).IsUnique();

            builder.Property(q => q.AccessToken).IsRequired().HasMaxLength(64);
            builder.Property(q => q.ContactName).IsRequired();
            builder.Property(q => q.ContactEmail).IsRequired();
            builder.Property(q => q.Currency).IsRequired().HasMaxLength(8);

            builder.Property(q => q.UnitPrice).HasPrecision(12, 2);
            builder.Property(q => q.TotalPrice).HasPrecision(12, 2);
            builder.Property(q => q.AmountDue).HasPrecision(12, 2);
            builder.Property(q => q.AmountPaid).HasPrecision(12, 2);

            builder.Property(q => q.QuoteSnapshotJson).HasColumnType("text");
            builder.Property(q => q.AdminNotes).HasColumnType("text");

            builder.Property(q => q.StripeCheckoutSessionId).HasMaxLength(255);
            builder.Property(q => q.StripePaymentIntentId).HasMaxLength(255);

            builder.HasOne(q => q.Identity)
                .WithMany()
                .HasForeignKey(q => q.IdentityId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(q => q.Job)
                .WithMany()
                .HasForeignKey(q => q.JobId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(q => q.CadFile)
                .WithMany()
                .HasForeignKey(q => q.CadFileId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasOne(q => q.DrawingFile)
                .WithMany()
                .HasForeignKey(q => q.DrawingFileId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}
