using System;

namespace Suteki.Shop.Validation
{
    public static class ValidationExtensions
    {
        public static ValidationProperty Label(this string value, string label)
        {
            return new ValidationProperty(value, label);
        }
    }

    public class ValidationProperty
    {
        string value;
        string label;

        public ValidationProperty(string value, string label)
        {
            this.value = value;
            this.label = label;
        }

        public ValidationProperty IsRequired()
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ValidationException(string.Format("You must enter a value for {0}", label));
            }

            return this;
        }
    }
}
