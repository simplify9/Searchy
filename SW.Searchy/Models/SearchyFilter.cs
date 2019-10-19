using System.Collections.Generic;
using System;
using System.Xml.Serialization;

namespace SW.Searchy
{
    public partial class SearchyFilter
    {
        public string MemberName { get; set; }

        public object FilterFor { get; set; }
        public SearchyRule Rule { get; set; }
        //public Type MemberType { get; set; }

        public SearchyFilter()
        {
        }

        public SearchyFilter(string MemberName, SearchyRule rule, object FilterFor)
        {
            this.MemberName = MemberName;
            this.FilterFor = FilterFor;
            this.Rule = rule;
        }


    }
}
