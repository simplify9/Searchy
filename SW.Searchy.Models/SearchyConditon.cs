using System.Collections.Generic;
using System;
using System.Linq;

namespace SW.Searchy
{
    public class SearchyConditon
    {
        public ICollection<ISearchyFilter> Filters { get; } = new List<ISearchyFilter>();
        public SearchyConditon() {}

        public SearchyConditon(IEnumerable<ISearchyFilter> filters) : this(filters.ToArray()) {}


        public SearchyConditon(params ISearchyFilter[] filters)
        {
            
            if (filters is null) throw new ArgumentNullException(nameof(filters));

            foreach (var _i in filters) Filters.Add(_i);
        }


    }
}
