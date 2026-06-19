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
    public class ChatMessageConfig : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasOne(m => m.WorkOrder)
          .WithMany(w => w.ChatMessages)
          .HasForeignKey(m => m.WorkOrderId)
          .OnDelete(DeleteBehavior.Cascade);

            // Creator relationship (optional)
            builder.HasOne(m => m.Creator)
                .WithMany()
                .HasForeignKey(m => m.CreatorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Workshop relationship (optional)
            builder.HasOne(m => m.Workshop)
                .WithMany()
                .HasForeignKey(m => m.WorkshopId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // QuoteRevision relationship (optional)
            builder.HasOne(m => m.QuoteRevision)
                .WithMany()
                .HasForeignKey(m => m.QuoteRevisionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // ProgressStage relationship (optional)
            builder.HasOne(m => m.ProgressStage)
                .WithMany()
                .HasForeignKey(m => m.ProgressStageId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
