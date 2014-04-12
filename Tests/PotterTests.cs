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

        [TestCase("AABBCCDE", 2 * (4 * UnitBookPrice * 0.80d))] // = ABCD + ABCE
        [TestCase("AAAAABBBBBCCCCDDDDDEEEE", (3 * (5 * UnitBookPrice * 0.75d)) + (2 * (4 * UnitBookPrice * 0.80d)))] // = ABCDE + ABCDE + ABCDE + ABCD + ABDE
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

        private static double CalculateSubTotalFor(IEnumerable<char> setOfBooks)
        {
            var numDifferentBooks = setOfBooks.Count();
            var percentDiscount = NumDifferentBooks2PercentDiscount[numDifferentBooks];
            var subTotal = (UnitBookPrice * numDifferentBooks).PercentOff(percentDiscount);
            return subTotal;
        }

        private static IEnumerable<Thing> ThingStep(Thing thing)
        {
            var combinations =
                Enumerable.Range(2, 5)
                          .Aggregate(
                              Enumerable.Empty<IEnumerable<char>>(),
                              (acc, i) =>
                              acc.Concat(Enumerable.Repeat(thing.RemainingBooks, i).Combinations()))
                          .ToList();

            if (combinations.Any())
            {
                if (combinations.Any(x => x.Count() >= 4))
                {
                    combinations = combinations.Where(x => x.Count() >= 4).ToList();
                }
                var newThings = new List<Thing>();
                foreach (var setOfBooks in combinations.Select(x => x.ToList()))
                {
                    var subTotal = CalculateSubTotalFor(setOfBooks);
                    var newThing = thing.Clone();
                    newThing.AddSubTotal(setOfBooks, subTotal);
                    newThings.Add(newThing);
                }
                return newThings;
            }

            var numBooks = thing.RemainingBooks.Count();
            if (numBooks > 0)
            {
                var subTotal = numBooks * UnitBookPrice;
                thing.AddSubTotal(thing.RemainingBooks, subTotal);
            }

            return Enumerable.Empty<Thing>();
        }

        private static double CalculatePriceByConsideringCombinations(IEnumerable<char> books)
        {
            var things = new List<Thing> {new Thing(books)};

            for (;;)
            {
                var keepGoing = false;
                var thingsToRemove = new List<Thing>();
                var overallNewThings = new List<Thing>();
                foreach (var thing in things)
                {
                    var newThings = ThingStep(thing).ToList();
                    if (newThings.Any())
                    {
                        overallNewThings.AddRange(newThings);
                        thingsToRemove.Add(thing);
                        keepGoing = true;
                    }
                }
                foreach (var thing in thingsToRemove) things.Remove(thing);
                things.AddRange(overallNewThings);
                if (!keepGoing) break;
            }

            var thingWithTheSmallestTotal = things.MinBy(x => x.Total);
            return thingWithTheSmallestTotal.Total;
        }

        private static double CalculatePriceForBooks(string books)
        {
            return CalculatePriceByConsideringCombinations(books.ToCharArray());
        }
    }
}
