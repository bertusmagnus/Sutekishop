using System;
using System.Linq;
using System.Collections.Generic;
using Suteki.Shop.Repositories;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Services
{
    public class StockService : IStockService
    {
        IRepository<Size> sizeRepository;

        public StockService(IRepository<Size> sizeRepository)
        {
            this.sizeRepository = sizeRepository;
        }

        public IEnumerable<StockItem> GetAll()
        {
            return sizeRepository.GetAll().Active().Select(size => new StockItem(size));
        }

        public void Update(IEnumerable<StockItem> stockItems)
        {
            var sizes = sizeRepository.GetAll().Active();

            foreach (Size size in sizes)
            {
                StockItem stockItem = stockItems.SingleOrDefault(item => item.SizeId == size.SizeId);
                if (stockItem != null)
                {
                    size.IsInStock = stockItem.IsInStock;
                }
            }

            sizeRepository.SubmitChanges();
        }
    }
}
