using System;
using System.Collections.Generic;
using LibEngine.Repository;

namespace LibEngine
{
    public class BaseSomeRepositoryDecoratorService<T> : IRepositoryFull<T>
    {
        private readonly IRepositoryFull<T> repository;

        public BaseSomeRepositoryDecoratorService(IRepositoryFull<T> repository)
        {
            this.repository = repository;
        }

        public ICollection<T> GetItems()
        {
            return repository.GetItems();
        }

        public void AddItem(T item)
        {
            repository.AddItem(item);
        }

        public void RemoveItem(T item)
        {
            repository.RemoveItem(item);
        }

        public T GetItemByExpression(Func<T, bool> expressionPredicate)
        {
            return repository.GetItemByExpression(expressionPredicate);
        }

        public ICollection<T> GetItemsByExpression(Func<T, bool> expressionPredicate)
        {
            return repository.GetItemsByExpression(expressionPredicate);
        }

        public void AddItems(ICollection<T> _items)
        {
            repository.AddItems(_items);
        }
    }
}
