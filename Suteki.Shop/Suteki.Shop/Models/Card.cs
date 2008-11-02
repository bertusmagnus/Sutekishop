using System;
using System.Collections.Generic;
using Suteki.Common.Extensions;
using Suteki.Common.Validation;

namespace Suteki.Shop
{
    public partial class Card
    {
        public static IEnumerable<int> Months
        {
            get
            {
                return 1.To(12);
            }
        }

        public static IEnumerable<int> ExpiryYears
        {
            get
            {
                return DateTime.Now.Year.To(DateTime.Now.Year + 8);
            }
        }

        public static IEnumerable<int> StartYears
        {
            get
            {
                return (DateTime.Now.Year - 4).To(DateTime.Now.Year);
            }
        }

        public Card Copy()
        {
            return new Card
            {
                _CardTypeId = this._CardTypeId,
                _Holder = this._Holder,
                _Number = this._Number,
                _IssueNumber = this._IssueNumber,
                _SecurityCode = this._SecurityCode,
                _StartMonth = this._StartMonth,
                _StartYear = this._StartYear,
                _ExpiryMonth = this._ExpiryMonth,
                _ExpiryYear = this._ExpiryYear
            };
        }

        // validation

        partial void OnHolderChanging(string value)
        {
            value.Label("Card Holder").IsRequired().WithMaxLength(50);
        }

        partial void OnNumberChanging(string value)
        {
            value.Label("Card Number").IsRequired().IsCreditCard();
        }

        partial void OnIssueNumberChanging(string value)
        {
            value.Label("Issue Number").IsNumeric().WithMaxLength(1);
        }

        partial void OnSecurityCodeChanging(string value)
        {
            value.Label("Security Code").IsRequired().IsNumeric().WithMaxLength(4);
        }

        // encrypted value setters

        public void SetEncryptedNumber(string number)
        {
            this._Number = number;
        }

        public void SetEncryptedSecurityCode(string securityCode)
        {
            this._SecurityCode = securityCode;
        }

        // computed properties

        public string StartDateAsString
        {
            get
            {
                if(StartMonth == 0 || StartYear == 0)
                {
                    return "&nbsp;";
                }
                return "{0:00} / {1:0000}".With(StartMonth, StartYear);
            }
        }

        public string ExpiryDateAsString
        {
            get
            {
                return "{0:00} / {1:0000}".With(ExpiryMonth, ExpiryYear);
            }
        }
    }
}
