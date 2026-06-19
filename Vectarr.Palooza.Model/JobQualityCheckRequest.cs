using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vectarr.Palooza.CoreClasses.Enums;

namespace Vectarr.Palooza.Model
{
    [Table("Job.QualityCheckRequests")]
    public class JobQualityCheckRequest : BaseModel
    {
        public int JobId { get; set; }
        public virtual Job Job { get; set; }
        public QualityCheckTypeEnum CheckType { get; set; }
        public string? Instructions { get; set; }  // Optional custom instructions
    }
}
