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

        /// <summary>
        /// This will check if the user has data of type T regardless of the search conditions
        /// </summary>
        public bool HasData { get; set; }

        public SearchInfo SearchInfo { get; set; }
    }

    public class SearchInfo
    {
        public List<KeyValuePair<string, string>> FilterQueryString { get; set; }

        public string OrderBy { get; set; }

        public bool OrderByDescending { get; set; }

        public int Page { get; set; }

        public int CountPerPage { get; set; }

        public List<int> ReturnedIds { get; set; }

        public int TotalPages { get; set; }
    }
}
