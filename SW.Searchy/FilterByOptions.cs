using System.Collections.Generic;
using System;
using System.Xml.Serialization;

namespace SW.Searchy
{
    public class FilterByOptions
    {
        private string _MemberName;

        public string MemberName
        {
            get
            {
                return _MemberName;
            }
            set
            {
                _MemberName = value;
            }
        }

        private object _FilterFor;

        [XmlElement("xmlInteger", Type = typeof(int))]
        [XmlElement("xmlString", Type = typeof(string))]
        [XmlElement("xmlGuid", Type = typeof(Guid))]
        [XmlElement("xmlBoolean", Type = typeof(bool))]
        [XmlElement("xmlIntegerList", Type = typeof(List<int>))]
        [XmlElement("xmlDecimalList", Type = typeof(List<decimal>))]
        [XmlElement("xmlStringList", Type = typeof(List<string>))]
        [XmlElement("xmlGuidList", Type = typeof(List<Guid>))]
        [XmlElement("xmlEmptyGuidList", Type = typeof(List<Guid?>))]
        public object FilterFor
        {
            get
            {
                return _FilterFor;
            }
            set
            {
                _FilterFor = value;
            }
        }


        private FilterOperatorOptions _FilterOperator;

        public FilterOperatorOptions FilterOperator
        {
            get
            {
                return _FilterOperator;
            }
            set
            {
                _FilterOperator = value;
            }
        }

        public FilterByOptions(string MemberName, FilterOperatorOptions FilterOperator, object FilterFor)
        {
            _MemberName = MemberName;
            _FilterFor = FilterFor;
            _FilterOperator = FilterOperator;
        }

        private System.Type _MemberType;

        public Type MemberType
        {
            get
            {
                return _MemberType;
            }
            set
            {
                _MemberType = value;
            }
        }

        public enum FilterOperatorOptions
        {
            EqualsTo = 1,
            NotEqualsTo = 2,
            BeginsWith = 3,
            Contains = 4,
            GreaterThan = 5,
            GreaterThanOrEquals = 6,
            LessThan = 7,
            LessThanOrEquals = 8,
            EqualsToList = 9
        }


        public FilterByOptions()
        {
        }
    }
}
