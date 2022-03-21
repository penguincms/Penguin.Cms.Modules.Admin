using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Penguin.Cms.Modules.Admin
{
    public static class ExpressionBuilder
    {
        private static readonly MethodInfo? ToStringMethod = typeof(object).GetMethod("ToString");
        private static readonly MethodInfo StringContainsMethod = typeof(string).GetMethods().Single(m => m.Name == nameof(string.Contains) && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(string));

        public static Expression? AnyPropertyContains<T>(string q, Type objectType)
        {
            MethodInfo m = typeof(ExpressionBuilder).GetMethods().Single(m => m.Name == nameof(ExpressionBuilder.AnyPropertyContains) && m.GetParameters().Length == 1).MakeGenericMethod(objectType);

            Expression? result = m.Invoke(null, new object[] { q }) as Expression;

            return result;
        }

        public static Expression<Func<T, bool>> AnyPropertyContains<T>(string q)
        {
            ConstantExpression query = Expression.Constant(q);
            Type type = typeof(T);
            ParameterExpression lambdaParam = Expression.Parameter(type);
            List<MethodCallExpression> predicates = type.GetProperties().Where(p => p.PropertyType == typeof(string) || !p.PropertyType.IsClass).Select(p => PredicateContainsBuilder(lambdaParam, p, query)).ToList();
            Expression body = predicates[0];
            body = predicates.Skip(1).Aggregate(body, Expression.OrElse);
            return Expression.Lambda<Func<T, bool>>(body, lambdaParam);
        }

        private static MethodCallExpression PredicateContainsBuilder(Expression lambdaParam, PropertyInfo prop, Expression query) => Expression.Call(Expression.Call(Expression.Property(lambdaParam, prop), ToStringMethod), StringContainsMethod, query);
    }
}
