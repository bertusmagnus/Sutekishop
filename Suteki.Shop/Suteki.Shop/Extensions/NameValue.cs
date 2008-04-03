using System;

namespace Suteki.Shop.Extensions
{
    /// <summary>
    /// A name value pair with the useful addition that it doesn't work out the value until it's needed
    /// 
    /// Example:
    /// return new NameValue&lt;string, object&gt;("Name", () =&gt; myCustomer.Name);
    /// </summary>
    /// <typeparam name="TName"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class NameValue<TName, TValue>
    {
        public TName Name { get; private set; }
        public TValue Value { get { return valueFunction(); } }

        Func<TValue> valueFunction;

        /// <summary>
        /// Example:
        /// return new NameValue&lt;string, object&gt;("Name", () =&gt; myCustomer.Name);
        /// </summary>
        /// <param name="name"></param>
        /// <param name="valueFunction"></param>
        public NameValue(TName name, Func<TValue> valueFunction)
        {
            Name = name;
            this.valueFunction = valueFunction;
        }
    }
}
