using System;
using System.Collections.Generic;

namespace LibEngine.Repository
{
    public interface IRepository<T>
    {
        public ICollection<T> GetItems();
    }

    public interface IAddItem<T>
    {
        public void AddItem(T _item);
    }

    public interface IAddItems<T>
    {
        public void AddItems(ICollection<T> _items);
    }

    public interface IRemoveItem<T>
    {
        public void RemoveItem(T _item);
    }

    public interface IRepositoryGettersExpression<T>
    {
        public T GetItemByExpression(Func<T, bool> _expressionPredicate);
        public ICollection<T> GetItemsByExpression(Func<T, bool> _expressionPredicate);
    }

    public interface IRepositoryGetters<T> : IRepositoryGettersExpression<T>
    {

    }

    public interface IRepositoryFull<T> : IRepository<T>, IRepositoryGetters<T>, IAddItem<T>, IRemoveItem<T>, IAddItems<T>
    {

    }
}
