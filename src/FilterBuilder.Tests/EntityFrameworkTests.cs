using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace DevExtremeFilterBuilder.Tests
{
    public class EntityFrameworkTests : IDisposable
    {

        readonly ProductsContext dbContext;
        readonly Products items = new Products();

        public EntityFrameworkTests()
        {
            dbContext = new ProductsContext();
            dbContext.Database.EnsureCreated();

            dbContext.Products.AddRange(items.All);
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            dbContext.Products.RemoveRange(dbContext.Products);
            dbContext.Manufacturers.RemoveRange(dbContext.Manufacturers);
            dbContext.SaveChanges();
        }

        [Fact]
        public void Test4()
        {

            var jsonFilter = @"[""Name"", ""endswith"", ""e""]";

            var expectedFilteredList = new List<Product>()
            {
                items[0], items[2], items[11]
            };

            FilterBuilder builder = new();
            var predicate = builder.GetExpression<Product>(jsonFilter);

            var query = dbContext.Products.Where(predicate).ToQueryString();
            var actualList = dbContext.Products.Where(predicate).ToList();

            Assert.True(ContainSameElements(expectedFilteredList, actualList));
        }


        bool ContainSameElements<T>(IEnumerable<T> A, IEnumerable<T> B)
        {
            bool areSame = !A.Except(B).Any() && A.Count() == B.Count();
            return areSame;
        }

        [Fact]
        public void sdfg()
        {

            var jsonFilter = @"[""Description"", ""anyof"", [""Wood""]]";

            var expectedFilteredList = new List<Product>()
            {
                items[11], items[12]
            };

            FilterBuilder builder = new();

            builder.RegisterOperator("anyof",
            (Expression value, Expression parameter) =>
            {
                var method = typeof(Enumerable)
                               .GetMethods()
                               .Where(m => m.Name == "Contains")
                               .Single(m => m.GetParameters().Length == 2)
                               .MakeGenericMethod(typeof(object));

                var v = Expression.TypeAs(value, typeof(object));
                return Expression.Call(method, parameter, v);
            });


            var predicate = builder.GetExpression<Product>(jsonFilter);

            var query = dbContext.Products.Where(predicate).ToQueryString();
            var actualList = dbContext.Products.Where(predicate).ToList();

            Assert.True(ContainSameElements(expectedFilteredList, actualList));

        }

        [Fact]
        public void Enum_not_equal()
        {

            var jsonFilter = @"[""Category"", ""<>"", ""Electronics""]";

            var expectedFilteredList = new List<Product>()
            {
                items[0], items[1], items[2], items[11], items[12], items[13]
            };

            FilterBuilder builder = new();

            var predicate = builder.GetExpression<Product>(jsonFilter);

            var query = dbContext.Products.Where(predicate).ToQueryString();
            var actualList = dbContext.Products.Where(predicate).ToList();

            Assert.True(ContainSameElements(expectedFilteredList, actualList));

        }

        [Fact]
        public void Custom_condition_operator()
        {

            var jsonFilter = @"[""Category"", ""anyof"", [""Food"", ""Furniture""]]";

            var expectedFilteredList = new List<Product>()
            {
                items[0], items[1], items[2], items[11], items[12], items[13]
            };

            FilterBuilder builder = new();

            builder.RegisterOperator("anyof",
            (Expression value, Expression parameter) =>
            {
                var method = typeof(Enumerable)
                               .GetMethods()
                               .Where(m => m.Name == "Contains")
                               .Single(m => m.GetParameters().Length == 2)
                               .MakeGenericMethod(typeof(object));

                var b = Expression.TypeAs(value, typeof(object));
                return Expression.Call(method, parameter, b);
            });


            var predicate = builder.GetExpression<Product>(jsonFilter);

            var query = dbContext.Products.Where(predicate).ToQueryString();
            var actualList = dbContext.Products.Where(predicate).ToList();

            Assert.True(ContainSameElements(expectedFilteredList, actualList));

        }
    }
}
