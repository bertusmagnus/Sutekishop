using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using Suteki.Shop.Extensions;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class Move<T> : IMoveItems<T>, IMoveDirection where T : IOrderable
    {
        private IQueryable<T> items;
        readonly private int position;

        public static IMoveItems<T> ItemAt(int position)
        {
            Move<T> move = new Move<T>(position);
            return move;
        }

        public Move(int position)
        {
            this.position = position;
        }

        IMoveDirection IMoveItems<T>.In(IQueryable<T> items)
        {
            Move<T> move = new Move<T>(this.position) { items = items };
            return move;
        }

        void IMoveDirection.UpOne()
        {
            SwapPositionWith(items.GetItemBefore(position));
        }

        void IMoveDirection.DownOne()
        {
            SwapPositionWith(items.GetItemAfter(position));
        }

        private void SwapPositionWith(IOrderable swapItem)
        {
            if (swapItem == null) return;

            IOrderable item = items.AtPosition(position);
            if (item == null)
                throw new ApplicationException("There is no item at position {0}".With(position));

            // swap postions
            int tempPosition = swapItem.Position;
            swapItem.Position = item.Position;
            item.Position = tempPosition;
        }
    }

    public interface IMoveItems<T>
    {
        IMoveDirection In(IQueryable<T> items);
    }

    public interface IMoveDirection
    {
        void UpOne();
        void DownOne();
    }

}
