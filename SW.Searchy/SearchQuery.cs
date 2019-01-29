using System.Collections.Generic;

namespace SW.Searchy
{
    public class SearchQuery
    {
        public ICollection<SearchCondition> Conditions { get; set; } = new List<SearchCondition>();



        public SearchQuery()
        {
        }

        public SearchQuery(SearchCondition SearchCondition)
        {
            Conditions.Add(SearchCondition);
        }

        public SearchQuery(FilterByOptions FilterByOptions)
        {
            Conditions.Add(new SearchCondition(FilterByOptions));
        }

        public SearchQuery Exclude(string[] StartingWith)
        {
            var _sq = new SearchQuery();
            foreach (var _c in this.Conditions)
                _sq.Conditions.Add(_c.Exclude(StartingWith));
            return _sq;
        }

        public SearchQuery Keep(string[] StartingWith, bool RemoveStartingWith = true)
        {
            var _sq = new SearchQuery();
            foreach (var _c in this.Conditions)
                _sq.Conditions.Add(_c.Keep(StartingWith, RemoveStartingWith));
            return _sq;
        }


        public SearchQuery Merge(SearchQuery SourceSearchQuery)
        {
            var _result = new SearchQuery();

            foreach (var _cd in this.Conditions)
            {
                if (SourceSearchQuery.Conditions.Count > 0)
                {
                    foreach (var _cs in SourceSearchQuery.Conditions)
                    {
                        var _newcd = new SearchCondition(_cd.Criteria);
                        _result.Conditions.Add(_newcd);
                        foreach (var _fo in _cs.Criteria)
                            _newcd.Criteria.Add(_fo);
                    }
                }
                else
                {
                    var _newcd = new SearchCondition(_cd.Criteria);
                    _result.Conditions.Add(_newcd);
                }
            }
            return _result;
        }
    }
}
