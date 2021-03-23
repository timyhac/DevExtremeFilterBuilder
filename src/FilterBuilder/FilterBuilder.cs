using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace FilterBuilder
{
    public class FilterBuilder
    {

        string[] builtInGroupOperators = new[]
        {
            "and",
            "or"
        };


        readonly Dictionary<string, ConditionExpression> conditionOperators;
        readonly Dictionary<string, Func<JsonElement, object>> customParsers = new Dictionary<string, Func<JsonElement, object>>();


        public FilterBuilder()
        {

            var stringContainsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var stringStartsWithMethodInfo = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var stringEndsWithMethodInfo = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

            conditionOperators = new Dictionary<string, ConditionExpression>()
            {
                {  "<",             ConditionExpression.Make((a,b) => Expression.LessThan(a,b))},
                {  "<=",            ConditionExpression.Make((a,b) => Expression.LessThanOrEqual(a,b))},
                {  ">",             ConditionExpression.Make((a,b) => Expression.GreaterThan(a,b))},
                {  ">=",            ConditionExpression.Make((a,b) => Expression.GreaterThanOrEqual(a,b))},
                {  "=",             ConditionExpression.Make((a,b) => Expression.Equal(a,b))},
                {  "<>",            ConditionExpression.Make((a,b) => Expression.NotEqual(a,b))},
                {  "contains",      ConditionExpression.Make((a,b) => Expression.Call(a, stringContainsMethodInfo, b))},
                {  "notcontains",   ConditionExpression.Make((a,b) => Expression.Not(Expression.Call(a, stringContainsMethodInfo, b)))},
                {  "startswith",    ConditionExpression.Make((a,b) => Expression.Call(a, stringStartsWithMethodInfo, b))},
                {  "endswith",      ConditionExpression.Make((a,b) => Expression.Call(a, stringEndsWithMethodInfo, b))},
                {  "between",       ConditionExpression.Make((a,b) =>
                    {
                        var parameters = ((object[])b).Select(x => (double)x).ToArray();
                        var value = Convert.ToDouble(a);

                        var lowerBound = parameters[0];
                        var upperBound = parameters[1];
                        return lowerBound < value && value < upperBound;
                    })},
            };



        }

        public Expression<Func<T, bool>> GetExpression<T>(string jsonFilter)
        {

            var json = JsonDocument.Parse(jsonFilter);

            var param = Expression.Parameter(typeof(T));
            var body = GetExpression(param, json.RootElement);

            var lambda = Expression.Lambda<Func<T, bool>>(body, param);

            return lambda;

        }


        public void RegisterOperator(string @operator, Func<object, object, bool> operatorFunction)
            => conditionOperators.Add(@operator, ConditionExpression.Make(operatorFunction));


        public void RegisterParser(string parameterName, Func<JsonElement, object> parserFunction)
            => customParsers.Add(parameterName, parserFunction);



        Expression GetExpression(ParameterExpression @object, JsonElement el)
        {
            if (el.ValueKind != JsonValueKind.Array)
                throw new Exception();

            else if (IsANotExpression(el))
                return Expression.Not(GetExpression(@object, el[1]));

            else if (IsGroupExpression(el))
                return GetGroupExpression(@object, el);

            else if (IsConditionExpression(el))
                return GetConditionExpression(@object, el);
            
            else
                throw new ArgumentOutOfRangeException("Unknown expression type");
        }
        
        
        bool IsANotExpression(JsonElement el)
            => el.GetArrayLength() == 2 && el[0].GetString() == "!";



        bool IsGroupExpression(JsonElement el)
        {
            var @operator = el[1].GetString();
            return builtInGroupOperators.Contains(@operator);
        }

        

        bool IsConditionExpression(JsonElement el)
        {
            var @operator = el[1].GetString();
            return conditionOperators.ContainsKey(@operator);
        }


        Expression GetConditionExpression(ParameterExpression @object, JsonElement el)
        {
            var propertyOrFieldName = el[0].GetString();
            string @operator = el[1].GetString();
            object parameter = GetParameter(propertyOrFieldName, el[2]);

            return GetConditionExpression(@object, @operator, propertyOrFieldName, parameter);
        }

        object GetParameter(string propertyOrFieldName, JsonElement el)
        {

            if (customParsers.ContainsKey(propertyOrFieldName))
                return customParsers[propertyOrFieldName](el);

            else
                return ParseConstant(el);

        }

        object ParseConstant(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Array:       return el.EnumerateArray().Select(item => ParseConstant(item)).ToArray();
                case JsonValueKind.String:      return el.GetString();
                case JsonValueKind.Number:      return el.GetDouble();
                case JsonValueKind.True:        return true;
                case JsonValueKind.False:       return false;
                case JsonValueKind.Null:        return null;
                case JsonValueKind.Undefined:
                case JsonValueKind.Object:
                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        Expression GetConditionExpression(ParameterExpression @object, string @operator, string propertyOrFieldName, object parameter)
        {
            var propertyExpression = Expression.PropertyOrField(@object, propertyOrFieldName);
            var parameterExpression = Expression.Constant(parameter);
            return conditionOperators[@operator].Make(propertyExpression, parameterExpression);
        }

        Expression GetGroupExpression(ParameterExpression @object, JsonElement el, int index = 0)
        {

            if (index == el.GetArrayLength()-1)
                return GetExpression(@object, el[index]);

            var conditionA = GetExpression(@object, el[index]);
            var @operator = el[index + 1].GetString();
            var conditionB = GetGroupExpression(@object, el, index+2);

            if (@operator == "and")
                return Expression.MakeBinary(ExpressionType.And, conditionA, conditionB);

            else if (@operator == "or")
                return Expression.MakeBinary(ExpressionType.Or, conditionA, conditionB);

            else
                throw new NotImplementedException();

        }

    }
}
