using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class Thing
    {
        private readonly IList<Tuple<string, double>> _subTotals;
        private readonly ICollection<char> _remainingItems;

        public Thing(IEnumerable<char> remainingBooks)
        {
            _remainingItems = new List<char>(remainingBooks);
            _subTotals = new List<Tuple<string, double>>();
        }

        private Thing(IEnumerable<char> remainingBooks, IEnumerable<Tuple<string, double>> items)
        {
            _remainingItems = new List<char>(remainingBooks);
            _subTotals = new List<Tuple<string, double>>(items);
        }

        public IEnumerable<char> RemainingBooks { get { return _remainingItems.AsEnumerable(); } }

        public void AddSubTotal(IEnumerable<char> setOfBooks, double subTotal)
        {
            var setOfBooksAsArray = setOfBooks.ToArray();
            _subTotals.Add(Tuple.Create(new string(setOfBooksAsArray), subTotal));
            foreach (var book in setOfBooksAsArray) _remainingItems.Remove(book);
        }

        public double Total
        {
            get { return _subTotals.Sum(x => x.Item2); }
        }

        public Thing Clone()
        {
            return new Thing(RemainingBooks, _subTotals);
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
            var subTotal = (PotterBooks.UnitBookPrice * numDifferentBooks).PercentOff(percentDiscount);
            return subTotal;
        }

        public IEnumerable<Thing> Step()
        {
            var combinations =
                Enumerable.Range(2, 5)
                          .Aggregate(
                              Enumerable.Empty<IEnumerable<char>>(),
                              (acc, i) =>
                              acc.Concat(Enumerable.Repeat(RemainingBooks, i).Combinations()))
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
                    var newThing = Clone();
                    newThing.AddSubTotal(setOfBooks, subTotal);
                    newThings.Add(newThing);
                }

                return newThings;
            }

            var numBooks = RemainingBooks.Count();
            if (numBooks > 0)
            {
                var subTotal = numBooks * PotterBooks.UnitBookPrice;
                AddSubTotal(RemainingBooks, subTotal);
            }

            return Enumerable.Empty<Thing>();
        }
    }
}
