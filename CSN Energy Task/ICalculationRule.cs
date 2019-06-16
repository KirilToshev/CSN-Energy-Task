using CSharpFunctionalExtensions;
using CSN_Energy_Task.Entity;
using System.Collections.Generic;

namespace CSN_Energy_Task
{
    public interface ICalculationRule
    {
        Result<decimal> Calculate(IEnumerable<Catalog> catalogs);
    }
}
