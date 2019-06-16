using CSharpFunctionalExtensions;

namespace CSN_Energy_Task.ValueObjects
{
    public sealed class Percentage : ValueObject<Percentage>
    {
        private Percentage(decimal value)
        {
            this.ValueAsFraction = value;
        }

        public decimal Value { get { return this.ValueAsFraction * 100; } }

        public decimal ValueAsFraction { get; private set; }

        public decimal ValueAsDecuction
        {
            get { return 1 - this.ValueAsFraction; }
        }

        public static Result<Percentage> Create(decimal value)
        {
            return Result.Ok()
                .Ensure(() => value >= 0 && value <= 100, 
                string.Format("Invalid percentage value {0}. Percentage value should be a fraction number in the range of 0 to 1", value))
                .Map(() => new Percentage(value));
        }

        protected override bool EqualsCore(Percentage other)
        {
            return this.Value.Equals(other.Value);
        }

        protected override int GetHashCodeCore()
        {
            return (this.Value.GetHashCode() ^ 697) | 83242;
        }
    }
}
