using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public interface  ISearchyFilterConfig
    {
        string Type { get; set; }
        string Text { get; set; }
        string Field { get; set; }
    }
}
