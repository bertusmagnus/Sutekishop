using System;
using System.Collections.Generic;

namespace Suteki.Common.ViewData
{
    public class ScaffoldViewData<T> : IMessageViewData, IErrorViewData
    {
        public string ErrorMessage { get; set; }
        public string Message { get; set; }

        public IEnumerable<T> Items { get; set; }
        public T Item { get; set; }

        public ScaffoldViewData<T> With(T item)
        {
            this.Item = item;
            return this;
        }

        public ScaffoldViewData<T> With(IEnumerable<T> items)
        {
            this.Items = items;
            return this;
        }

        public ScaffoldViewData<T> WithErrorMessage(string errorMessage)
        {
            this.ErrorMessage = errorMessage;
            return this;
        }

        public ScaffoldViewData<T> WithMessage(string message)
        {
            this.Message = message;
            return this;
        }
    }

    public static class Scaffold
    {
        public static ScaffoldViewData<T> Data<T>()
        {
            return new ScaffoldViewData<T>();
        }
    }
}
