﻿using System.Collections.Generic;
using System;
using System.Linq;

namespace SW.Searchy
{
    public class SearchyConditon
    {
        public ICollection<SearchyFilter> Filters { get; set; }

        public SearchyConditon() => Filters = new List<SearchyFilter>();

        public SearchyConditon(SearchyFilter filter)
        {
            Filters = new List<SearchyFilter> { filter };
        }

        public SearchyConditon(ICollection<SearchyFilter> filters)
        {
            Filters = filters;
        }

        public SearchyConditon(ICollection<ISearchyFilterTyped> filters)
        {
            Filters = filters.Select(f => new SearchyFilter(f)).ToList();
        }

        public SearchyConditon(ICollection<ISearchyFilter> filters)
        {
            Filters = filters.Select(f => new SearchyFilter(f)).ToList();
        }









    }
}
