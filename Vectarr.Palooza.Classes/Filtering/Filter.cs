using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.Filtering
{
    public abstract class Filter
    {
        protected Filter(FilterVariables filterVariable)
        {
            FilterVariable = filterVariable;
        }

        public FilterVariables FilterVariable { get; private set; }

        public string Value { get; protected set; }

        public string ValueNames { get; protected set; }

        public List<string> Collection { get; protected set; }

        public int? FormFieldId { get; set; }

        public KeyValuePair<string, string> SourceKeyValuePair { get; set; }
    }
}
