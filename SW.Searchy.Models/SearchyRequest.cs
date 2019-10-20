using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public class SearchyRequest
    {
        public IEnumerable<SearchyConditon> Query { get; set; }
        public IEnumerable<SearchyOrder> Order { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public bool CountRows { get; set; }
    }
}
