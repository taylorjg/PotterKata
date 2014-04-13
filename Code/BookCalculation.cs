using System;
using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public class BookCalculation
    {
        private readonly IList<char> _remainingItems;
        private readonly IList<Tuple<string, double>> _subTotals;

        public BookCalculation(IEnumerable<char> remainingBooks)
        {
            _remainingItems = new List<char>(remainingBooks);
            _subTotals = new List<Tuple<string, double>>();
        }

        private BookCalculation(IEnumerable<char> remainingBooks, IEnumerable<Tuple<string, double>> items)
        {
            _remainingItems = new List<char>(remainingBooks);
            _subTotals = new List<Tuple<string, double>>(items);
        }

        public double Total
        {
            get { return _subTotals.Sum(x => x.Item2); }
        }

        public bool IsDone
        {
            get { return !_remainingItems.Any(); }
        }

        public BookCalculation Clone()
        {
            return new BookCalculation(_remainingItems, _subTotals);
        }

        private void AddSubTotal(IEnumerable<char> setOfBooks, double subTotal)
        {
            var setOfBooksAsArray = setOfBooks.ToArray();
            _subTotals.Add(Tuple.Create(new string(setOfBooksAsArray), subTotal));
            _remainingItems.RemoveRange(setOfBooksAsArray);
        }

        public IList<BookCalculation> FindSingleDiscountCombinations()
        {
            var combinations =
                Enumerable.Range(2, 5)
                          .Aggregate(
                              Enumerable.Empty<IEnumerable<char>>(),
                              (acc, i) => acc.Concat(Enumerable
                                  .Repeat(_remainingItems, i)
                                  .Combinations()))
                          .ToList();

            if (combinations.Any())
            {
                // Optimisation - if we have any combinations consisting of 4 or more books
                // then don't bother considering smaller combinations.
                if (combinations.Any(x => x.Count() >= 4))
                {
                    combinations = combinations.Where(x => x.Count() >= 4).ToList();
                }

                var newBookCalculations = new List<BookCalculation>();

                foreach (var setOfBooks in combinations.Select(x => x.ToList()))
                {
                    var subTotal = PotterBooks.CalculateSubTotalForSetOfDifferentBooks(setOfBooks);
                    var newBookCalculation = Clone();
                    newBookCalculation.AddSubTotal(setOfBooks, subTotal);
                    newBookCalculations.Add(newBookCalculation);
                }

                return newBookCalculations;
            }

            var numBooks = _remainingItems.Count();
            if (numBooks > 0)
            {
                var subTotal = numBooks * PotterBooks.UnitBookPrice;
                AddSubTotal(_remainingItems, subTotal);
            }

            return new List<BookCalculation> {this};
        }
    }
}
