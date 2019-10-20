﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public class SearchyResponse<TModel>
    {
        public IEnumerable<TModel> Result { get; set; }
        public int TotalCount { get; set; }
    }
}