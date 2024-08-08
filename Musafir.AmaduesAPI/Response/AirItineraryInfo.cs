using Musafir.AmaduesAPI.Enums;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Musafir.AmaduesAPI.Response
{
    public class AirItineraryInfo
    {
        [JsonIgnore]
        public AirTripType AirTripType
        {
            get
            {
                if(AirOriginDestinations.Count == 1)
                {
                    return AirTripType.OneWay;
                }
                else if(AirOriginDestinations.Count == 2)
                {
                    return AirTripType.RoundTrip;
                }
                else
                {
                    return AirTripType.Other;
                }
            }
        }

        [JsonPropertyName("AirTrip")]
        public string AirTripDetails => AirTripType.ToString();
        
        public List<AirOriginDestinationInfo> AirOriginDestinations { get; set; } = [];

        public decimal TotalAmount { get; set; }

    }


    public class AirOriginDestinationInfo
    {
        public virtual List<FlightSegmentInfo> FlightSegments { get; set; } = [];

        public TimeSpan? TotalTripTime { get; set; }


        public virtual AirOriginDestinationInfo Clone()
        {
            if (this == null)
            {
                throw new ArgumentNullException(nameof(AirOriginDestinationInfo));
            }

            var serializer = new DataContractSerializer(typeof(AirOriginDestinationInfo));
            using var ms = new MemoryStream();
            serializer.WriteObject(ms, this);
            ms.Position = 0;
            return (AirOriginDestinationInfo)serializer?.ReadObject(ms);
        }


    }


    public class FlightSegmentInfo
    {
        CultureInfo _provider = CultureInfo.InvariantCulture;
        
        public string DepartureAirport { get; set; } = string.Empty;

        public string ArrivalAirport { get; set; } = string.Empty;


        public DateTime DepartureDateTime { get; set; }

        public DateTime ArrivalDateTime { get; set; }

        public string FlightNumber { get; set; } = string.Empty;

        public byte Stops { get; set; }

        public string OperatingAirline { get; set;} = string.Empty;

        public string MarketingAirline { get; set;} = string.Empty;
    }


}
