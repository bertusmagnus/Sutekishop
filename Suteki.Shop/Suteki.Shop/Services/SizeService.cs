using System;
using System.Linq;
using System.Collections.Specialized;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;

namespace Suteki.Shop.Services
{
    public class SizeService : ISizeService
    {
        IRepository<Size> sizeRepository;
        NameValueCollection form;

        public SizeService(IRepository<Size> sizeRepository)
        {
            this.sizeRepository = sizeRepository;
        }

        public ISizeService WithValues(NameValueCollection form)
        {
            this.form = form;
            return this;
        }

        public void Update(Product product)
        {
            if (form == null) throw new ApplicationException("form must be set with 'WithValues' before calling Update");
            var keys = form.AllKeys.Where(key => key.StartsWith("size_") && form[key].Length > 0);
//            if (keys.Count() > 0)
//            {
//                Clear(product);
//            }
            keys.ForEach(key => new Size { Name = form[key], Product = product, IsActive = true, IsInStock = true } );
        }

        public void Clear(Product product)
        {
            product.Sizes.ForEach(size => size.IsActive = false);
        }
    }
}
