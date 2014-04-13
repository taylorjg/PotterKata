using System;
using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public class BookCalculation
    {
        private readonly IEnumerable<char> _remainingBooks;
        private readonly IList<Tuple<string, double>> _subTotals;

        public BookCalculation(IEnumerable<char> remainingBooks)
        {
            _remainingBooks = remainingBooks;
            _subTotals = new List<Tuple<string, double>>();
        }

        private BookCalculation(IEnumerable<char> remainingBooks, IEnumerable<Tuple<string, double>> subTotals)
        {
            _remainingBooks = remainingBooks;
            _subTotals = new List<Tuple<string, double>>(subTotals);
        }

        public double Total
        {
            get { return _subTotals.Sum(x => x.Item2); }
        }

        public bool IsDone
        {
            get { return !_remainingBooks.Any(); }
        }

        public BookCalculation Clone(IEnumerable<char> remainingBooks)
        {
            return new BookCalculation(remainingBooks, _subTotals);
        }

        private void AddSubTotal(IEnumerable<char> setOfBooks, double subTotal)
        {
            _subTotals.Add(Tuple.Create(new string(setOfBooks.ToArray()), subTotal));
        }

        public IList<BookCalculation> FindSingleDiscountCombinations()
        {
            var newBookCalculations = new List<BookCalculation>();

            var combinations =
                Enumerable.Range(2, 5)
                          .Aggregate(
                              Enumerable.Empty<IEnumerable<char>>(),
                              (acc, i) => acc.Concat(Enumerable
                                  .Repeat(_remainingBooks, i)
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

                foreach (var setOfBooks in combinations.Select(x => x.ToList()))
                {
                    var newBookCalculation = CreateNewBookCalculation(setOfBooks, PotterBooks.CalculateSubTotalForSetOfDifferentBooks);
                    newBookCalculations.Add(newBookCalculation);
                }
            }
            else
            {
                var setOfBooks = _remainingBooks.ToList();
                var newBookCalculation = CreateNewBookCalculation(setOfBooks, PotterBooks.CalculateSubTotalForSetOfSameBooks);
                newBookCalculations.Add(newBookCalculation);
            }

            return newBookCalculations;
        }

        private BookCalculation CreateNewBookCalculation(IList<char> setOfBooks, Func<IEnumerable<char>, double> func)
        {
            var newRemainingBooks = new List<char>(_remainingBooks);
            newRemainingBooks.RemoveRange(setOfBooks);
            var subTotal = func(setOfBooks);
            var newBookCalculation = Clone(newRemainingBooks);
            newBookCalculation.AddSubTotal(setOfBooks, subTotal);
            return newBookCalculation;
        }
    }
}
