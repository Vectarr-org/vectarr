using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.BaseApi
{
    public class ListViewModel<T> where T : class, IApiModel, new()
    {
        public ListViewModel(List<T> data, int count)
        {
            Data = data;
            Count = count;
        }

        public List<T> Data { get; }

        public int Count { get; }
    }
}
