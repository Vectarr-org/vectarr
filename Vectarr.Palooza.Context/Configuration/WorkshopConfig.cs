using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vectarr.Palooza.Context.Configuration
{
    internal class WorkshopConfig : IEntityTypeConfiguration<Workshop>
    {
        public void Configure(EntityTypeBuilder<Workshop> builder)
        {
            // One-to-One with UserAccount
            builder.HasOne(w => w.UserAccount)
                .WithOne(u => u.Workshop)
                .HasForeignKey<Workshop>(w => w.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Capabilities
            builder.HasMany(w => w.WorkshopCapabilities)
                .WithOne(wc => wc.Workshop)
                .HasForeignKey(wc => wc.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Specializations
            builder.HasMany(w => w.WorkshopSpecializations)
                .WithOne(ws => ws.Workshop)
                .HasForeignKey(ws => ws.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Equipment
            builder.HasMany(w => w.WorkshopEquipment)
                .WithOne(we => we.Workshop)
                .HasForeignKey(we => we.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many with Certifications
            builder.HasMany(w => w.WorkshopCertifications)
                .WithOne(wc => wc.Workshop)
                .HasForeignKey(wc => wc.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
            .HasOne(w => w.Logo)
     .WithMany()  // Image doesn't navigate back to Workshop as Logo
     .HasForeignKey(w => w.LogoId)
     .OnDelete(DeleteBehavior.SetNull);  // When image is deleted, set LogoId to null

            builder
                       .HasMany(w => w.Images)
                .WithOne(i => i.Workshop)
                .HasForeignKey(i => i.WorkshopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.Bids)
           .WithOne(b => b.Workshop)
           .HasForeignKey(b => b.WorkshopId)
           .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(w => w.WorkOrders)
       .WithOne(wo => wo.Workshop)
       .HasForeignKey(wo => wo.WorkshopId)
       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
