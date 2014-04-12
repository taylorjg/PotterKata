using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class Thing
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
}
