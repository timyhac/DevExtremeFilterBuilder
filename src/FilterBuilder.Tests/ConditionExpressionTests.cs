using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FilterBuilder.Tests
{
    public class ConditionExpressionTests
    {

        readonly Products products = new Products();

        [Fact]
        public void Endswith_condition()
        {

            var jsonFilter = @"[""Name"", ""endswith"", ""e""]";

            var expectedFilteredList = new List<Product>()
            {
                products.Product0, products.Product2, products.Product11
            };

            FilterBuilder builder = new();
            var predicate = builder.GetExpression<Product>(jsonFilter).Compile();

            var actualFilteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, actualFilteredList);

        }

        [Fact]
        public void Custom_condition_operator()
        {

            var jsonFilter = @"[""Category"", ""anyof"", [""Food"", ""Furniture""]]";

            var expectedFilteredList = new List<Product>
            {
                products.Product0, products.Product1, products.Product2, products.Product11, products.Product12, products.Product13
            };

            FilterBuilder builder = new();

            builder.RegisterOperator("anyof",
            (propertyName, parameterElement) =>
            {
                return parameterElement.EnumerateArray()
                            .Select(x => x.GetString())
                            .Select(x => Enum.Parse<ProductCategory>(x))
                            .ToArray();
            },
            (value, parameter) =>
            {
                var allowedValues = (ProductCategory[])parameter;
                var actualValue = (ProductCategory)value;
                return allowedValues.Contains(actualValue);
            });

            var predicate = builder.GetExpression<Product>(jsonFilter).Compile();
            var filteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, filteredList);

        }

        [Fact]
        public void Between_condition()
        {

            var jsonFilter = @"[""Cost"", ""between"", [100, 200]]";

            var expectedFilteredList = new List<Product>()
            {
                products.Product3, products.Product4, products.Product11, products.Product13
            };

            FilterBuilder builder = new();
            var predicate = builder.GetExpression<Product>(jsonFilter).Compile();

            var actualFilteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, actualFilteredList);

        }
    }
}
