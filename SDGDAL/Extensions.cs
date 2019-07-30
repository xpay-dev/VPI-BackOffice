using System;
using System.Linq;
using System.Linq.Expressions;

namespace SDGDAL
{
    public static class Extensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string memberName)
        {
            ParameterExpression[] typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };

            System.Reflection.PropertyInfo pi = typeof(T).GetProperty(memberName);

            return (IOrderedQueryable<T>)query.Provider.CreateQuery(
            Expression.Call(
             typeof(Queryable),
             "OrderBy",
             new Type[] { typeof(T), pi.PropertyType },
             query.Expression,
             Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
            );
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string memberName)
        {
            ParameterExpression[] typeParams = new ParameterExpression[] { Expression.Parameter(typeof(T), "") };

            System.Reflection.PropertyInfo pi = typeof(T).GetProperty(memberName);

            return (IOrderedQueryable<T>)query.Provider.CreateQuery(
            Expression.Call(
             typeof(Queryable),
             "OrderByDescending",
             new Type[] { typeof(T), pi.PropertyType },
             query.Expression,
             Expression.Lambda(Expression.Property(typeParams[0], pi), typeParams))
            );
        }
    }
}