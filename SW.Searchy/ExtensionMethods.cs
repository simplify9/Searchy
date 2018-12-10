using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SW.Searchy
{
    public static class ExtensionMethods

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

        //public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> Target, SearchOptions SearchOptions, int PageSize = 0, int PageIndex = 0)
        //{
        //    SearchQuery _seconds = new SearchQuery();
        //    if (SearchOptions.SearchMatchType == SearchMatchType.MatchAll)
        //    {
        //        var _sc = new SearchCondition();
        //        _seconds.Conditions.Add(new SearchCondition() { Criteria = new List<FilterByOptions>(SearchOptions.FilterOptions) });
        //    }
        //    else
        //        foreach (var _f in SearchOptions.FilterOptions)
        //        {
        //            var _lst = new List<FilterByOptions>();
        //            _lst.Add(_f);
        //            _seconds.Conditions.Add(new SearchCondition() { Criteria = _lst });
        //        }
        //    return Search(Target, _seconds, SearchOptions.OrderOptions, PageSize, PageIndex);
        //}

        public static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> Target, SearchQuery SearchQuery, IEnumerable<OrderByOptions> OrderByList = null, int PageSize = 0, int PageIndex = 0)
        {
            var _param = Expression.Parameter(typeof(TEntity), "TEntity");
            Expression _finalexp = BuildSearchExpression<TEntity>(_param, SearchQuery.Conditions);

            if (_finalexp != null)
            {
                var _finalwhereexp = Expression.Lambda<Func<TEntity, bool>>(_finalexp, _param);
                Target = Target.Where(_finalwhereexp);
            }

            if (OrderByList != null && OrderByList.Count() > 0)
            {
                var _MainOrderBy = OrderByList.FirstOrDefault();
                Type _MainSortMemberType = typeof(TEntity).GetProperty(_MainOrderBy.MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                BuildOrderByThenBy(_MainOrderBy, _MainSortMemberType, ref Target, true);
                List<OrderByOptions> _EOO = new List<OrderByOptions>();
                _EOO.Add(_MainOrderBy);
                foreach (var _OO in OrderByList.Except(_EOO.AsEnumerable()))
                {
                    Type _SortMemberType = typeof(TEntity).GetProperty(_OO.MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                    BuildOrderByThenBy(_OO, _SortMemberType, ref Target, false);
                }
            }

            if (PageSize > 0 & PageIndex > 0)
                Target = Target.Skip(PageIndex * PageSize).Take(PageSize);
            else if (PageSize > 0)
                Target = Target.Take(PageSize);

            return Target;
        }

        public static IQueryable<TEntity> SearchMany<TEntity, TEntityMany>(this IQueryable<TEntity> Target, string NavigationProperty, SearchQuery SearchQuery)
        {
            var _param = Expression.Parameter(typeof(TEntity), "TEntity");
            ParameterExpression _parammany = Expression.Parameter(typeof(TEntityMany), "TEntityMany");
            Expression _finalexp = BuildSearchExpression<TEntityMany>(_parammany, SearchQuery.Conditions);

            if (_finalexp != null)
            {
                var _methodany = typeof(Enumerable).GetMethods().Where(m => m.Name == "Any" & m.GetParameters().Length == 2).Single().MakeGenericMethod(typeof(TEntityMany));
                var _innerfunction = Expression.Lambda<Func<TEntityMany, bool>>(_finalexp, _parammany);
                var _finalwhereexp = Expression.Lambda<Func<TEntity, bool>>(Expression.Call(_methodany, Expression.Property(_param, typeof(TEntity).GetProperty(NavigationProperty, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)), _innerfunction), new ParameterExpression[] { _param });
                Target = Target.Where(_finalwhereexp);
            }

            return Target;
        }

        private static Expression BuildSearchExpression<TEntity>(Expression Parameter, IEnumerable<SearchCondition> SearchConditions)
        {
            Expression _finalexp2 = null;
            int _counter2 = 0;

            foreach (var _sc in SearchConditions)
            {
                Expression _finalexp = null;
                int _counter = 0;
                foreach (var _fo in _sc.Criteria)
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

        private static Expression BuildInnerExpression<TEntity>(Expression Parameter, FilterByOptions FilterByOptions)
        {
            Expression _member = null;
            Type _membertype = null;

            if (FilterByOptions.MemberName.StartsWith("."))
            {
                var _mems = FilterByOptions.MemberName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                _member = Parameter;
                _membertype = typeof(TEntity);
                foreach (var _s in _mems)
                {
                    _member = Expression.Property(_member, _s);
                    _membertype = _membertype.GetProperty(_s, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
                }
            }
            else
            {
                _member = Expression.Property(Parameter, FilterByOptions.MemberName);
                _membertype = typeof(TEntity).GetProperty(FilterByOptions.MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
            }

            Expression _condition = null;
            Expression _constant = null;

            var _constcoll = FilterByOptions.FilterFor as ICollection;
            if (_constcoll == null)
            {
                _constant = Expression.Constant(ConvertObjectToType(FilterByOptions.FilterFor, _membertype));
                _constant = Expression.Convert(_constant, _membertype);
            }

            switch (FilterByOptions.FilterOperator)
            {
                case FilterByOptions.FilterOperatorOptions.BeginsWith:
                case FilterByOptions.FilterOperatorOptions.Contains:
                    {
                        string _method = "";
                        switch (FilterByOptions.FilterOperator)
                        {
                            case FilterByOptions.FilterOperatorOptions.BeginsWith:
                                {
                                    _method = "StartsWith";
                                    break;
                                }

                            case FilterByOptions.FilterOperatorOptions.Contains:
                                {
                                    _method = "Contains";
                                    break;
                                }
                        }
                        MethodInfo method = typeof(string).GetMethod(_method, new System.Type[] { typeof(string) });
                        string str = System.Convert.ToString(FilterByOptions.FilterFor);
                        return Expression.Call(_member, method, Expression.Constant(FilterByOptions.FilterFor, FilterByOptions.FilterFor.GetType()));
                    }

                case FilterByOptions.FilterOperatorOptions.EqualsTo:
                    {
                        return Expression.Equal(_member, _constant);
                    }

                case FilterByOptions.FilterOperatorOptions.NotEqualsTo:
                    {
                        return Expression.NotEqual(_member, _constant);
                    }

                case FilterByOptions.FilterOperatorOptions.LessThan:
                    {
                        return Expression.LessThan(_member, _constant);
                    }

                case FilterByOptions.FilterOperatorOptions.LessThanOrEquals:
                    {
                        return Expression.LessThanOrEqual(_member, _constant);
                    }

                case FilterByOptions.FilterOperatorOptions.GreaterThan:
                    {
                        return Expression.GreaterThan(_member, _constant);
                    }

                case FilterByOptions.FilterOperatorOptions.GreaterThanOrEquals:
                    {
                        return Expression.GreaterThanOrEqual(_member, _constant);
                    }

                case FilterByOptions.FilterOperatorOptions.EqualsToList:
                    {
                        if (_constcoll.Count > 0)
                        {
                            MethodInfo _method = typeof(System.Linq.Enumerable).GetMethods().Where(o => o.Name == "Contains" & o.GetParameters().Count() == 2).First();
                            _method = _method.MakeGenericMethod(_membertype);
                            dynamic _ienumerable = null;
                            Type _t1 = typeof(List<>);
                            _ienumerable = Activator.CreateInstance(_t1.MakeGenericType(new[] { _membertype }));
                            foreach (var _i in _constcoll)
                                _ienumerable.add(_i);
                            FilterByOptions.FilterFor = _ienumerable;
                            return Expression.Call(_method, new[] { Expression.Constant(FilterByOptions.FilterFor), _member });
                        }
                        else
                            return null;
                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                        break;
                    }
            }
        }

        private static void BuildOrderBy<U, TEntity>(OrderByOptions OO, ref IQueryable<TEntity> Query)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "");
            if (OO.SortOrder == OrderByOptions.Order.ASC)
                Query = Query.OrderBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
            else
                Query = Query.OrderByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
        }

        private static void BuildThenBy<U, TEntity>(OrderByOptions OO, ref IQueryable<TEntity> Query)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "");
            IOrderedQueryable<TEntity> OrderedQuery = (IOrderedQueryable<TEntity>)Query;
            if (OO.SortOrder == OrderByOptions.Order.ASC)
                Query = OrderedQuery.ThenBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
            else
                Query = OrderedQuery.ThenByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
        }

        private static void BuildOrderByThenBy<TEntity>(OrderByOptions OO, System.Type Type, ref IQueryable<TEntity> Query, bool IsMainOrderBy)
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
                        throw new Exception("Unsupported sort datatype: " + Type.ToString());
                        break;
                    }
            }
        }

        public static object ConvertObjectToType(object TargetObject, System.Type TargetType)
        {
            if (TargetObject == null)
                return null;

            var _ObjType = TargetObject.GetType();

            switch (true)
            {
                case object _ when _ObjType == TargetType:
                    {
                        return TargetObject;
                    }

                case object _ when TargetType == typeof(bool):
                case object _ when TargetType == typeof(bool?):
                    {
                        return System.Convert.ToBoolean(TargetObject);
                    }

                case object _ when TargetType == typeof(long):
                case object _ when TargetType == typeof(long?):
                    {
                        return System.Convert.ToInt64(TargetObject);
                    }

                case object _ when TargetType == typeof(int):
                case object _ when TargetType == typeof(int?):
                    {
                        return System.Convert.ToInt32(TargetObject);
                    }

                case object _ when TargetType == typeof(double):
                case object _ when TargetType == typeof(double?):
                    {
                        return System.Convert.ToDouble(TargetObject);
                    }

                case object _ when TargetType == typeof(short):
                case object _ when TargetType == typeof(short?):
                    {
                        return System.Convert.ToInt16(TargetObject);
                    }

                case object _ when TargetType == typeof(byte):
                case object _ when TargetType == typeof(byte?):
                    {
                        return System.Convert.ToByte(TargetObject);
                    }

                case object _ when TargetType == typeof(DateTime):
                case object _ when TargetType == typeof(DateTime?):
                    {
                        return System.Convert.ToDateTime(TargetObject);
                    }

                case object _ when TargetType == typeof(DateTime):
                case object _ when TargetType == typeof(DateTime?):
                    {
                        return System.Convert.ToDateTime(TargetObject);
                    }

                case object _ when TargetType == typeof(decimal):
                case object _ when TargetType == typeof(decimal?):
                    {
                        return System.Convert.ToDecimal(TargetObject);
                    }

                case object _ when TargetType == typeof(Guid):
                case object _ when TargetType == typeof(Guid?):
                    {
                        if (TargetObject.GetType() == typeof(string))
                            return new Guid(System.Convert.ToString(TargetObject));
                        else if (TargetObject.GetType() == typeof(Guid))
                            return (Guid)TargetObject;
                        break;
                    }

                default:
                    {
                        throw new Exception("Unsupported type conversion.");
                        break;
                    }
            }
            return null;
        }

    }
}
