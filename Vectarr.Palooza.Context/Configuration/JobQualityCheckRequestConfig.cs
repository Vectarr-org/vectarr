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
    public class JobQualityCheckRequestConfig : IEntityTypeConfiguration<JobQualityCheckRequest>
    {
        public void Configure(EntityTypeBuilder<JobQualityCheckRequest> builder)
        {
            builder.HasOne(qc => qc.Job)
                       .WithMany(j => j.QualityChecks)
                       .HasForeignKey(qc => qc.JobId)
                       .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
