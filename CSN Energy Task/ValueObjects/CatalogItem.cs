using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSN_Energy_Task.ValueObjects
{
    public class CatalogItem : ValueObject<CatalogItem>
    {
        private CatalogItem(string name, Category category, decimal price)
        {
            this.Name = name;
            this.Category = category;
            this.Price = price;
        }

        public string Name { get; private set; }
        public Category Category { get; private set; }
        public decimal Price { get; private set; }

        public static Result<CatalogItem> Create(string name, Category category, decimal price)
        {
            return Result.Ok()
                .Ensure(() => price >= 0, "Price of the book should be positive value")
                .Map(() => new CatalogItem(name, category, price));
        }

        protected override bool EqualsCore(CatalogItem other)
        {
            return this.Name.Equals(other.Name)
                && this.Category.Equals(other.Category)
                && this.Price.Equals(other.Price);
        }

        protected override int GetHashCodeCore()
        {
            return this.Name.GetHashCode() ^ 503
                | this.Category.GetHashCode() & 362
                ^ this.Price.GetHashCode() ^ 862;
        }
    }
}
