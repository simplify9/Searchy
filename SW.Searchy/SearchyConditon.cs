using System.Collections.Generic;
using System;

namespace SW.Searchy
{
    public class SearchyConditon
    {
        public ICollection<SearchyFilter> Criteria { get; set; } = new List<SearchyFilter>();

        public SearchyConditon()
        {
        }

        public SearchyConditon(SearchyFilter FilterByOptions)
        {
            this.Criteria.Add(FilterByOptions);
        }

        public SearchyConditon(IEnumerable<SearchyFilter> FilterByOptions)
        {
            foreach (var _i in FilterByOptions)
                this.Criteria.Add(_i);
        }

        public SearchyConditon Exclude(string[] StartingWith)
        {
            var _sc = new SearchyConditon();
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

        public SearchyConditon Keep(string[] StartingWith, bool RemoveStartingWith = true)
        {
            var _sc = new SearchyConditon();
            foreach (var _fo in this.Criteria)
            {
                foreach (var _s in StartingWith)
                {
                    if (_fo.MemberName.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var _nfo = new SearchyFilter(_fo.MemberName.Remove(0, _s.Length), _fo.FilterOperator, _fo.FilterFor);
                        _sc.Criteria.Add(_nfo);
                        break;
                    }
                }
            }
            return _sc;
        }
    }
}
