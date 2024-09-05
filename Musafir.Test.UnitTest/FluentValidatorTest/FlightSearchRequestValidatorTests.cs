using FluentValidation.TestHelper;
using Musafir.AmaduesAPI.FluentValidation;
using Musafir.AmaduesAPI.Request;
using NUnit.Framework;

namespace Musafir.Test.UnitTest.FluentValidatorTest
{
    [TestFixture]
    public class FlightSearchRequestValidatorTests
    {
        private FlightSearchRequestValidator? _validator;

        [SetUp]
        public void SetUp() => _validator = new FlightSearchRequestValidator();

        [Test]
        public void Should_Have_Error_When_Itineraries_Is_Empty()
        {
            //Arrange
            var model = new FlightSearchRequestModel
            {
                Itineraries = [],
                TotalPax = new PaxDetails()
            };

            //Act
            var result = _validator.TestValidate(model);

            //Assert
            result.ShouldHaveValidationErrorFor(m => m.Itineraries);
        }

        [Test]
        public void Should_Have_Error_When_PaxDetails_Is_InValid()
        {
            var model = new FlightSearchRequestModel
            {
                Itineraries =
                [
                    new()
                    {
                        DepartureAirport = "BOM",
                        ArrivalAirport = "DXB",
                        DepartureDate = DateTime.Now.AddDays(1)
                    }
                ],
                TotalPax = new PaxDetails
                {
                    Adult = 0,
                    Child = 0,
                    Infant = 0,
                }
            };


            //Act
            var result = _validator.TestValidate(model);

            //Assert
            result.ShouldHaveValidationErrorFor(m => m.TotalPax.Adult);
        }

        [Test]
        public void Should_Have_Error_When_Infant_Is_Invalid()
        {
            var model = new FlightSearchRequestModel
            {
                Itineraries =
                [
                    new()
                    {
                        DepartureAirport = "BOM",
                        ArrivalAirport = "DXB",
                        DepartureDate = DateTime.Now.AddDays(1)
                    }
                ],
                TotalPax = new PaxDetails
                {
                    Adult = 2,
                    Child = 0,
                    Infant = 3,
                }
            };


            //Act
            var result = _validator.TestValidate(model);

            //Assert
            result.ShouldHaveValidationErrorFor(m => m.TotalPax.Infant);
        }
    }
}
