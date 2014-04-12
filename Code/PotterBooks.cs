using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public static class PotterBooks
    {
        public const double UnitBookPrice = 8d;

        private readonly static IDictionary<int, int> NumDifferentBooks2PercentDiscount = new Dictionary<int, int>
            {
                {0, 0},
                {1, 0},
                {2, 5},
                {3, 10},
                {4, 20},
                {5, 25}
            };

        public static double CalculateSubTotalFor(IEnumerable<char> setOfBooks)
        {
            var numDifferentBooks = setOfBooks.Count();
            var percentDiscount = NumDifferentBooks2PercentDiscount[numDifferentBooks];
            var subTotal = (UnitBookPrice * numDifferentBooks).PercentOff(percentDiscount);
            return subTotal;
        }

        private static double CalculatePriceByConsideringCombinations(IEnumerable<char> books)
        {
            var things = new List<Thing> { new Thing(books) };

            for (; ; )
            {
                var done = true;

                var thingsToRemove = new List<Thing>();
                var thingsToAdd = new List<Thing>();

                foreach (var thing in things)
                {
                    var newThings = thing.Step().ToList();
                    if (newThings.Any())
                    {
                        thingsToAdd.AddRange(newThings);
                        thingsToRemove.Add(thing);
                        done = false;
                    }
                }

                foreach (var thing in thingsToRemove) things.Remove(thing);
                things.AddRange(thingsToAdd);

                if (done) break;
            }

            var thingWithTheSmallestTotal = things.MinBy(x => x.Total);
            return thingWithTheSmallestTotal.Total;
        }

        public static double CalculatePriceFor(string books)
        {
            return CalculatePriceByConsideringCombinations(books.ToCharArray());
        }
    }
}
