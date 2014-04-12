using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public static class PotterBooks
    {
        public const double UnitBookPrice = 8d;

        private static double CalculatePriceByConsideringCombinations(IEnumerable<char> books)
        {
            var things = new List<Thing> { new Thing(books) };

            for (; ; )
            {
                var keepGoing = false;
                var thingsToRemove = new List<Thing>();
                var overallNewThings = new List<Thing>();
                foreach (var thing in things)
                {
                    var newThings = thing.Step().ToList();
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

        public static double CalculatePriceForBooks(string books)
        {
            return CalculatePriceByConsideringCombinations(books.ToCharArray());
        }
    }
}
