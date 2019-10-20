using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public class SearchyFilterConfig : ISearchyFilterConfig
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Field { get; set; }
    }
}
