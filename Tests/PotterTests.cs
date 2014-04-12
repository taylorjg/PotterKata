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
            AssertPrice(price, 0 * UnitBookPrice);
        }

        [TestCase("A")]
        [TestCase("B")]
        [TestCase("C")]
        [TestCase("D")]
        [TestCase("E")]
        public void ASingleBookIsPricedCorrectly(string books)
        {
            var price = CalculatePriceForBooks(books);
            AssertPrice(price, 1 * UnitBookPrice);
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
            AssertPrice(price, expectedPrice);
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
            AssertPrice(price, expectedPrice);
        }

        [TestCase("AAB", UnitBookPrice + (2 * UnitBookPrice * 0.95d))]
        [TestCase("AABB", 2 * (2 * UnitBookPrice * 0.95d))]
        [TestCase("AABCCD", (4 * UnitBookPrice * 0.80d) + (2 * UnitBookPrice * 0.95d))]
        [TestCase("ABBCDE", UnitBookPrice + (5 * UnitBookPrice * 0.75d))]
        public void SimpleDiscountCombinationsArePricedCorrectly(string books, double expectedPrice)
        {
            var price = CalculatePriceForBooks(books);
            AssertPrice(price, expectedPrice);
        }

        [TestCase("AABBCCDE", 2 * (4 * UnitBookPrice * 0.80d))] // = ABCDE + ABC or ABCD + ABCE
        public void EdgeCaseIsPricedCorrectly(string books, double expectedPrice)
        {
            var price = CalculatePriceForBooks(books);
            AssertPrice(price, expectedPrice);
        }

        private static void AssertPrice(double actualPrice, double expectedPrice)
        {
            Assert.That(actualPrice, Is.EqualTo(expectedPrice));
        }

        private readonly static IDictionary<int, int> NumDifferentBooks2PercentDiscount = new Dictionary<int, int>
            {
                {0, 0},
                {1, 0},
                {2, 5},
                {3, 10},
                {4, 20},
                {5, 25}
            };

        private static IEnumerable<char> ExtractDistinctSetOfBooks(ICollection<char> books, int size)
        {
            var distinctSetOfBooks = books.Distinct().ToList();
            var diff = distinctSetOfBooks.Count() - size;
            if (diff < 0)
            {
                return Enumerable.Empty<char>();
            }
            if (diff > 0)
            {
                distinctSetOfBooks.RemoveRange(0, diff);
            }
            foreach (var book in distinctSetOfBooks) books.Remove(book);
            return distinctSetOfBooks;
        }

        private static int GetSizeOfLargestDistinctSetOfBooks(IEnumerable<char> books)
        {
            return books.Distinct().Count();
        }

        private static double CalculatePriceByRepeatedlyApplyingBiggestDiscount(ICollection<char> books)
        {
            double total = 0;
            for (;;)
            {
                var sizeOfLargestDistinctSetOfBooks = GetSizeOfLargestDistinctSetOfBooks(books);
                var distinctSetOfBooks = ExtractDistinctSetOfBooks(books, sizeOfLargestDistinctSetOfBooks);
                var numDifferentBooks = distinctSetOfBooks.Count();
                if (numDifferentBooks == 0) break;
                var percentDiscount = NumDifferentBooks2PercentDiscount[numDifferentBooks];
                var subTotal = (UnitBookPrice * numDifferentBooks).PercentOff(percentDiscount);
                total += subTotal;
            }
            return total;
        }

        private static double CalculatePriceBySomethingMoreCunning(ICollection<char> books)
        {
            return CalculatePriceByRepeatedlyApplyingBiggestDiscount(books);
        }

        private static double CalculatePriceForBooks(string books)
        {
            var prices = new List<double>
                {
                    CalculatePriceByRepeatedlyApplyingBiggestDiscount(books.ToCharArray().ToList()),
                    CalculatePriceBySomethingMoreCunning(books.ToCharArray().ToList())
                };
            return prices.Min();
        }
    }

    internal static class DoubleExtensions
    {
        public static double PercentOff(this double d, int p)
        {
            return d - (d * p / 100);
        }
    }
}
