using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtremeFilterBuilder.Tests
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
                products[0], products[2], products[11]
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
                products[0], products[1], products[2], products[11], products[12], products[13]
            };

            FilterBuilder filterBuilder = new();
            filterBuilder.RegisterOperator("anyof",
            (object value, object parameter) =>
            {
                var allowedValues = ((object[])parameter).Select(x => (ProductCategory)x).ToArray();
                var actualValue = (ProductCategory)value;
                return allowedValues.Contains(actualValue);
            });

            var predicate = filterBuilder.GetExpression<Product>(jsonFilter).Compile();
            var filteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, filteredList);

        }

        [Fact]
        public void Equal_to_enum_using_default_parser()
        {

            var jsonFilter = @"[""Category"", ""="", ""Food""]";

            var expectedFilteredList = new List<Product>
            {
                products[0], products[1], products[2]
            };

            FilterBuilder filterBuilder = new();

            var predicate = filterBuilder.GetExpression<Product>(jsonFilter).Compile();
            var filteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, filteredList);
        }

        [Fact]
        public void Between_two_numbers()
        {

            var jsonFilter = @"[""Cost"", ""between"", [100, 200]]";

            var expectedFilteredList = new List<Product>()
            {
                products[3], products[4], products[11], products[13]
            };

            FilterBuilder builder = new();
            var predicate = builder.GetExpression<Product>(jsonFilter).Compile();

            var actualFilteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, actualFilteredList);

        }
    }
}
