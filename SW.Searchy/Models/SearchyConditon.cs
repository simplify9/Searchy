using System.Collections.Generic;
using System;
using System.Linq;

namespace SW.Searchy
{
    public class SearchyConditon
    {
        public ICollection<SearchyFilter> Criteria { get; } = new List<SearchyFilter>();

        public SearchyConditon() {}

        public SearchyConditon(IEnumerable<SearchyFilter> filters) : this(filters.ToArray()) {}


        public SearchyConditon(params SearchyFilter[] filters)
        {
            
            if (filters is null) throw new ArgumentNullException(nameof(filters));

            foreach (var _i in filters) Criteria.Add(_i);
        }


    }
}
