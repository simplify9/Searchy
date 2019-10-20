﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public class SearchyRequest
    {
        public IEnumerable<SearchyConditon> Conditions { get; set; }
        public IEnumerable<SearchyOrder> Orders { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public bool CountRows { get; set; }
    }
}
