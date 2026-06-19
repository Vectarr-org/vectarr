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
    internal class BlogSectionConfig : IEntityTypeConfiguration<BlogSection>
    {
        public void Configure(EntityTypeBuilder<BlogSection> builder)
        {
            builder.Property(s => s.Heading).HasMaxLength(256);
            builder.Property(s => s.Content).IsRequired();
            builder.HasIndex(s => new { s.BlogId, s.Order });
        }
    }
}