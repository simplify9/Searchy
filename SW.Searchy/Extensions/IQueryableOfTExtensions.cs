using SW.PrimitiveTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SW.Searchy
{
    public static class IQueryableOfTExtensions
    {

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target, string field, object valueEquals)
        {
            return Search(target, new SearchyCondition[] { new SearchyCondition(field, SearchyRule.EqualsTo, valueEquals) });
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target, string field, SearchyRule rule, object value)
        {
            return Search(target, new SearchyCondition[] { new SearchyCondition(field, rule, value) });
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target, SearchyCondition condition)
        {
            return Search(target, new SearchyCondition[] { condition });
        }

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> target,
            IEnumerable<SearchyCondition> conditions,
            IEnumerable<SearchySort> orders = null,
            int pageSize = 0,
            int pageIndex = 0)
        {

            var param = Expression.Parameter(typeof(TEntity), "TEntity");
            Expression finalexp = SearchyExpressionBuilder.BuildSearchExpression<TEntity>(param, conditions);

            if (finalexp != null)
            {
                var finalwhereexp = Expression.Lambda<Func<TEntity, bool>>(finalexp, param);
                target = target.Where(finalwhereexp);
            }

            if (orders != null && orders.Count() > 0)
            {
                var _MainOrderBy = orders.FirstOrDefault();

                var _MainSortMemberType = typeof(TEntity).GetProperty(_MainOrderBy.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;

                SearchyExpressionBuilder.BuildOrderByThenBy(_MainOrderBy, _MainSortMemberType, ref target, true);
                
                var _EOO = new List<SearchySort>
                {
                    _MainOrderBy
                };

                foreach (var _OO in orders.Except(_EOO.AsEnumerable()))
                {
                    var _SortMemberType = typeof(TEntity).GetProperty(_OO.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    SearchyExpressionBuilder.BuildOrderByThenBy(_OO, _SortMemberType, ref target, false);
                }
            }

            if (pageSize > 0 & pageIndex > 0)
                target = target.Skip(pageIndex * pageSize).Take(pageSize);
            else if (pageSize > 0)
                target = target.Take(pageSize);

            return target;
        }

        public static IQueryable<TEntity> Search<TEntity, TRelated>(this IQueryable<TEntity> target,
            string navigationProperty,
            IEnumerable<SearchyCondition> conditions)
        {
            var param = Expression.Parameter(typeof(TEntity), "TEntity");
            ParameterExpression _parammany = Expression.Parameter(typeof(TRelated), "TRelated");
            Expression _finalexp = SearchyExpressionBuilder.BuildSearchExpression<TRelated>(_parammany, conditions);

            if (_finalexp != null)
            {
                var _methodany = typeof(Enumerable).GetMethods().Where(m => m.Name == "Any" & m.GetParameters().Length == 2).Single().MakeGenericMethod(typeof(TRelated));
                var _innerfunction = Expression.Lambda<Func<TRelated, bool>>(_finalexp, _parammany);
                var _finalwhereexp = Expression.Lambda<Func<TEntity, bool>>(Expression.Call(_methodany, Expression.Property(param, typeof(TEntity).GetProperty(navigationProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)), _innerfunction), new ParameterExpression[] { param });
                target = target.Where(_finalwhereexp);
            }

            return target;
        }


    }
}
