using System;
namespace Suteki.Shop.Services
{
    public interface IOrderableService<T>
     where T : class, Suteki.Shop.IOrderable
    {
        IOrderServiceWithPosition<T> MoveItemAtPosition(int postion);
        int NextPosition { get; }
    }
}
