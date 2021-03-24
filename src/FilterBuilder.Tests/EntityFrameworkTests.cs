using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace DevExtremeFilterBuilder.Tests
{
    public class EntityFrameworkTests : IDisposable
    {

        readonly ProductsContext dbContext;
        readonly List<Product> items = new()
        {
            new() { Name = "Sauce",                 Description =   "",         CurrentInventory = 9,   Cost = 4.99f,   Category = ProductCategory.Food },
            new() { Name = "Beans",                 Description =   "",         CurrentInventory = 12,  Cost = 1.50f,   Category = ProductCategory.Food },
            new() { Name = "Chocolate",             Description =   "",         CurrentInventory = 8,   Cost = 3.20f,   Category = ProductCategory.Food },
            new() { Name = "SuperHD Video Player",  Description =   "",         CurrentInventory = 8,   Cost = 175,     Category = ProductCategory.Electronics },
            new() { Name = "HD Video Player",       Description =   "",         CurrentInventory = 3,   Cost = 110,     Category = ProductCategory.Electronics },
            new() { Name = "SuperLED 50",           Description =   "",         CurrentInventory = 0,   Cost = 775,     Category = ProductCategory.Electronics },
            new() { Name = "SuperLED 42",           Description =   "",         CurrentInventory = 0,   Cost = 675,     Category = ProductCategory.Electronics },
            new() { Name = "SuperLCD 55",           Description =   null,       CurrentInventory = 1,   Cost = 745f,    Category = ProductCategory.Electronics },
            new() { Name = "SuperLCD 42",           Description =   "",         CurrentInventory = 3,   Cost = 710f,    Category = ProductCategory.Electronics },
            new() { Name = "SuperLCD 70",           Description =   "",         CurrentInventory = 2,   Cost = 2125f,   Category = ProductCategory.Electronics },
            new() { Name = "DesktopLED 19",         Description =   "",         CurrentInventory = 1,   Cost = 70f,     Category = ProductCategory.Electronics },
            new() { Name = "Table",                 Description =   "Wood",     CurrentInventory = 3,   Cost = 120f,    Category = ProductCategory.Furniture },
            new() { Name = "Chair",                 Description =   "Wood",     CurrentInventory = 8,   Cost = 70f,     Category = ProductCategory.Furniture },
            new() { Name = "Deluxe Office Chair",   Description =   "Plastic",  CurrentInventory = 2,   Cost = 199.99f, Category = ProductCategory.Furniture },
        };


        public EntityFrameworkTests()
        {

            dbContext = new ProductsContext();
            dbContext.Database.EnsureCreated();

            dbContext.Products.AddRange(items);
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            dbContext.Products.RemoveRange(dbContext.Products);
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

            var actualList = dbContext.Products.Where(predicate);

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

            builder.RegisterParser("Category", el => el.EnumerateArray().Select(x => (object)Enum.Parse<ProductCategory>(x.GetString())).ToArray());
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

            var actualList = dbContext.Products.Where(predicate);

            Assert.True(ContainSameElements(expectedFilteredList, actualList));

        }
    }
}
