using System;
using System.Collections.Generic;

namespace Suteki.Shop.Services
{
    public interface IStockService
    {
        IEnumerable<StockItem> GetAll();

        void Update(IEnumerable<StockItem> stockItems);
    }
}
