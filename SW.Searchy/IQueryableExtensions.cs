using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SW.Searchy
{
    public static class IQueryableExtensions

    {
        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> Target, string FindMember, FilterByOptions.FilterOperatorOptions SearchOperator, object WithValue)
        {
            return Search(Target, new SearchQuery(new FilterByOptions(FindMember, SearchOperator, WithValue)));
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> Target, string FindMember, object WithValueEquals)
        {
            return Search(Target, new SearchQuery(new FilterByOptions(FindMember, FilterByOptions.FilterOperatorOptions.EqualsTo, WithValueEquals)));
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> Target, SearchCondition SearchCondition)
        {
            return Search(Target, new SearchQuery(SearchCondition));
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> Target, SearchQuery SearchQuery, IEnumerable<OrderByOptions> OrderByList = null, int PageSize = 0, int PageIndex = 0)
        {
            var _param = Expression.Parameter(typeof(TEntity), "TEntity");
            Expression _finalexp = SearchyExpressionBuilder.BuildSearchExpression<TEntity>(_param, SearchQuery.Conditions);

            if (_finalexp != null)
            {
                var _finalwhereexp = Expression.Lambda<Func<TEntity, bool>>(_finalexp, _param);
                Target = Target.Where(_finalwhereexp);
            }

            if (OrderByList != null && OrderByList.Count() > 0)
            {
                var _MainOrderBy = OrderByList.FirstOrDefault();
                Type _MainSortMemberType = typeof(TEntity).GetProperty(_MainOrderBy.MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                SearchyExpressionBuilder.BuildOrderByThenBy(_MainOrderBy, _MainSortMemberType, ref Target, true);
                List<OrderByOptions> _EOO = new List<OrderByOptions>();
                _EOO.Add(_MainOrderBy);
                foreach (var _OO in OrderByList.Except(_EOO.AsEnumerable()))
                {
                    Type _SortMemberType = typeof(TEntity).GetProperty(_OO.MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    SearchyExpressionBuilder.BuildOrderByThenBy(_OO, _SortMemberType, ref Target, false);
                }
            }

            if (PageSize > 0 & PageIndex > 0)
                Target = Target.Skip(PageIndex * PageSize).Take(PageSize);
            else if (PageSize > 0)
                Target = Target.Take(PageSize);

            return Target;
        }

        //public static IQueryable<TEntity> SearchMany<TEntity, TEntityMany>(this IQueryable<TEntity> Target, string NavigationProperty, SearchQuery SearchQuery)
        //{
        //    var _param = Expression.Parameter(typeof(TEntity), "TEntity");
        //    ParameterExpression _parammany = Expression.Parameter(typeof(TEntityMany), "TEntityMany");
        //    Expression _finalexp = SearchyExpressionBuilder.BuildSearchExpression<TEntityMany>(_parammany, SearchQuery.Conditions);

        //    if (_finalexp != null)
        //    {
        //        var _methodany = typeof(Enumerable).GetMethods().Where(m => m.Name == "Any" & m.GetParameters().Length == 2).Single().MakeGenericMethod(typeof(TEntityMany));
        //        var _innerfunction = Expression.Lambda<Func<TEntityMany, bool>>(_finalexp, _parammany);
        //        var _finalwhereexp = Expression.Lambda<Func<TEntity, bool>>(Expression.Call(_methodany, Expression.Property(_param, typeof(TEntity).GetProperty(NavigationProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)), _innerfunction), new ParameterExpression[] { _param });
        //        Target = Target.Where(_finalwhereexp);
        //    }

        //    return Target;
        //}


    }
}
