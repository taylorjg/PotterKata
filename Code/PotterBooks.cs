using System.Collections.Generic;
using System.Diagnostics;
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

        internal static double CalculateSubTotalForSetOfDifferentBooks(IEnumerable<char> setOfBooks)
        {
            var setOfBooksAsList = setOfBooks.ToList();
            Debug.Assert(setOfBooksAsList.Count() == setOfBooksAsList.Distinct().Count());
            var numDifferentBooks = setOfBooksAsList.Count();
            var percentDiscount = NumDifferentBooks2PercentDiscount[numDifferentBooks];
            var subTotal = (UnitBookPrice * numDifferentBooks).PercentOff(percentDiscount);
            return subTotal;
        }

        private static double CalculatePriceByConsideringCombinations(IEnumerable<char> books)
        {
            var bookCalculations = new List<BookCalculation> { new BookCalculation(books) };

            for (; ; )
            {
                var done = true;

                var bookCalculationsToRemove = new List<BookCalculation>();
                var bookCalculationsToAdd = new List<BookCalculation>();

                foreach (var bookCalculation in bookCalculations)
                {
                    var newBookCalculations = bookCalculation.FindSingleDiscountCombinations();
                    if (newBookCalculations.Any())
                    {
                        bookCalculationsToRemove.Add(bookCalculation);
                        bookCalculationsToAdd.AddRange(newBookCalculations);
                        done = false;
                    }
                }

                bookCalculations.RemoveRange(bookCalculationsToRemove);
                bookCalculations.AddRange(bookCalculationsToAdd);

                if (done) break;
            }

            var bookCalculationWithTheSmallestTotal = bookCalculations.MinBy(x => x.Total);
            return bookCalculationWithTheSmallestTotal.Total;
        }

        public static double CalculatePriceFor(string books)
        {
            return CalculatePriceByConsideringCombinations(books.ToCharArray());
        }
    }
}
