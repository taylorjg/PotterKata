using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class Thing
    {
        private readonly IList<Tuple<string, double>> _items;
        private readonly ICollection<char> _remainingItems;

        public Thing(IEnumerable<char> remainingBooks)
        {
            _remainingItems = new List<char>(remainingBooks);
            _items = new List<Tuple<string, double>>();
        }

        private Thing(IEnumerable<char> remainingBooks, IEnumerable<Tuple<string, double>> items)
        {
            _remainingItems = new List<char>(remainingBooks);
            _items = new List<Tuple<string, double>>(items);
        }

        public IEnumerable<char> RemainingBooks { get { return _remainingItems.AsEnumerable(); } }

        public void AddSubTotal(IEnumerable<char> setOfBooks, double subTotal)
        {
            var setOfBooksAsArray = setOfBooks.ToArray();
            _items.Add(Tuple.Create(new string(setOfBooksAsArray), subTotal));
            foreach (var book in setOfBooksAsArray) _remainingItems.Remove(book);
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
}
