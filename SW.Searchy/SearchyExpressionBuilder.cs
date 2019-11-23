﻿using SW.PrimitiveTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SW.Searchy
{
    internal static class SearchyExpressionBuilder
    {
        public static Expression BuildSearchExpression<TEntity>(Expression Parameter, IEnumerable<SearchyCondition> SearchConditions)
        {
            Expression _finalexp2 = null;
            int _counter2 = 0;

            foreach (var _sc in SearchConditions)
            {
                Expression _finalexp = null;
                int _counter = 0;
                foreach (var _fo in _sc.Filters)
                {
                    var _inner = BuildInnerExpression<TEntity>(Parameter, _fo);
                    if (_counter == 0)
                        _finalexp = _inner;
                    else if (_inner != null)
                        _finalexp = Expression.AndAlso(_finalexp, _inner);
                    _counter = _counter + 1;
                }

                if (_counter2 == 0)
                    _finalexp2 = _finalexp;
                else if (_finalexp != null)
                    _finalexp2 = Expression.OrElse(_finalexp2, _finalexp);
                _counter2 = _counter2 + 1;
            }

            return _finalexp2;
        }

        private static Expression BuildInnerExpression<TEntity>(Expression Parameter, ISearchyFilter filter)
        {
            Expression member = null;
            Type memberType = null;

            if (filter.Field.StartsWith("."))
            {
                var _mems = filter.Field.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                member = Parameter;
                memberType = typeof(TEntity);
                foreach (var _s in _mems)
                {
                    member = Expression.Property(member, _s);
                    memberType = memberType.GetProperty(_s, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                }
            }
            else
            {
                member = Expression.Property(Parameter, filter.Field);
                memberType = typeof(TEntity).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
            }

            //Expression _condition = null;
            Expression constant = null;

            var _constcoll = filter.Value as ICollection;
            if (_constcoll is null)
            {
                constant = Expression.Constant(ConvertValueToType(filter.Value, memberType));
                constant = Expression.Convert(constant, memberType);
            }

            switch (filter.Rule)
            {
                case SearchyRule.StartsWith:
                case SearchyRule.Contains:
                    {
                        string _method = "";
                        switch (filter.Rule)
                        {
                            case SearchyRule.StartsWith:
                                {
                                    _method = "StartsWith";
                                    break;
                                }

                            case SearchyRule.Contains:
                                {
                                    _method = "Contains";
                                    break;
                                }
                        }
                        MethodInfo method = typeof(string).GetMethod(_method, new System.Type[] { typeof(string) });
                        string str = System.Convert.ToString(filter.Value);
                        return Expression.Call(member, method, Expression.Constant(filter.Value, filter.Value.GetType()));
                    }

                case SearchyRule.EqualsTo:
                    {
                        return Expression.Equal(member, constant);
                    }

                case SearchyRule.NotEqualsTo:
                    {
                        return Expression.NotEqual(member, constant);
                    }

                case SearchyRule.LessThan:
                    {
                        return Expression.LessThan(member, constant);
                    }

                case SearchyRule.LessThanOrEquals:
                    {
                        return Expression.LessThanOrEqual(member, constant);
                    }

                case SearchyRule.GreaterThan:
                    {
                        return Expression.GreaterThan(member, constant);
                    }

                case SearchyRule.GreaterThanOrEquals:
                    {
                        return Expression.GreaterThanOrEqual(member, constant);
                    }

                case SearchyRule.EqualsToList:
                    {
                        if (_constcoll.Count > 0)
                        {
                            MethodInfo _method = typeof(Enumerable).GetMethods().Where(o => o.Name == "Contains" & o.GetParameters().Count() == 2).First();
                            _method = _method.MakeGenericMethod(memberType);
                            dynamic _ienumerable = null;
                            Type _t1 = typeof(List<>);
                            _ienumerable = Activator.CreateInstance(_t1.MakeGenericType(new[] { memberType }));
                            foreach (var _i in _constcoll)
                                _ienumerable.Add(ConvertValueToType(_i, memberType));
                            filter.Value = _ienumerable;
                            return Expression.Call(_method, new[] { Expression.Constant(filter.Value), member });
                        }
                        else
                            return null;
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        static void BuildOrderBy<U, TEntity>(SearchySort OO, ref IQueryable<TEntity> Query)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "");
            if (OO.Sort == SearchySortOrder.ASC)
                Query = Query.OrderBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.Field), new ParameterExpression[] { pe }));
            else
                Query = Query.OrderByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.Field), new ParameterExpression[] { pe }));
        }

        static void BuildThenBy<U, TEntity>(SearchySort OO, ref IQueryable<TEntity> Query)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "");
            IOrderedQueryable<TEntity> OrderedQuery = (IOrderedQueryable<TEntity>)Query;
            if (OO.Sort == SearchySortOrder.ASC)
                Query = OrderedQuery.ThenBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.Field), new ParameterExpression[] { pe }));
            else
                Query = OrderedQuery.ThenByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.Field), new ParameterExpression[] { pe }));
        }

        public static void BuildOrderByThenBy<TEntity>(SearchySort OO, Type Type, ref IQueryable<TEntity> Query, bool IsMainOrderBy)
        {
            switch (true)
            {
                case object _ when Type.Equals(typeof(int)):
                    {
                        if (IsMainOrderBy)
                            BuildOrderBy<int, TEntity>(OO, ref Query);
                        else
                            BuildThenBy<int, TEntity>(OO, ref Query);
                        break;
                    }

                case object _ when Type.Equals(typeof(string)):
                    {
                        if (IsMainOrderBy)
                            BuildOrderBy<string, TEntity>(OO, ref Query);
                        else
                            BuildThenBy<string, TEntity>(OO, ref Query);
                        break;
                    }

                case object _ when Type.Equals(typeof(DateTime?)):
                    {
                        if (IsMainOrderBy)
                            BuildOrderBy<DateTime?, TEntity>(OO, ref Query);
                        else
                            BuildThenBy<DateTime?, TEntity>(OO, ref Query);
                        break;
                    }

                case object _ when Type.Equals(typeof(DateTime)):
                    {
                        if (IsMainOrderBy)
                            BuildOrderBy<DateTime, TEntity>(OO, ref Query);
                        else
                            BuildThenBy<DateTime, TEntity>(OO, ref Query);
                        break;
                    }

                case object _ when Type.Equals(typeof(byte)):
                    {
                        if (IsMainOrderBy)
                            BuildOrderBy<byte, TEntity>(OO, ref Query);
                        else
                            BuildThenBy<byte, TEntity>(OO, ref Query);
                        break;
                    }

                case object _ when Type.Equals(typeof(short)):
                    {
                        if (IsMainOrderBy)
                            BuildOrderBy<short, TEntity>(OO, ref Query);
                        else
                            BuildThenBy<short, TEntity>(OO, ref Query);
                        break;
                    }

                case object _ when Type.Equals(typeof(decimal)):
                    {
                        if (IsMainOrderBy)
                            BuildOrderBy<decimal, TEntity>(OO, ref Query);
                        else
                            BuildThenBy<decimal, TEntity>(OO, ref Query);
                        break;
                    }

                default:
                    {
                        throw new SWException("Unsupported sort datatype: " + Type.ToString());
                    }
            }
        }


        static dynamic ConvertValueToType(object value, Type type)
        {
            if (value is null) return null;

            var t = Nullable.GetUnderlyingType(type);
            if (t != null)
            {
                type = t;
                if (string.IsNullOrEmpty(value.ToString())) return null;
            }

            return Convert.ChangeType(value, type);

        }

    }
}
