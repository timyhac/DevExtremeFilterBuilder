using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtremeFilterBuilder.Tests
{
    public class GroupExpressionTests
    {

        readonly Products products = new Products();



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


        [Fact]
        public void Group_with_sub_group()
        {

            var jsonFilter = @"
[
    [""Name"", ""contains"", ""HD""],
    ""or"",
    [
        [""Cost"", ""<"", 200],
        ""and"",
        [""Cost"", "">"", 100]
    ]
]";

            var expectedFilteredList = new List<Product>()
            {
                products.Product3, products.Product4, products.Product11, products.Product13
            };

            FilterBuilder builder = new();
            var expression = builder.GetExpression<Product>(jsonFilter);
            var predicate = expression.Compile();

            var actualFilteredList = products.All.Where(predicate);

            Assert.Equal(expectedFilteredList, actualFilteredList);

        }

    }
}
