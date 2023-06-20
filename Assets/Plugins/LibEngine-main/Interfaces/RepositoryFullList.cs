using System;
using System.Collections.Generic;
using System.Linq;
using LibEngine.Repository;

namespace LibEngine
{
    public class RepositoryFullList<T> : IRepositoryFull<T>
    {
        public List<T> collection;

        public RepositoryFullList()
        {
            collection = new List<T>();
        }

        public ICollection<T> GetItems()
        {
            return collection;
        }

        public void AddItem(T item)
        {
            collection.Add(item);
        }

        public void RemoveItem(T item)
        {
            collection.Remove(item);
        }

        public T GetItemByExpression(Func<T, bool> expressionPredicate)
        {
            return collection.FirstOrDefault(expressionPredicate);
        }

        public ICollection<T> GetItemsByExpression(Func<T, bool> expressionPredicate)
        {
            return collection.Where(expressionPredicate).ToList();
        }

        public void AddItems(ICollection<T> _items)
        {
            collection.AddRange(_items);
        }
    }
}
