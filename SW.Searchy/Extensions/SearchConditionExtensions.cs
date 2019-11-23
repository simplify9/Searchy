using SW.PrimitiveTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SW.Searchy
{
    public static class SearchConditionExtensions
    {
        public static  SearchyCondition Exclude(this SearchyCondition sc, string[] BeginsWith)
        {
            if (BeginsWith is null)
            {
                throw new SWException(nameof(BeginsWith));
            }

            var _sc = new SearchyCondition();
            foreach (var _fo in sc.Filters)
            {
                foreach (var _s in BeginsWith)
                {
                    if (_fo.Field.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase))
                        break;
                    _sc.Filters.Add(_fo);
                }
            }
            return _sc;
        }

        public static SearchyCondition Keep(this SearchyCondition sc, string[] BeginsWith, bool RemoveStartingWith = true)
        {
            var _sc = new SearchyCondition();
            foreach (var _fo in sc.Filters)
            {
                foreach (var _s in BeginsWith)
                {
                    if (_fo.Field.StartsWith(_s, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var _nfo = new SearchyFilter(_fo.Field.Remove(0, _s.Length), _fo.Rule, _fo.Value);
                        _sc.Filters.Add(_nfo);
                        break;
                    }
                }
            }
            return _sc;
        }
    }
}
