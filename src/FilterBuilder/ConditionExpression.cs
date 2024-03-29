﻿using System;
using System.Linq.Expressions;

namespace DevExtremeFilterBuilder
{

    class ConditionExpression
    {
        Func<Expression, Expression, Expression> _func;

        ConditionExpression(Func<Expression, Expression, Expression> func) => _func = func;

        public static ConditionExpression Make(Func<Expression, Expression, Expression> func) => new ConditionExpression(func);

        public static ConditionExpression Make(Func<object, object, bool> func)
        {
            Expression<Func<object, object, bool>> methodExpression = (propertyValue, parameterValue) => func(propertyValue, parameterValue);
            return new ConditionExpression((a, b) => {
                var prop = Expression.Convert(a, typeof(object));
                var param = Expression.Convert(b, typeof(object));
                return Expression.Invoke(methodExpression, prop, param);
            });
        }

        public Expression Make(Expression operandA, Expression operandB) => _func(operandA, operandB);
    }

}
