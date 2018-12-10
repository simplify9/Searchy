using System.Collections.Generic;
using System;

namespace SW.Searchy
{
    public class SearchCondition
    {
        public ICollection<FilterByOptions> Criteria { get; set; } = new List<FilterByOptions>();

        public SearchCondition()
        {
        }

        public SearchCondition(FilterByOptions FilterByOptions)
        {
            this.Criteria.Add(FilterByOptions);
        }

        public SearchCondition(IEnumerable<FilterByOptions> FilterByOptions)
        {
            foreach (var _i in FilterByOptions)
                this.Criteria.Add(_i);
        }

        public SearchCondition Exclude(string[] StartingWith)
        {
            var _sc = new SearchCondition();
            foreach (var _fo in this.Criteria)
            {
                foreach (var _s in StartingWith)
                {
                    if (_fo.MemberName.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase))
                        break;
                    _sc.Criteria.Add(_fo);
                }
            }
            return _sc;
        }

        public SearchCondition Keep(string[] StartingWith, bool RemoveStartingWith = true)
        {
            var _sc = new SearchCondition();
            foreach (var _fo in this.Criteria)
            {
                foreach (var _s in StartingWith)
                {
                    if (_fo.MemberName.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var _nfo = new FilterByOptions(_fo.MemberName.Remove(0, _s.Length), _fo.FilterOperator, _fo.FilterFor);
                        _sc.Criteria.Add(_nfo);
                        break;
                    }
                }
            }
            return _sc;
        }
    }
}
