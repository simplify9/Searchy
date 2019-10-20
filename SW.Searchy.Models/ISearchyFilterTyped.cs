using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public interface ISearchyFilterTyped : ISearchyFilter
    {
        bool? ValueBool { get; set; }
        byte? ValueByte { get; set; }
        int? ValueInt { get; set; }
        long? ValueLong { get; set; }
        decimal? ValueDecimal { get; set; }
        string ValueString { get; set; }
        DateTime? ValueDate { get; set; }
        byte[] ValueByteArray { get; set; }
        int[] ValueIntArray { get; set; }
        long[] ValueLongArray { get; set; }
        string[] ValueStringArray { get; set; }
    }
}
