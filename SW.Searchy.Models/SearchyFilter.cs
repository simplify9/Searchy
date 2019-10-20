


using System;

namespace SW.Searchy
{
    public class SearchyFilter : ISearchyFilterTyped
    {
        object value;

        public string Field { get; set; }

        public object Value 
        { 
            get 
            {
                if (!(ValueBool is null)) return ValueBool;
                else if (!(ValueByte is null)) return ValueByte;
                else if (!(ValueInt is null)) return ValueInt;
                else if (!(ValueLong is null)) return ValueLong;
                else if (!(ValueDecimal is null)) return ValueDecimal;
                else if (!(ValueDate is null)) return ValueDate;
                else if (!(ValueString is null)) return ValueString;

                else if (!(ValueByteArray is null)) return ValueByteArray;
                else if (!(ValueIntArray is null)) return ValueIntArray;
                else if (!(ValueLongArray is null)) return ValueIntArray;
                else if (!(ValueStringArray is null)) return ValueStringArray;
                
                return value;
            }
            set 
            {
                this.value = value;
            } 
        }
        public SearchyRule Rule { get; set; }
        public bool? ValueBool { get; set; }
        public byte? ValueByte { get; set; }
        public int? ValueInt { get; set; }
        public long? ValueLong { get; set; }
        public decimal? ValueDecimal { get; set; }
        public string ValueString { get; set; }
        public DateTime? ValueDate { get; set; }
        public byte[] ValueByteArray { get; set; }
        public int[] ValueIntArray { get; set; }
        public long[] ValueLongArray { get; set; }
        public string[] ValueStringArray { get; set; }

        public SearchyFilter() {}

        public SearchyFilter(string field, SearchyRule rule, object value)
        {
            Field = field;
            Value = value;
            Rule = rule;
        }

        public SearchyFilter(ISearchyFilter filter) : this(filter.Field, filter.Rule, filter.Value ) {}

        public SearchyFilter(ISearchyFilterTyped filter) : this((ISearchyFilter) filter)
        {
            ValueBool = filter.ValueBool;
            ValueByte = filter.ValueByte;
            ValueInt = filter.ValueInt;
            ValueLong = filter.ValueLong;
            ValueString = filter.ValueString;
            ValueDate = filter.ValueDate;
            ValueByteArray  = filter.ValueByteArray;
            ValueIntArray = filter.ValueIntArray;
            ValueLongArray  = filter.ValueLongArray;
            ValueStringArray = filter.ValueStringArray;
        }
    }
}
