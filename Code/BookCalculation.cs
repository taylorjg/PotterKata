﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Code
{
    public class BookCalculation
    {
        private readonly IEnumerable<char> _remainingBooks;
        private readonly IEnumerable<Tuple<string, double>> _subTotals;

        public BookCalculation(IEnumerable<char> remainingBooks)
            : this(remainingBooks, Enumerable.Empty<Tuple<string, double>>())
        {
        }

        private BookCalculation(IEnumerable<char> remainingBooks, IEnumerable<Tuple<string, double>> subTotals)
        {
            _remainingBooks = remainingBooks;
            _subTotals = subTotals;
        }

        public double Total
        {
            get { return _subTotals.Sum(x => x.Item2); }
        }

        public bool HasRemainingBooks
        {
            get { return _remainingBooks.Any(); }
        }

        public IList<BookCalculation> FindCombinationsOfNextSetOfBooks()
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

                foreach (var setOfBooks in combinations)
                {
                    newBookCalculations.Add(CreateNewBookCalculation(setOfBooks, PotterBooks.CalculateSubTotalForSetOfDifferentBooks));
                }
            }
            else
            {
                newBookCalculations.Add(CreateNewBookCalculation(_remainingBooks, PotterBooks.CalculateSubTotalForSetOfSameBooks));
            }

            return newBookCalculations;
        }

        private BookCalculation CreateNewBookCalculation(IEnumerable<char> setOfBooks, Func<IEnumerable<char>, double> calculateSubTotalForSetOfBooks)
        {
            var setOfBooksList = setOfBooks.ToList();
            var subTotalValue = calculateSubTotalForSetOfBooks(setOfBooksList);
            return CloneWithAdditionalSubTotal(setOfBooksList, subTotalValue);
        }

        private BookCalculation CloneWithAdditionalSubTotal(IList<char> setOfBooks, double subTotalValue)
        {
            var newRemainingBooks = new List<char>(_remainingBooks);
            newRemainingBooks.RemoveRange(setOfBooks);
            var newSubTotal = CreateSubTotal(setOfBooks, subTotalValue);
            return new BookCalculation(newRemainingBooks, _subTotals.Concat(new[] { newSubTotal }));
        }

        private static Tuple<string, double> CreateSubTotal(IEnumerable<char> setOfBooks, double subTotalValue)
        {
            var setOfBooksString = new string(setOfBooks.ToArray());
            return Tuple.Create(setOfBooksString, subTotalValue);
        }
    }
}
