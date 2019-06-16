using CSharpFunctionalExtensions;

namespace CSN_Energy_Task.ValueObjects
{
    public class Category : ValueObject<Category>
    {
        private Category(string name, Percentage discount)
        {
            this.Name = name;
            this.Discount = discount;
        }

        public string Name { get; set; }
        public Percentage Discount { get; set; }

        public static Result<Category> Create(string name, decimal discount)
        {
            return Percentage.Create(discount)
                .Map(p => new Category(name, p));
        }

        protected override bool EqualsCore(Category other)
        {
            return this.Name.Equals(other.Name)
                && this.Discount.Equals(other.Discount);
        }

        protected override int GetHashCodeCore()
        {
            return this.Name.GetHashCode() ^ 832
                & this.Discount.GetHashCode() ^ 952;
        }
    }
}
