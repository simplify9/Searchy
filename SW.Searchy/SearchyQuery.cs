using System.Collections.Generic;

namespace SW.Searchy
{
    public class SearchyQuery
    {
        public ICollection<SearchyConditon> Conditions { get; set; } = new List<SearchyConditon>();



        public SearchyQuery()
        {
        }

        public SearchyQuery(SearchyConditon SearchCondition)
        {
            Conditions.Add(SearchCondition);
        }

        public SearchyQuery(SearchyFilter FilterByOptions)
        {
            Conditions.Add(new SearchyConditon(FilterByOptions));
        }

        public SearchyQuery Exclude(string[] StartingWith)
        {
            var _sq = new SearchyQuery();
            foreach (var _c in this.Conditions)
                _sq.Conditions.Add(_c.Exclude(StartingWith));
            return _sq;
        }

        public SearchyQuery Keep(string[] StartingWith, bool RemoveStartingWith = true)
        {
            var _sq = new SearchyQuery();
            foreach (var _c in this.Conditions)
                _sq.Conditions.Add(_c.Keep(StartingWith, RemoveStartingWith));
            return _sq;
        }


        public SearchyQuery Merge(SearchyQuery SourceSearchQuery)
        {
            var _result = new SearchyQuery();

            foreach (var _cd in this.Conditions)
            {
                if (SourceSearchQuery.Conditions.Count > 0)
                {
                    foreach (var _cs in SourceSearchQuery.Conditions)
                    {
                        var _newcd = new SearchyConditon(_cd.Criteria);
                        _result.Conditions.Add(_newcd);
                        foreach (var _fo in _cs.Criteria)
                            _newcd.Criteria.Add(_fo);
                    }
                }
                else
                {
                    var _newcd = new SearchyConditon(_cd.Criteria);
                    _result.Conditions.Add(_newcd);
                }
            }
            return _result;
        }
    }
}
