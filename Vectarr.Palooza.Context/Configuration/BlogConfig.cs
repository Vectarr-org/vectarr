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
    public class BlogConfig : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasIndex(b => b.Slug).IsUnique();
            builder.Property(b => b.Slug).IsRequired().HasMaxLength(256);
            builder.Property(b => b.Title).IsRequired().HasMaxLength(256);
            builder.Property(b => b.Excerpt).IsRequired().HasMaxLength(512);
            builder.Property(b => b.Description).IsRequired().HasMaxLength(1024);
            builder.Property(b => b.Category).IsRequired().HasMaxLength(128);

            builder.HasOne(b => b.Image)
                   .WithMany()
                   .HasForeignKey(b => b.ImageId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Sections)
                   .WithOne(s => s.Blog)
                   .HasForeignKey(s => s.BlogId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Keywords)
                   .WithOne(k => k.Blog)
                   .HasForeignKey(k => k.BlogId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
