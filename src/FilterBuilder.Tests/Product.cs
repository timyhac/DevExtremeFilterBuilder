using System.Collections.Generic;

namespace FilterBuilder.Tests
{
    class Product
    {
        public int CurrentInventory { get; set; }
        public string Name { get; set; }
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
        public readonly List<Product> All;

        public readonly Product Product0 = new() { CurrentInventory = 9, Cost = 4.99f, Name = "Sauce", Category = ProductCategory.Food };
        public readonly Product Product1 = new() { CurrentInventory = 12, Cost = 1.50f, Name = "Beans", Category = ProductCategory.Food };
        public readonly Product Product2 = new() { CurrentInventory = 8, Cost = 3.20f, Name = "Chocolate", Category = ProductCategory.Food };
        public readonly Product Product3 = new() { CurrentInventory = 8, Cost = 175, Name = "SuperHD Video Player", Category = ProductCategory.Electronics };
        public readonly Product Product4 = new() { CurrentInventory = 3, Cost = 110, Name = "HD Video Player", Category = ProductCategory.Electronics };
        public readonly Product Product5 = new() { CurrentInventory = 0, Cost = 775, Name = "SuperLED 50", Category = ProductCategory.Electronics };
        public readonly Product Product6 = new() { CurrentInventory = 0, Cost = 675, Name = "SuperLED 42", Category = ProductCategory.Electronics };
        public readonly Product Product7 = new() { CurrentInventory = 1, Cost = 745f, Name = "SuperLCD 55", Category = ProductCategory.Electronics };
        public readonly Product Product8 = new() { CurrentInventory = 3, Cost = 710f, Name = "SuperLCD 42", Category = ProductCategory.Electronics };
        public readonly Product Product9 = new() { CurrentInventory = 2, Cost = 2125f, Name = "SuperLCD 70", Category = ProductCategory.Electronics };
        public readonly Product Product10 = new() { CurrentInventory = 1, Cost = 70f, Name = "DesktopLED 19", Category = ProductCategory.Electronics };
        public readonly Product Product11 = new() { CurrentInventory = 3, Cost = 120f, Name = "Table", Category = ProductCategory.Furniture };
        public readonly Product Product12 = new() { CurrentInventory = 8, Cost = 70f, Name = "Chair", Category = ProductCategory.Furniture };
        public readonly Product Product13 = new() { CurrentInventory = 2, Cost = 199.99f, Name = "Deluxe Office Chair", Category = ProductCategory.Furniture };

        public Products()
        {
            All = new() { Product0, Product1, Product2, Product3, Product4, Product5, Product6, Product7, Product8, Product9, Product10, Product11, Product12, Product13 };
        }
    }
}
