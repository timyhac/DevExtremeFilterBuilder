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

        [Fact]
        public void Group_with_two_condition_expressions()
        {

            var jsonFilter = @"
[
    [""Name"", ""contains"", ""HD""],
    ""and"",
    [""Cost"", ""<"", 200]
]";


            var expectedFilteredList = new List<Product>()
            {
                products.Product3, products.Product4
            };

            FilterBuilder builder = new();
            var expression = builder.GetExpression<Product>(jsonFilter);
            var predicate = expression.Compile();

            var actualFilteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, actualFilteredList);

        }

        [Fact]
        public void Group_with_three_condition_expressions()
        {

            var jsonFilter = @"
[
    [""Name"", ""contains"", ""HD""],
    ""and"",
    [""Cost"", ""<"", 200],
    ""and"",
    [""Cost"", "">"", 150]
]";


            var expectedFilteredList = new List<Product>()
            {
                products.Product3
            };

            FilterBuilder builder = new();
            var expression = builder.GetExpression<Product>(jsonFilter);
            var predicate = expression.Compile();

            var actualFilteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, actualFilteredList);

        }

    }
}
