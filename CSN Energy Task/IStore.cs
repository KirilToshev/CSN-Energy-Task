using CSharpFunctionalExtensions;

namespace CSN_Energy_Task
{
    public interface IStore
    {
        Result Import(string catalogAsJson);
        Result<int> Quantity(string name);
        Result<decimal> Buy(params string[] basketByNames);
    }
}
