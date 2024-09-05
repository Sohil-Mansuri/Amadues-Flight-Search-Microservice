
using FluentValidation;
using Musafir.AmaduesAPI.Request;
using Musafir.AmaduesAPI.Resources;

namespace Musafir.AmaduesAPI.FluentValidation
{
    public class FlightSearchRequestValidator : AbstractValidator<FlightSearchRequestModel>
    {
        public FlightSearchRequestValidator()
        {
            RuleFor(x => x.Itineraries)
                 .NotEmpty().WithMessage(ErrorMessages.Required);

            RuleForEach(x => x.Itineraries)
                .SetValidator(new ItineraryValidator());

            RuleFor(x => x.TotalPax)
                .SetValidator(new PaxValidator());
        }
    }


    public class ItineraryValidator : AbstractValidator<Itinerary>
    {
        public ItineraryValidator()
        {
            RuleFor(itinerary => itinerary.DepartureAirport)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.Required)
                .MaximumLength(3).MinimumLength(3).WithMessage(ErrorMessages.AirprotCodeLength);

            RuleFor(itinerary => itinerary.ArrivalAirport)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.Required)
                .MaximumLength(3).MinimumLength(3).WithMessage(ErrorMessages.AirprotCodeLength);


            RuleFor(itinerary => itinerary.DepartureDate)
                .NotNull().NotEmpty().WithMessage(ErrorMessages.Required)
                .GreaterThan(DateTime.Today).WithMessage(ErrorMessages.FlightDate);
        }
    }


    public class PaxValidator : AbstractValidator<PaxDetails>
    {
        public PaxValidator()
        {
            RuleFor(pax => (int)pax.Adult)
                .InclusiveBetween(1, 5).WithMessage(ErrorMessages.PaxSelection);

            RuleFor(pax => (int)pax.Child)
                .InclusiveBetween(0, 5).WithMessage(ErrorMessages.PaxSelection);

            RuleFor(pax => (int)pax.Infant)
                .Must((model, infantCount) => infantCount <= model.Adult)
                .WithMessage(ErrorMessages.InfantLessThanAdult);

        }
    }
}
