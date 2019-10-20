

namespace SW.Searchy
{
    public class SearchyFilter : ISearchyFilter
    {
        public string Field { get; set; }

        public object Value { get; set; }
        public SearchyRule Rule { get; set; }

        public SearchyFilter() {}

        public SearchyFilter(string field, SearchyRule rule, object value)
        {
            Field = field;
            Value = value;
            Rule = rule;
        }
    }
}
