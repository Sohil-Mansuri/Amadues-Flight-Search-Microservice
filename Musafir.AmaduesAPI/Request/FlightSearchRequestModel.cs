using Musafir.AmaduesAPI.CustomValidation;
using Musafir.AmaduesAPI.Resources;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Musafir.AmaduesAPI.Request
{
    public class FlightSearchRequestModel
    {
        public Itinerary[] Itineraries { get; set; } = [];
        public PaxDetails TotalPax { get; set; } = new();
    }

    public class Itinerary
    {
        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Required), ErrorMessageResourceType =  typeof(ErrorMessages))]
        [MinLength(3, ErrorMessageResourceName = nameof(ErrorMessages.AirprotCodeLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        [MaxLength(3, ErrorMessageResourceName = nameof(ErrorMessages.AirprotCodeLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public string DepartureAirport { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [MinLength(3, ErrorMessageResourceName = nameof(ErrorMessages.AirprotCodeLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        [MaxLength(3, ErrorMessageResourceName = nameof(ErrorMessages.AirprotCodeLength), ErrorMessageResourceType = typeof(ErrorMessages))]
        public string ArrivalAirport { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = nameof(ErrorMessages.Required), ErrorMessageResourceType = typeof(ErrorMessages))]
        [FutureDate(ErrorMessageResourceName = nameof(ErrorMessages.FlightDate),  ErrorMessageResourceType = typeof(ErrorMessages))]
        public DateTime DepartureDate { get; set; }

    }

    public class PaxDetails
    {
        [Range(1,10, ErrorMessageResourceName = nameof(ErrorMessages.PaxSelection), ErrorMessageResourceType = typeof(ErrorMessages))]
        public byte Adult { get; set; } = 1;

        [Range(0, 10, ErrorMessageResourceName = nameof(ErrorMessages.PaxSelection), ErrorMessageResourceType = typeof(ErrorMessages))]
        public byte Child { get; set; }

        [Range(0, 10, ErrorMessageResourceName = nameof(ErrorMessages.PaxSelection), ErrorMessageResourceType = typeof(ErrorMessages))]
        [InfantLessThanAdult(ErrorMessageResourceName = nameof(ErrorMessages.InfantLessThanAdult), ErrorMessageResourceType = typeof(ErrorMessages))]
        public byte Infant { get; set; }

        [JsonIgnore]
        public int PaxCount => Adult + Child + Infant;
    }


    public enum PaxType
    {
        ADT,
        CHD,
        INF
    }
}
