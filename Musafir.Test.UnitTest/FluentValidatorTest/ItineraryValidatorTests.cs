
using FluentValidation.TestHelper;
using Musafir.AmaduesAPI.FluentValidation;
using Musafir.AmaduesAPI.Request;
using NUnit.Framework;

namespace Musafir.Test.UnitTest.FluentValidatorTest
{
    [TestFixture]
    public class ItineraryValidatorTests
    {
        private ItineraryValidator? _validator;

        [SetUp]
        public void Setup() => _validator = new ItineraryValidator();


        [Test]
        public void Should_Have_Error_When_DepaturCode_Is_Empty()
        {
            //Arrange
            var model = new Itinerary
            {
                DepartureAirport = "",
                ArrivalAirport = "DXB",
                DepartureDate = DateTime.Now.AddDays(1)
            };

            //Act
            var result = _validator.TestValidate(model);

            //Assert
            result.ShouldHaveValidationErrorFor(m => m.DepartureAirport);
        }

        [Test]
        public void Should_Have_Error_When_DepaturCode_Is_InValid()
        {
            //Arrange
            var model = new Itinerary
            {
                DepartureAirport = "BE",
                ArrivalAirport = "DXB",
                DepartureDate = DateTime.Now.AddDays(1)
            };

            //Act
            var result = _validator.TestValidate(model);

            //Assert
            result.ShouldHaveValidationErrorFor(m => m.DepartureAirport);
        }

        [Test]
        public void Should_Have_Error_When_DepatureDate_Is_InValid()
        {
            //Arrange
            var model = new Itinerary
            {
                DepartureAirport = "BOM",
                ArrivalAirport = "DXB",
                DepartureDate = DateTime.Now.AddDays(-1)
            };

            //Act
            var result = _validator.TestValidate(model);

            //Assert
            result.ShouldHaveValidationErrorFor(m => m.DepartureDate);
        }

    }
}
