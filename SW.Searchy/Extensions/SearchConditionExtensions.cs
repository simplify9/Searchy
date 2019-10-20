using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public static class SearchConditionExtensions
    {
        public static  SearchyConditon Exclude(this SearchyConditon sc, string[] BeginsWith)
        {
            if (BeginsWith is null)
            {
                throw new ArgumentNullException(nameof(BeginsWith));
            }

            var _sc = new SearchyConditon();
            foreach (var _fo in sc.Criteria)
            {
                foreach (var _s in BeginsWith)
                {
                    if (_fo.Field.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase))
                        break;
                    _sc.Criteria.Add(_fo);
                }
            }
            return _sc;
        }

        public static SearchyConditon Keep(this SearchyConditon sc, string[] BeginsWith, bool RemoveStartingWith = true)
        {
            var _sc = new SearchyConditon();
            foreach (var _fo in sc.Criteria)
            {
                foreach (var _s in BeginsWith)
                {
                    if (_fo.Field.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var _nfo = new SearchyFilter(_fo.Field.Remove(0, _s.Length), _fo.Rule, _fo.Value);
                        _sc.Criteria.Add(_nfo);
                        break;
                    }
                }
            }
            return _sc;
        }
    }
}
