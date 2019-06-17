using CSN_Energy_Task;
using CSN_Energy_Task.Entity;
using CSN_Energy_Task.ValueObjects;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public class PriceCalculationTests
    {
        private static readonly ICalculationRule _calculationRule = new PriceCalculationRule();
        private Catalog book1 = Catalog.Create("book 1", Category.Create("Category One", 0.1m).Value, 10, 1).Value;
        private Catalog book2 = Catalog.Create("book 2", Category.Create("Category Two", 0.1m).Value, 5, 2).Value;
        private Catalog book3 = Catalog.Create("book 3", Category.Create("Category Two", 0.1m).Value, 8, 3).Value;
        private Catalog book4 = Catalog.Create("book 4", Category.Create("Category Three", 0.1m).Value, 3, 2).Value;
        private Catalog book5 = Catalog.Create("book 5", Category.Create("Category Three", 0.1m).Value, 7, 3).Value;
        private Catalog book6 = Catalog.Create("book 6", Category.Create("Category Three", 0.1m).Value, 9, 3).Value;

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void ShouldBuyOneBookTest()
        {
            //Arrange
            var catalog = new[] { book1 };
            //Act
            var value = _calculationRule.Calculate(catalog).Value;
            //Assert
            Assert.AreEqual(book1.Item.Price, value);
        }
    }
}