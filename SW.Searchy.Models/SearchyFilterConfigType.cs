

using System.Collections.Generic;

namespace SW.Searchy 
{
    public static class SearchyFilterConfigType
    {
        public const string String = "string";
        public const string Date = "date";
        public const string Int = "int";
        public const string Decimal = "decimal";

        public static ICollection<SearchyRule> RulesFor(string dataType)
        {
            switch (dataType)
            {
                case String:
                    {
                        return new SearchyRule[]
                        {
                            SearchyRule.EqualsTo,
                            SearchyRule.NotEqualsTo,
                            SearchyRule.Contains,
                            SearchyRule.StartsWith,
                        };
                    }
                case Int:
                case Decimal:
                    {
                        return new SearchyRule[]
                        {
                            SearchyRule.EqualsTo,
                            SearchyRule.NotEqualsTo,
                            SearchyRule.LessThan,
                            SearchyRule.LessThanOrEquals,
                            SearchyRule.GreaterThan,
                            SearchyRule.GreaterThanOrEquals,

                        };
                    }
                case Date:
                    {
                        return new SearchyRule[]
                        {
                            SearchyRule.LessThan,
                            SearchyRule.LessThanOrEquals,
                            SearchyRule.GreaterThan,
                            SearchyRule.GreaterThanOrEquals,
                        };
                    }
                    //case _ : return Enum.GetValues(typeof(SearchyRule));



            }

            return null;

        }
    };


}


