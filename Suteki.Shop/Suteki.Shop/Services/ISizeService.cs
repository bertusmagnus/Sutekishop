using System;

namespace Suteki.Shop.Services
{
    public interface ISizeService
    {
        ISizeService WithVaues(System.Collections.Specialized.NameValueCollection form);
        void Update(Product product);
    }
}
