using CSharpFunctionalExtensions;
using CSN_Energy_Task.Entity;
using CSN_Energy_Task.Model;
using CSN_Energy_Task.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSN_Energy_Task
{
    public class PriceCalculationRule : ICalculationRule
    {
        public PriceCalculationRule()
        {
        }

        public Result<decimal> Calculate(IEnumerable<Catalog> catalogs)
        {
            var groupedByCategory = catalogs
                .GroupBy(x => x.Item.Category)
                .ToDictionary(x => x.Key, y => y.Select(catalog => catalog));

            var value = groupedByCategory.Sum(categoryGroup =>
            {
                var catalogsGroupedByCount = categoryGroup.Value.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                if (catalogsGroupedByCount.Any(c => c.Key.Quantity < c.Value))
                    //return Result.Fail<decimal>("Not enough books");
                    throw new NotEnoughInventoryException();

                var catalogSum = catalogsGroupedByCount.Sum(catalogCountPair =>
                {
                    if (catalogCountPair.Value == 1 && categoryGroup.Value.Count() == 1)
                        return catalogCountPair.Key.Item.Price;

                    return (catalogCountPair.Key.Item.Price * catalogCountPair.Key.Item.Category.Discount.ValueAsDecuction)
                        + (catalogCountPair.Key.Item.Price * (catalogCountPair.Value - 1));
                });
                return catalogSum;
            });

            return Result.Ok(value);
        }
    }

    public class NotEnoughInventoryException : Exception
    {
        public IEnumerable<INameQuantity> Missing { get; }
    }

    public interface INameQuantity
    {
        string Name { get; }

        int Quantity { get; }
    }
}
