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
        public static Expression BuildSearchExpression<TEntity>(Expression Parameter, IEnumerable<SearchyConditon> SearchConditions)
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

        static Expression BuildInnerExpression<TEntity>(Expression Parameter, SearchyFilter filter)
        {
            Expression _member = null;
            Type _membertype = null;

            if (filter.Field.StartsWith("."))
            {
                var _mems = filter.Field.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
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
                _member = Expression.Property(Parameter, filter.Field);
                _membertype = typeof(TEntity).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
            }

            //Expression _condition = null;
            Expression constant = null;

            var _constcoll = filter.Value as ICollection;
            if (_constcoll is null)
            {
                constant = Expression.Constant(ConvertObjectToType(filter.Value, _membertype));
                constant = Expression.Convert(constant, _membertype);
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
                        return Expression.Call(_member, method, Expression.Constant(filter.Value, filter.Value.GetType()));
                    }

                case SearchyRule.EqualsTo:
                    {
                        return Expression.Equal(_member, constant);
                    }

                case SearchyRule.NotEqualsTo:
                    {
                        return Expression.NotEqual(_member, constant);
                    }

                case SearchyRule.LessThan:
                    {
                        return Expression.LessThan(_member, constant);
                    }

                case SearchyRule.LessThanOrEquals:
                    {
                        return Expression.LessThanOrEqual(_member, constant);
                    }

                case SearchyRule.GreaterThan:
                    {
                        return Expression.GreaterThan(_member, constant);
                    }

                case SearchyRule.GreaterThanOrEquals:
                    {
                        return Expression.GreaterThanOrEqual(_member, constant);
                    }

                case SearchyRule.EqualsToList:
                    {
                        if (_constcoll.Count > 0)
                        {
                            MethodInfo _method = typeof(Enumerable).GetMethods().Where(o => o.Name == "Contains" & o.GetParameters().Count() == 2).First();
                            _method = _method.MakeGenericMethod(_membertype);
                            dynamic _ienumerable = null;
                            Type _t1 = typeof(List<>);
                            _ienumerable = Activator.CreateInstance(_t1.MakeGenericType(new[] { _membertype }));
                            foreach (var _i in _constcoll)
                                _ienumerable.Add(ConvertObjectToType(_i, _membertype));
                            filter.Value = _ienumerable;
                            return Expression.Call(_method, new[] { Expression.Constant(filter.Value), _member });
                        }
                        else
                            return null;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        static void BuildOrderBy<U, TEntity>(SearchyOrder OO, ref IQueryable<TEntity> Query)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "");
            if (OO.SortOrder == SearchyOrder.Order.ASC)
                Query = Query.OrderBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
            else
                Query = Query.OrderByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
        }

        static void BuildThenBy<U, TEntity>(SearchyOrder OO, ref IQueryable<TEntity> Query)
        {
            ParameterExpression pe = Expression.Parameter(typeof(TEntity), "");
            IOrderedQueryable<TEntity> OrderedQuery = (IOrderedQueryable<TEntity>)Query;
            if (OO.SortOrder == SearchyOrder.Order.ASC)
                Query = OrderedQuery.ThenBy(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
            else
                Query = OrderedQuery.ThenByDescending(Expression.Lambda<Func<TEntity, U>>(Expression.Property(pe, OO.MemberName), new ParameterExpression[] { pe }));
        }

        public static void BuildOrderByThenBy<TEntity>(SearchyOrder OO, Type Type, ref IQueryable<TEntity> Query, bool IsMainOrderBy)
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
                        //break;
                    }
            }
        }

        static dynamic ConvertObjectToType(object TargetObject, System.Type TargetType)
        {
            if (TargetObject is null)
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
                    }
            }
            return null;
        }

    }
}
