using System.Collections.Generic;
using System;
using System.Linq;

namespace SW.Searchy
{
    public class SearchyConditon
    {
        public ICollection<SearchyFilter> Filters { get; set; }
        public SearchyConditon() => Filters = new List<SearchyFilter>();

        public SearchyConditon(IEnumerable<SearchyFilter> filters)
        {
            Filters = new List<SearchyFilter>(filters);
        }
        
        public SearchyConditon(SearchyFilter filter)
        {
            Filters = new List<SearchyFilter>
            {
                filter
            };
        }



    }
}
