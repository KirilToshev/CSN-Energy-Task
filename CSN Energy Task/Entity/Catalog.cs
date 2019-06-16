using CSharpFunctionalExtensions;
using CSN_Energy_Task.ValueObjects;
using System;

namespace CSN_Energy_Task.Entity
{
    public class Catalog : IEquatable<Catalog>
    {
        private Catalog(CatalogItem item, int quantity)
        {
            this.Item = item;
            this.Quantity = quantity;
        }

        public CatalogItem Item { get; private set; }
        public int Quantity { get; private set; }

        public static Result<Catalog> Create(string name, Category category, decimal price, int quantity)
        {
            return CatalogItem.Create(name, category, price)
                .Ensure(ci => quantity >= 0, "Quantity value should not be negative")
                .Map(ci => new Catalog(ci, quantity));
        }

        public override bool Equals(object obj)
        {
            var other = obj as Catalog;
            if (other == null)
                return false;

            return this.Equals(other);
        }

        public bool Equals(Catalog other)
        {
            return this.Item.Equals(other.Item);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item);
        }
    }
}
