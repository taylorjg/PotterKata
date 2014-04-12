using System;
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

        private static double CalculateSubTotalForSetOfBooks(ICollection<char> books, IList<char> setOfBooks)
        {
            var numDifferentBooks = setOfBooks.Count();
            var percentDiscount = NumDifferentBooks2PercentDiscount[numDifferentBooks];
            var subTotal = (UnitBookPrice * numDifferentBooks).PercentOff(percentDiscount);
            foreach (var book in setOfBooks) books.Remove(book);
            return subTotal;
        }

        private class Thing
        {
            private readonly IList<Tuple<string, double>> _items;

            public Thing(IEnumerable<char> remainingBooks)
            {
                RemainingBooks = new List<char>(remainingBooks);
                _items = new List<Tuple<string, double>>();
            }

            private Thing(IEnumerable<char> remainingBooks, IEnumerable<Tuple<string, double>> items)
            {
                RemainingBooks = new List<char>(remainingBooks);
                _items = new List<Tuple<string, double>>(items);
            }

            public IList<char> RemainingBooks { get; private set; }

            public void AddItem(IEnumerable<char> setOfBooks, double subTotal)
            {
                _items.Add(Tuple.Create(new string(setOfBooks.ToArray()), subTotal));
            }

            public double Total
            {
                get { return _items.Sum(x => x.Item2); }
            }

            public Thing Clone()
            {
                return new Thing(RemainingBooks, _items);
            }
        }

        private static IEnumerable<Thing> RecursiveStep(Thing thing)
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
                var newThings = new List<Thing>();
                foreach (var combination in combinations.Select(x => x.ToList()))
                {
                    var newThing = thing.Clone();
                    var subTotal = CalculateSubTotalForSetOfBooks(newThing.RemainingBooks, combination);
                    newThing.AddItem(combination, subTotal);
                    newThings.Add(newThing);
                }
                return newThings;
            }

            {
                var subTotal = thing.RemainingBooks.Count() * UnitBookPrice;
                thing.AddItem(thing.RemainingBooks, subTotal);
                thing.RemainingBooks.Clear();
                return Enumerable.Empty<Thing>();
            }
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
                    var newThings = RecursiveStep(thing).ToList();
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

            var calculationWithTheSmallestTotal = things.First();
            var smallestTotal = things.First().Total;

            foreach (var thing in things)
            {
                if (thing.Total < smallestTotal)
                {
                    smallestTotal = thing.Total;
                    calculationWithTheSmallestTotal = thing;
                }
            }

            return calculationWithTheSmallestTotal.Total;
        }

        private static double CalculatePriceForBooks(string books)
        {
            return CalculatePriceByConsideringCombinations(books.ToCharArray());
        }
    }

    internal static class DoubleExtensions
    {
        public static double PercentOff(this double d, int p)
        {
            return d - (d * p / 100);
        }
    }

    internal static class CombinationsFromStackOverflow
    {
        // http://stackoverflow.com/questions/5980810/generate-all-unique-combinations-of-elements-of-a-ienumerableof-t
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                from accseq in accumulator
                // Exclude items that were already picked
                from item in sequence.Except(accseq)
                // Enforce ascending order to avoid same sequence in different order
                where !accseq.Any() || Comparer<T>.Default.Compare(item, accseq.Last()) > 0
                select accseq.Concat(new[] { item })).ToArray();
        }
    }
}
