using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.CoreClasses
{
    public class EnumListModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string? Description { get; set; }
    }

    public class EnumExerciseListModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Unit { get; set; }
        public int MultipleOf { get; set; }
        public int Max { get; set; }
    }

    public class EnumImgListModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Img { get; set; }
    }

    public class EnumCountryListModel : EnumListModel
    {
        public string TwoLetterCode { get; set; }
    }
}
