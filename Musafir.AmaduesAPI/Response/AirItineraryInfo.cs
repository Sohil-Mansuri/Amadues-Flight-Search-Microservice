using Musafir.AmaduesAPI.Enums;
using ProtoBuf;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Musafir.AmaduesAPI.Response
{
    [ProtoContract]
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

        [ProtoMember(1)]
        [JsonPropertyName("AirTrip")]
        public string AirTripDetails => AirTripType.ToString();

        [ProtoMember(2)]
        public List<AirOriginDestinationInfo> AirOriginDestinations { get; set; } = [];

        [ProtoMember(3)]
        public decimal TotalAmount { get; set; }

    }


    [ProtoContract]
    public class AirOriginDestinationInfo
    {
        [ProtoMember(1)]
        public List<FlightSegmentInfo> FlightSegments { get; set; } = [];

        [ProtoMember(2)]
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

    [ProtoContract]
    public class FlightSegmentInfo
    {
        CultureInfo _provider = CultureInfo.InvariantCulture;

        [ProtoMember(1)]
        public string DepartureAirport { get; set; } = string.Empty;

        [ProtoMember(2)]
        public string ArrivalAirport { get; set; } = string.Empty;

        [ProtoMember(3)]
        public DateTime DepartureDateTime { get; set; }

        [ProtoMember(4)]
        public DateTime ArrivalDateTime { get; set; }

        [ProtoMember(5)]
        public string FlightNumber { get; set; } = string.Empty;

        [ProtoMember(6)]
        public byte Stops { get; set; }

        [ProtoMember(7)]
        public string OperatingAirline { get; set;} = string.Empty;

        [ProtoMember(8)]
        public string MarketingAirline { get; set;} = string.Empty;
    }


}
