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
    public class QuoteRevisionConfig : IEntityTypeConfiguration<QuoteRevision>
    {
        public void Configure(EntityTypeBuilder<QuoteRevision> builder)
        {
            // WorkOrder relationship
            builder.HasOne(q => q.WorkOrder)
                .WithMany(w => w.QuoteRevisions)
                .HasForeignKey(q => q.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Creator who responded relationship
            builder.HasOne(q => q.RespondedByCreator)
                .WithMany()
                .HasForeignKey(q => q.RespondedByCreatorId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}
