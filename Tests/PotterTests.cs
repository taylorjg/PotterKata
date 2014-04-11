using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class PotterTests
    {
        private const double UnitBookPrice = 8d;

        [Test]
        public void NoBooksCostZero()
        {
            var price = CalculatePriceForBooks(string.Empty);
            Assert.That(price, Is.EqualTo(0d));
        }

        [TestCase("A")]
        [TestCase("B")]
        [TestCase("C")]
        [TestCase("D")]
        [TestCase("E")]
        public void ASingleBookIsPricedCorrectly(string books)
        {
            var price = CalculatePriceForBooks(books);
            Assert.That(price, Is.EqualTo(UnitBookPrice));
        }

        [TestCase("AA", 2 * UnitBookPrice)]
        [TestCase("AAA", 3 * UnitBookPrice)]
        [TestCase("AAAA", 4 * UnitBookPrice)]
        [TestCase("AAAAA", 5 * UnitBookPrice)]
        [TestCase("BB", 2 * UnitBookPrice)]
        [TestCase("BBB", 3 * UnitBookPrice)]
        [TestCase("BBBB", 4 * UnitBookPrice)]
        [TestCase("BBBBB", 5 * UnitBookPrice)]
        [TestCase("AAAAAA", 6 * UnitBookPrice)]
        [TestCase("AAAAAAA", 7 * UnitBookPrice)]
        [TestCase("AAAAAAAA", 8 * UnitBookPrice)]
        [TestCase("AAAAAAAAA", 9 * UnitBookPrice)]
        [TestCase("AAAAAAAAAA", 10 * UnitBookPrice)]
        public void MultipleBooksOfTheSameTypeArePricedCorrectly(string books, double expectedPrice)
        {
            var price = CalculatePriceForBooks(books);
            Assert.That(price, Is.EqualTo(expectedPrice));
        }

        [TestCase("AB", 2 * UnitBookPrice * 0.95d)]
        [TestCase("ABC", 3 * UnitBookPrice * 0.90d)]
        [TestCase("CDE", 3 * UnitBookPrice * 0.90d)]
        [TestCase("ACE", 3 * UnitBookPrice * 0.90d)]
        [TestCase("ABCD", 4 * UnitBookPrice * 0.80d)]
        [TestCase("BCDE", 4 * UnitBookPrice * 0.80d)]
        [TestCase("ABCDE", 5 * UnitBookPrice * 0.75d)]
        [TestCase("EDCBA", 5 * UnitBookPrice * 0.75d)]
        public void MultipleBooksOfDifferentTypesArePricedCorrectly(string books, double expectedPrice)
        {
            var price = CalculatePriceForBooks(books);
            Assert.That(price, Is.EqualTo(RoundUp(expectedPrice)));
        }

        private static double RoundUp(double d)
        {
            return System.Math.Round(d, 2);
        }

        private readonly static IDictionary<int, int> NumDifferentItems2PercentDiscount = new Dictionary<int, int>
            {
                {0, 0},
                {1, 0},
                {2, 5},
                {3, 10},
                {4, 20},
                {5, 25}
            };

        private static double CalculatePriceForBooks(string books)
        {
            var numBooks = books.Length;
            double? price = null;

            if (books.ToCharArray().Distinct().Count() == numBooks)
            {
                var percentDiscount = NumDifferentItems2PercentDiscount[numBooks];
                price = (numBooks * UnitBookPrice) / 100 * (100 - percentDiscount);
            }

            if (!price.HasValue)
            {
                price = numBooks * UnitBookPrice;
            }

            return RoundUp(price.Value);
        }
    }
}
