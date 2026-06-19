using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.Classes.BaseApi
{
    public class ItemViewModel<T> where T : class, IApiModel, new()
    {
        public ItemViewModel(T item)
        {
            Item = item;
        }
        public T Item { get; private set; }
    }
}
