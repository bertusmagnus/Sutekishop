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

            if (product.DefaultSizeMissing)
            {
                AddDefaultSize(product);
            }

            var keys = form.AllKeys.Where(key => key.StartsWith("size_") && form[key].Length > 0);
            keys.ForEach(key => new Size { Name = form[key], Product = product, IsActive = true, IsInStock = true } );
        }

        private void AddDefaultSize(Product product)
        {
            product.Sizes.Add(new Size { IsActive = false, Name = "-", IsInStock = true });
        }

        public void Clear(Product product)
        {
            product.Sizes.ForEach(size => size.IsActive = false);
        }
    }
}
