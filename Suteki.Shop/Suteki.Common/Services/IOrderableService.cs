using System;
namespace Suteki.Common.Services
{
    public interface IOrderableService<T>
     where T : class, Suteki.Common.IOrderable
    {
        IOrderServiceWithPosition<T> MoveItemAtPosition(int postion);
        int NextPosition { get; }
    }
}
