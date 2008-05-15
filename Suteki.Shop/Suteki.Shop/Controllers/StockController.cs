using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.Services;
using Suteki.Shop.ViewData;
using System.Collections.Specialized;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Controllers
{
    public class StockController : ControllerBase
    {
        IStockService stockService;

        public StockController(IStockService stockService)
        {
            this.stockService = stockService;
        }

        public ActionResult Index()
        {
            var stockItems = stockService.GetAll();
            return RenderView("Index", View.Data.WithStockItems(stockItems));
        }

        public ActionResult Update()
        {
            var stockItems = stockService.GetAll().ToList();
            UpdateFromForm(stockItems, Form);
            stockService.Update(stockItems);

            return RenderView("Index", View.Data.WithStockItems(stockItems));
        }

        private void UpdateFromForm(IEnumerable<StockItem> stockItems, NameValueCollection form)
        {
            foreach (StockItem stockItem in stockItems)
            {
                stockItem.IsInStock = form["stockitem_{0}".With(stockItem.SizeId)] != null;
            }
        }
    }
}
