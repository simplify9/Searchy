using System.Collections.Generic;
using System.Linq;

namespace SW.Searchy
{
    public class SearchyQuery
    {
        public ICollection<SearchyConditon> Conditions { get; } = new List<SearchyConditon>();

        public SearchyQuery() {}
        public SearchyQuery(SearchyConditon condition) => Conditions.Add(condition);
        public SearchyQuery(SearchyFilter filter) => Conditions.Add(new SearchyConditon(filter));
    }
}
