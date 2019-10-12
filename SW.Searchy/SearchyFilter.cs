using System.Collections.Generic;
using System;
using System.Xml.Serialization;

namespace SW.Searchy
{
    public partial class SearchyFilter
    {
        public string MemberName { get; set; }

        public object FilterFor { get; set; }
        public SearchyOperator FilterOperator { get; set; }
        public Type MemberType { get; set; }

        public SearchyFilter(string MemberName, SearchyOperator FilterOperator, object FilterFor)
        {
            this.MemberName = MemberName;
            this.FilterFor = FilterFor;
            this.FilterOperator = FilterOperator;
        }




        public SearchyFilter()
        {
        }
    }
}
