using CSharpFunctionalExtensions;
using CSN_Energy_Task;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace IntegrationTests
{
    public class BookStoreTests
    {
        BookStore _store;

        public BookStoreTests()
        {
            
        }

        [SetUp]
        public void Setup()
        {
            _store = BookStore.Create(
                Path.Combine(Program.AssemblyDirectory, "Assets", "bookStoreInventorySchema.json"),
                new PriceCalculationRule()).Value;

            var exampleFilePath = Path.Combine(Program.AssemblyDirectory, "Assets", "bookStoreInventoryExample.json");
            var jsonExampleResult = File.ReadAllText(exampleFilePath);

            _store.Import(jsonExampleResult);
        }

        [Test]
        public void ShouldBuyOneBook()
        {
            var value = _store.Buy("J.K Rowling - Goblet Of fire");
            Assert.AreEqual(
                _store.Catalogs.First(x => x.Item.Name == "J.K Rowling - Goblet Of fire").Item.Price, 
                value.Value);
        }

        [Test]
        public void ShouldBuyTwoBooksSameCategoryGetDiscount()
        {
            var value = _store.Buy("Robin Hobb - Assassin Apprentice", "J.K Rowling - Goblet Of fire");
            Assert.AreEqual(
                (8 * 0.9m) + (12 * 0.9m),
                value.Value);
        }

        [Test]
        public void ShouldBuyThreeBooksSameCategoryGetDiscount()
        {
            var value = _store.Buy("J.K Rowling - Goblet Of fire", "Robin Hobb - Assassin Apprentice", "Robin Hobb - Assassin Apprentice");
            Assert.AreEqual(
                8 * 0.9 + 12 * 0.9 + 12,
                value.Value);
        }

        [Test]
        public void ShouldBuySevenBooksSameCategoryGetDiscount()
        {
            var value = _store.Buy(
                "Ayn Rand - FountainHead",
                "Isaac Asimov - Foundation",
                "Isaac Asimov - Robot series",
                "J.K Rowling - Goblet Of fire",
                "J.K Rowling - Goblet Of fire",
                "Robin Hobb - Assassin Apprentice",
                "Robin Hobb - Assassin Apprentice"
                );
            Assert.AreEqual(
                12 + 5 * 0.95 + 16 * 0.95 + 8 * 0.9 + 8 + 12 * 0.9 + 12,
                value.Value);
        }

        [Test]
        public void ShouldThrowExceptionWhenBuyMoreBooksThanCatalogContains()
        {
            Assert.Throws<NotEnoughInventoryException>(() => _store.Buy("Isaac Asimov - Foundation", "Isaac Asimov - Foundation"));
        }
    }
}