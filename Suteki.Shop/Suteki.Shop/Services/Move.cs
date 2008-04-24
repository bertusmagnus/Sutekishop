using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using Suteki.Shop.Extensions;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class Move : IMoveItems, IMoveDirection
    {
        private IQueryable<IOrderable> items;
        private int position;

        public static IMoveItems ItemAt(int position)
        {
            Move move = new Move { position = position };
            return move;
        }

        public IMoveDirection In(IQueryable<IOrderable> items)
        {
            this.items = items;
            return this;
        }

        public void UpOne()
        {
            MoveInDirection(-1);
        }

        public void DownOne()
        {
            MoveInDirection(1);
        }

        private void MoveInDirection(int direction)
        {
            IOrderable swapItem = items.AtPosition(position + direction);
            
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

    public interface IMoveItems
    {
        IMoveDirection In(IQueryable<IOrderable> items);
    }

    public interface IMoveDirection
    {
        void UpOne();
        void DownOne();
    }

}
