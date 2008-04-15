using System;

namespace Suteki.Shop.ViewData
{
    public class CommonViewData : IErrorViewData, IMessageViewData
    {
        public string ErrorMessage { get; set; }
        public string Message { get; set; }
    }
}
