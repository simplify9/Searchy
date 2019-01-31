using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SW.Searchy
{
    public class SearchQuery<TEntity> : SearchQuery
    {

        public ICollection< OrderByOptions> Order { get; set; } = new List<OrderByOptions>();

        public Paging Paging { get; set; }

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

        public IQueryable<TEntity> Apply(IQueryable<TEntity> queryable)
        {
            var param = Expression.Parameter(typeof(TEntity), "TEntity");
            Expression exp = SearchyExpressionBuilder.BuildSearchExpression<TEntity>(param, Conditions);

            if (exp != null)
            {
                var where = Expression.Lambda<Func<TEntity, bool>>(exp, param);
                queryable = queryable.Where(where);
            }

            if (Order != null && Order.Count() > 0)
            {
                var _MainOrderBy = Order.FirstOrDefault();
                Type _MainSortMemberType = typeof(TEntity).GetProperty(_MainOrderBy.MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                SearchyExpressionBuilder.BuildOrderByThenBy(_MainOrderBy, _MainSortMemberType, ref queryable, true);
                List<OrderByOptions> _EOO = new List<OrderByOptions>();
                _EOO.Add(_MainOrderBy);
                foreach (var _OO in Order.Except(_EOO.AsEnumerable()))
                {
                    Type _SortMemberType = typeof(TEntity).GetProperty(_OO.MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    SearchyExpressionBuilder.BuildOrderByThenBy(_OO, _SortMemberType, ref queryable, false);
                }
            }

            if (Paging == null) return queryable;

            if (Paging.PageSize > 0 & Paging.PageIndex > 0)
                queryable = queryable.Skip(Paging.PageIndex * Paging.PageSize).Take(Paging.PageSize);
            else if (Paging.PageSize > 0)
                queryable = queryable.Take(Paging.PageSize);

            return queryable;

        }


    }
}
