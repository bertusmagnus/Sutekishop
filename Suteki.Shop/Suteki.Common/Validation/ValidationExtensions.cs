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
        public static ValidationProperty<T> Label<T>(this T value, string label)
        {
            return new ValidationProperty<T>(value, label);
        }

    }


    public class ValidationProperty<T>
    {
        T value;
        string label;

        public ValidationProperty(T value, string label)
        {
            this.value = value;
            this.label = label;
        }

        public bool IsObject
        {
            get { return typeof (object).IsAssignableFrom(typeof (T)); }
        }

        public bool IsString
        {
            get { return (IsObject && typeof (string).IsAssignableFrom(typeof (T))); }
        }

        public string ValueAsString
        {
            get { return value as string; }
        }

        public T Value
        {
            get { return value; }
        }

        public ValidationProperty<T> IsRequired()
        {
            if (value == null)
            {
                throw new ValidationException(label, StringExtensions.With("You must enter a value for {0}", label));
            }
            if (IsString && string.IsNullOrEmpty(ValueAsString))
            {
                throw new ValidationException(label, StringExtensions.With("You must enter a value for {0}", label));
            }

           return this;
        }

        public ValidationProperty<T> IsNumeric()
        {
            if(IsString)
            {
                // the obvious thing (int.Parse) doesn't work for very long strings of digits
                if (ValueAsString.Trim().Any(c => !char.IsDigit(c)))
                {
                    throw new ValidationException(label,"{0} must be a number e.g. 240".With(label));
                }
            }

            return this;
        }

        public ValidationProperty<T> IsDecimal()
        {
            if (IsString && ValueAsString.Trim().Any(c => !(char.IsDigit(c) || c == '.')))
            {
                throw new ValidationException(label,"{0} must be a decimal number e.g 12.30".With(label));
            }
            
            return this;
        }

        public ValidationProperty<T> IsNonZero()
        {
            int test;
            if (!int.TryParse(value.ToString(), out test))
            {
                throw new ValidationException(label,"{0} must be a non-zero number".With(label));
            }
            if (test == 0)
            {
                throw new ValidationException(label,"{0} must be non-zero".With(label));
            }
            return this;
        }

        public ValidationProperty<T> WithMaxLength(int maxLength)
        {
            if (IsString && ValueAsString.Length > maxLength)
            {
                throw new ValidationException(label,"{0} must not exceed {1} characters".With(label, maxLength));
            }
            return this;
        }

        public ValidationProperty<T> WithLengthRange(IEnumerable<int> range)
        {
            if (IsString && ValueAsString.Length < range.Min() || ValueAsString.Length > range.Max())
            {
                throw new ValidationException(label,
                    "{0} length must be between {1} and {2} characters".With(label, range.Min(), range.Max()));
            }

            return this;
        }

        public ValidationProperty<T> IsEmail()
        {
            // ignore is null or empty, use IsRequired in parrallel to check this if needed
            if (!IsString) return this;

            if (string.IsNullOrEmpty(ValueAsString)) return this;

            const string patternLenient = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            if (!Regex.Match(ValueAsString, patternLenient).Success)
            {
                throw new ValidationException(label,"{0} must be a valid email address".With(label));
            }

            return this;
        }

        public ValidationProperty<T> IsCreditCard()
        {
            if (IsString)
            {
                var trimmedValue = Regex.Replace(value.ToString(), "[^0-9]", "");
                
                trimmedValue.Label(label).IsNumeric().WithLengthRange(13.To(19));

                var numbers = trimmedValue.Trim().Reverse().Select(c => int.Parse(c.ToString()));

                var oddSum = numbers.AtOddPositions().Sum();
                var doubleEvenSum = numbers.AtEvenPositions().SelectMany(i => new[] { (i * 2) % 10, (i * 2) / 10 }).Sum();

                if ((oddSum + doubleEvenSum) % 10 != 0)
                {
                    throw new ValidationException(label,"{0} is not a valid credit card number".With(label));
                }
            }
            return this;
        }
    }
}