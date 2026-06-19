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
    internal class BlogKeywordConfig : IEntityTypeConfiguration<BlogKeyword>
    {
        public void Configure(EntityTypeBuilder<BlogKeyword> builder)
        {
            builder.Property(k => k.Keyword).IsRequired().HasMaxLength(128);
            builder.HasIndex(k => new { k.BlogId, k.Keyword }).IsUnique();
        }
    }
} 
