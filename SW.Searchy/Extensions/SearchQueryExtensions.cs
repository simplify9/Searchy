using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Searchy
{
    public static  class SearchQueryExtensions
    {

        public static SearchyQuery Exclude(this SearchyQuery sq, string[] StartingWith)
        {
            var _sq = new SearchyQuery();
            foreach (var _c in sq.Conditions)
                _sq.Conditions.Add(_c.Exclude(StartingWith));
            return _sq;
        }

        public static SearchyQuery Keep(this SearchyQuery sq, string[] StartingWith, bool RemoveStartingWith = true)
        {
            var _sq = new SearchyQuery();
            foreach (var _c in sq.Conditions)
                _sq.Conditions.Add(_c.Keep(StartingWith, RemoveStartingWith));
            return _sq;
        }

        //public static SearchyQuery Merge(this SearchyQuery sq, SearchyQuery SourceSearchQuery)
        //{
        //    var _result = new SearchyQuery();

        //    foreach (var _cd in sq.Conditions)
        //    {
        //        if (SourceSearchQuery.Conditions.Count > 0)
        //        {
        //            foreach (var _cs in SourceSearchQuery.Conditions)
        //            {
        //                var _newcd = new SearchyConditon(_cd.Filters.ToArray());
        //                _result.Conditions.Add(_newcd);
        //                foreach (var _fo in _cs.Filters) _newcd.Filters.Add(_fo);
        //            }
        //        }
        //        else
        //        {
        //            var _newcd = new SearchyConditon(_cd.Filters.ToArray());
        //            _result.Conditions.Add(_newcd);
        //        }
        //    }
        //    return _result;
        //}
    }
}
