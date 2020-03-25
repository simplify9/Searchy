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
    internal static class ExpressionExtensions
    {
        public static Expression BuildSearchExpression<TEntity>(this Expression parameter, IEnumerable<SearchyCondition> searchyConditions)
        {
            Expression finalExpression = null;

            foreach (var searchyCondition in searchyConditions)
            {
                Expression conditionExpression = null;

                foreach (var searchyFilter in searchyCondition.Filters)
                {
                    var filterExpression = parameter.BuildFilterExpression<TEntity>(searchyFilter);

                    if (conditionExpression == null)
                        conditionExpression = filterExpression;

                    else if (filterExpression != null)
                        conditionExpression = Expression.AndAlso(conditionExpression, filterExpression);
                }

                if (finalExpression == null)
                    finalExpression = conditionExpression;
                else if (conditionExpression != null)
                    finalExpression = Expression.OrElse(finalExpression, conditionExpression);
            }

            return finalExpression;
        }

        static Expression BuildFilterExpression<TEntity>(this Expression parameter, ISearchyFilter filter)
        {
            //Expression fieldNameExpression = null;
            //Type fieldType = null;

            //if (filter.Field.Contains("."))
            //{
            var fieldSegments = filter.Field.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            var fieldNameExpression = parameter;
            var fieldType = typeof(TEntity);

            foreach (var fieldSegment in fieldSegments)
            {
                fieldNameExpression = Expression.Property(fieldNameExpression, fieldSegment);
                fieldType = fieldType.GetProperty(fieldSegment, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
            }
            //}
            //else
            //{
            //    fieldNameExpression = Expression.Property(parameter, filter.Field);
            //    fieldType = typeof(TEntity).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).PropertyType;
            //}

            switch (filter.Rule)
            {
                case SearchyRule.StartsWith:
                    return Expression.Call(fieldNameExpression, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.Contains:
                    return Expression.Call(fieldNameExpression, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.EqualsTo:
                    return Expression.Equal(fieldNameExpression, GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.NotEqualsTo:
                    return Expression.NotEqual(fieldNameExpression, GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.LessThan:
                    return Expression.LessThan(fieldNameExpression, GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.LessThanOrEquals:
                    return Expression.LessThanOrEqual(fieldNameExpression, GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.GreaterThan:
                    return Expression.GreaterThan(fieldNameExpression, GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.GreaterThanOrEquals:
                    return Expression.GreaterThanOrEqual(fieldNameExpression, GetValueAsConstantExpression(filter.Value, fieldType));

                case SearchyRule.Range:
                    var rangeValues = PopulateRangeValues(filter.Value);
                    var lowerRangeExpression = Expression.GreaterThanOrEqual(fieldNameExpression, GetValueAsConstantExpression(rangeValues.Lower, fieldType));
                    var upperRangeExpression = Expression.LessThan(fieldNameExpression, GetValueAsConstantExpression(rangeValues.Upper, fieldType));
                    return Expression.AndAlso(lowerRangeExpression, upperRangeExpression);

                case SearchyRule.EqualsToList:
                    var containsMethod = typeof(Enumerable).GetMethods().Where(o => o.Name == "Contains" & o.GetParameters().Count() == 2).First();
                    containsMethod = containsMethod.MakeGenericMethod(fieldType);
                    var equalToListListType = typeof(List<>).MakeGenericType(new[] { fieldType });
                    var addMethod = equalToListListType.GetMethod("Add");
                    var equalToList = Activator.CreateInstance(equalToListListType);
                    foreach (var item in (IEnumerable)filter.Value)
                        addMethod.Invoke(equalToList, new object[] { ConvertValueToType(item, fieldType) });
                    //filter.Value = equalToList;
                    return Expression.Call(containsMethod, new[] { Expression.Constant(equalToList), fieldNameExpression });

                default:
                    return null;
            }
        }

        static Expression GetValueAsConstantExpression(object value, Type type)
        {
            var constant = Expression.Constant(ConvertValueToType(value, type));
            return Expression.Convert(constant, type);
        }

        static RangeValues PopulateRangeValues(object valueCollection)
        {
            var rangeValues = new RangeValues();
            int index = 0;
            foreach (var item in (IEnumerable)valueCollection)
            {
                if (index == 0)
                    rangeValues.Lower = item;
                else if (index == 1)
                    rangeValues.Upper = item;

                index++;
            }

            if (index != 2)
                throw new ArgumentException("Range rule value collection should contain exactly two values.");

            return rangeValues;
        }

        static object ConvertValueToType(object value, Type type)
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
