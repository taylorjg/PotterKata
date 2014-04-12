using System;
using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public class Thing
    {
        private readonly ICollection<char> _remainingItems;
        private readonly IList<Tuple<string, double>> _subTotals;

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

        public double Total
        {
            get { return _subTotals.Sum(x => x.Item2); }
        }

        public Thing Clone()
        {
            return new Thing(_remainingItems, _subTotals);
        }

        private void AddSubTotal(IEnumerable<char> setOfBooks, double subTotal)
        {
            var setOfBooksAsArray = setOfBooks.ToArray();
            _subTotals.Add(Tuple.Create(new string(setOfBooksAsArray), subTotal));
            foreach (var book in setOfBooksAsArray) _remainingItems.Remove(book);
        }

        public IEnumerable<Thing> Step()
        {
            var combinations =
                Enumerable.Range(2, 5)
                          .Aggregate(
                              Enumerable.Empty<IEnumerable<char>>(),
                              (acc, i) =>
                              acc.Concat(Enumerable.Repeat(_remainingItems, i).Combinations()))
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
                    var subTotal = PotterBooks.CalculateSubTotalFor(setOfBooks);
                    var newThing = Clone();
                    newThing.AddSubTotal(setOfBooks, subTotal);
                    newThings.Add(newThing);
                }

                return newThings;
            }

            var numBooks = _remainingItems.Count();
            if (numBooks > 0)
            {
                var subTotal = numBooks * PotterBooks.UnitBookPrice;
                AddSubTotal(_remainingItems, subTotal);
            }

            return Enumerable.Empty<Thing>();
        }
    }
}
