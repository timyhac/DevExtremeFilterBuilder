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
}
