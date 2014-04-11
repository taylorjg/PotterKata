using System.Linq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class PotterTests
    {
        [Test]
        public void NoBooksCostZero()
        {
            var price = CalculatePriceForBooks(string.Empty);
            Assert.That(price, Is.EqualTo(0));
        }

        [TestCase("A")]
        [TestCase("B")]
        [TestCase("C")]
        [TestCase("D")]
        [TestCase("E")]
        public void ASingleBookIsPricedCorrectly(string books)
        {
            var price = CalculatePriceForBooks(books);
            Assert.That(price, Is.EqualTo(8));
        }

        [TestCase("AA", 2 * 8)]
        [TestCase("AAA", 3 * 8)]
        [TestCase("AAAA", 4 * 8)]
        [TestCase("AAAAA", 5 * 8)]
        [TestCase("BB", 2 * 8)]
        [TestCase("BBB", 3 * 8)]
        [TestCase("BBBB", 4 * 8)]
        [TestCase("BBBBB", 5 * 8)]
        [TestCase("AAAAAA", 6 * 8)]
        [TestCase("AAAAAAA", 7 * 8)]
        [TestCase("AAAAAAAA", 8 * 8)]
        [TestCase("AAAAAAAAA", 9 * 8)]
        [TestCase("AAAAAAAAAA", 10 * 8)]
        public void MultipleBooksOfTheSameTypeArePricedCorrectly(string books, double expectedPrice)
        {
            var price = CalculatePriceForBooks(books);
            Assert.That(price, Is.EqualTo(expectedPrice));
        }

        [TestCase("AB", 2 * 8 * 0.95)]
        [TestCase("ABC", 3 * 8 * 0.90)]
        [TestCase("ABCD", 4 * 8 * 0.80)]
        [TestCase("ABCDE", 5 * 8 * 0.75)]
        public void MultipleBooksOfDifferentTypesArePricedCorrectly(string books, double expectedPrice)
        {
            var price = CalculatePriceForBooks(books);
            Assert.That(price, Is.EqualTo(expectedPrice));
        }

        private static double CalculatePriceForBooks(string books)
        {
            var numBooks = books.Length;

            if (books.ToCharArray().Distinct().Count() == numBooks)
            {
                switch (numBooks)
                {
                    case 0:
                        return 0;
                    case 1:
                        return 1 * 8;
                    case 2:
                        return 2 * 8 * 0.95;
                    case 3:
                        return 3 * 8 * 0.90;
                    case 4:
                        return 4 * 8 * 0.80;
                    case 5:
                        return 5 * 8 * 0.75;
                }
            }

            return numBooks * 8;
        }
    }
}
