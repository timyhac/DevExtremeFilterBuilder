using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FilterBuilder.Tests
{
    public class GroupExpressionTests
    {

        readonly List<Product> items;

        readonly Product item0 = new() { CurrentInventory = 9, Cost = 4.99f, Name = "Sauce", Category = ProductCategory.Food };
        readonly Product item1 = new() { CurrentInventory = 12, Cost = 1.50f, Name = "Beans", Category = ProductCategory.Food };
        readonly Product item2 = new() { CurrentInventory = 8, Cost = 3.20f, Name = "Chocolate", Category = ProductCategory.Food };
        readonly Product item3 = new() { CurrentInventory = 8, Cost = 175, Name = "SuperHD Video Player", Category = ProductCategory.Electronics };
        readonly Product item4 = new() { CurrentInventory = 3, Cost = 110, Name = "HD Video Player", Category = ProductCategory.Electronics };
        readonly Product item5 = new() { CurrentInventory = 0, Cost = 775, Name = "SuperLED 50", Category = ProductCategory.Electronics };
        readonly Product item6 = new() { CurrentInventory = 0, Cost = 675, Name = "SuperLED 42", Category = ProductCategory.Electronics };
        readonly Product item7 = new() { CurrentInventory = 1, Cost = 745f, Name = "SuperLCD 55", Category = ProductCategory.Electronics };
        readonly Product item8 = new() { CurrentInventory = 3, Cost = 710f, Name = "SuperLCD 42", Category = ProductCategory.Electronics };
        readonly Product item9 = new() { CurrentInventory = 2, Cost = 2125f, Name = "SuperLCD 70", Category = ProductCategory.Electronics };
        readonly Product item10 = new() { CurrentInventory = 1, Cost = 70f, Name = "DesktopLED 19", Category = ProductCategory.Electronics };
        readonly Product item11 = new() { CurrentInventory = 3, Cost = 120f, Name = "Table", Category = ProductCategory.Furniture };
        readonly Product item12 = new() { CurrentInventory = 8, Cost = 70f, Name = "Chair", Category = ProductCategory.Furniture };
        readonly Product item13 = new() { CurrentInventory = 2, Cost = 199.99f, Name = "Deluxe Office Chair", Category = ProductCategory.Furniture };


        public GroupExpressionTests()
        {
            items = new() { item0, item1, item2, item3, item4, item5, item6, item7, item8, item9, item10, item11, item12, item13 };
        }

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

            var predicate = builder.Get<Product>(jsonFilter).Compile();
            var filteredLst = list.Where(predicate);

        }

    }
}
