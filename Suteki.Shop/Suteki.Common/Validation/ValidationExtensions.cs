using System;
using System.Linq;
using Suteki.Common.Extensions;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Suteki.Common.Validation;
using EnumerableExtensions=Suteki.Common.Extensions.EnumerableExtensions;

namespace Suteki.Common.Validation
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
                throw new ValidationException(StringExtensions.With("You must enter a value for {0}", label));
            }

            return this;
        }

        public ValidationProperty IsNumeric()
        {
            if (value.Trim().Any(c => !char.IsDigit(c)))
            {
                throw new ValidationException("{0} must be a number".With(label));
            }

            return this;
        }

        public ValidationProperty IsDecimal()
        {
            if (value.Trim().Any(c => !(char.IsDigit(c) || c == '.')))
            {
                throw new ValidationException("{0} must be a decimal number e.g 12.30".With(label));
            }

            return this;
        }

        public ValidationProperty WithMaxLength(int maxLength)
        {
            if (value.Length > maxLength)
            {
                throw new ValidationException("{0} must not exceed {1} characters".With(label, maxLength));
            }

            return this;
        }

        public ValidationProperty WithLengthRange(IEnumerable<int> range)
        {
            if (value.Length < range.Min() || value.Length > range.Max())
            {
                throw new ValidationException(
                    "{0} length must be between {1} and {2} characters".With(label, range.Min(), range.Max()));
            }

            return this;
        }

        public ValidationProperty IsEmail()
        {
            // ignore is null or empty, use IsRequired in parrallel to check this if needed
            if (string.IsNullOrEmpty(value)) return this;

            string patternLenient = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            if (!Regex.Match(value, patternLenient).Success)
            {
                throw new ValidationException("{0} must be a valid email address".With(label));
            }

            return this;
        }

        public ValidationProperty IsCreditCard()
        {
            var trimmedValue = Regex.Replace(value, "[^0-9]", "");

            trimmedValue.Label(label).IsNumeric().WithLengthRange(13.To(18));

            var numbers = trimmedValue.Trim().Reverse().Select(c => int.Parse(c.ToString()));

            int oddSum = numbers.AtOddPositions().Sum();
            int doubleEvenSum = numbers.AtEvenPositions().SelectMany(i => new int[] { (i * 2) % 10, (i * 2) / 10 }).Sum();

            if ((oddSum + doubleEvenSum) % 10 != 0)
            {
                throw new ValidationException("{0} is not a valid credit card number".With(label));
            }

            return this;
        }
    }
}