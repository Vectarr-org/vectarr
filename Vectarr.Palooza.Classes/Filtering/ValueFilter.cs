using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.Filtering
{
    public class ValueFilter : Filter
    {
        public ValueFilter(FilterVariables filterVariable, string value, string valueNames = null)
            : base(filterVariable)
        {
            Value = value;

            ValueNames = !string.IsNullOrEmpty(valueNames) ? valueNames : null;

            if (string.IsNullOrEmpty(value))
                Collection = new List<string>();
            else
                Collection = value.Split(",").ToList();
        }
    }
}
