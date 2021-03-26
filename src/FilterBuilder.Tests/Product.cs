using System.Collections.Generic;

namespace DevExtremeFilterBuilder.Tests
{
    class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentInventory { get; set; }
        public ProductCategory Category { get; set; }
        public float Cost { get; set; }
    }

    enum ProductCategory
    {
        Food,
        Electronics,
        Furniture
    }

    class Products
    {
        public readonly List<Product> All = new()
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

        public Product this[int index] => All[index];
    }
}
