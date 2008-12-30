using System;
using System.Linq;
using NUnit.Framework;
using Suteki.Common.Validation;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class CardTests
    {
        [Test]
        public void Months_ShouldReturnNumbersOneToTwelve()
        {
            var months = Card.Months;
            Assert.That(months.Count(), Is.EqualTo(12));

            var expectedMonth = 1;
            foreach (var month in months)
            {
                Assert.That(month, Is.EqualTo(expectedMonth));
                expectedMonth++;
            }
        }

        [Test]
        public void ExpiryYears_ShouldReturn8YearsFromThisYear()
        {
            var expiryYears = Card.ExpiryYears;
            Assert.That(expiryYears.Count(), Is.EqualTo(9));

            var expectedYear = DateTime.Now.Year;
            foreach (var expiryYear in expiryYears)
            {
                Assert.That(expiryYear, Is.EqualTo(expectedYear));
                expectedYear++;
            }
        }

        [Test]
        public void StartYears_ShouldReturnThePrevious4Years()
        {
            var startYears = Card.StartYears;
            Assert.That(startYears.Count(), Is.EqualTo(5));

            var expectedYear = DateTime.Now.Year - 4;
            foreach (var startYear in startYears)
            {
                Assert.That(startYear, Is.EqualTo(expectedYear));
                expectedYear++;
            }
        }

        [Test]
        public void Copy_ShouldMakeAShallowCopyOfACard()
        {
            var originalCard = new Card
                                   {
                                       CardTypeId = 3,
                                       Holder = "Mike Hadlow",
                                       Number = "1111 1111 1111 1117",
                                       IssueNumber = "1",
                                       SecurityCode = "123",
                                       StartMonth = 1,
                                       StartYear = DateTime.Now.Year,
                                       ExpiryMonth = 1,
                                       ExpiryYear = DateTime.Now.Year
                                   };

            var copiedCard = originalCard.Copy();

            Assert.That(originalCard.CardTypeId, Is.EqualTo(copiedCard.CardTypeId));
            Assert.That(originalCard.Holder, Is.EqualTo(copiedCard.Holder));
            Assert.That(originalCard.Number, Is.EqualTo(copiedCard.Number));
            Assert.That(originalCard.IssueNumber, Is.EqualTo(copiedCard.IssueNumber));
            Assert.That(originalCard.SecurityCode, Is.EqualTo(copiedCard.SecurityCode));
            Assert.That(originalCard.StartMonth, Is.EqualTo(copiedCard.StartMonth));
            Assert.That(originalCard.StartYear, Is.EqualTo(copiedCard.StartYear));
            Assert.That(originalCard.ExpiryMonth, Is.EqualTo(copiedCard.ExpiryMonth));
            Assert.That(originalCard.ExpiryYear, Is.EqualTo(copiedCard.ExpiryYear));
        }

        [Test]
        public void IssueNumber_ShouldOnlyBeASingleDigit()
        {
            var card = new Card();
            card.IssueNumber = "3";

            // should also be OK to set an empty string
            card.IssueNumber = "";
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void IssueNumber_ShouldNotBeATwoDigitNumber()
        {
            var card = new Card();
            card.IssueNumber = "33";
        }
    }
}