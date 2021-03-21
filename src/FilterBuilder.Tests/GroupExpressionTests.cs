using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FilterBuilder.Tests
{
    public class GroupExpressionTests
    {

        readonly Products products = new Products();


        [Fact]
        public void Test1()
        {

            var jsonFilter = @"
[
    [""CurrentInventory"", ""anyof"", [8, 12]],
    ""or"",
    [
        [""Name"", ""contains"", ""HD""],
        ""and"",
        [""Cost"", ""<"", 200]
    ]
]";


            var list = new List<Product>
            {
                
            };


            FilterBuilder builder = new();

            builder.RegisterOperator("anyof",
            (propertyName, parameterElement) => {

                return parameterElement.EnumerateArray()
                            .Select(item => item.GetInt32())
                            .ToArray();

            }, (value, parameter) =>
            {
                var allowedValues = (int[])parameter;
                var actualValue = (int)value;
                return allowedValues.Contains(actualValue);
            });

            var predicate = builder.GetExpression<Product>(jsonFilter).Compile();
            var filteredLst = products.All.Where(predicate);

        }

    }
}
