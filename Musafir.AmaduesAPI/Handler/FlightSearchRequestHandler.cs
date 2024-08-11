using AmadeusWebService;
using Musafir.AmaduesAPI.Header;
using Musafir.AmaduesAPI.Request;
using System.Xml;

namespace Musafir.AmaduesAPI.Handler
{
    public class FlightSearchRequestHandler(AmadeusSecurityHeader amadeusSecurityHeader, IConfiguration configuration) : IFightSearchRequestHandler
    {
        public Fare_MasterPricerTravelBoardSearchRequest GetRequest(FlightSearchRequestModel request)
        {
            Fare_MasterPricerTravelBoardSearch amadeusFlightSearchRequest = new()
            {
                paxReference = AddPassengerDetails(request.TotalPax),

                numberOfUnit =
            [
                new()
                {
                    typeOfUnit = "RC",
                    numberOfUnits = "200"
                },
                new()
                {
                    typeOfUnit = "PX",
                    numberOfUnits = request.TotalPax.PaxCount.ToString()
                }
            ],

                fareOptions = AddFareDetails(),

                itinerary = AddItineraires(request.Itineraries)
            };

            var fareMasterPriceTravlerboardRequest = new Fare_MasterPricerTravelBoardSearchRequest
            {
                Fare_MasterPricerTravelBoardSearch = amadeusFlightSearchRequest,
                AMA_SecurityHostedUser = GetAMA_SecurityHostedUser(),
                TransactionFlowLink = GetTransactionFlowLinkType(),
                Security = amadeusSecurityHeader
            };

            return fareMasterPriceTravlerboardRequest;
        }

        private TravellerReferenceInformationType[] AddPassengerDetails(PaxDetails paxDetails)
        {
            var paxRefernce = new List<TravellerReferenceInformationType>();
            byte paxReferenceNo = 0;

            if (paxDetails.Adult > 0)
            {
                var adutPax = new TravellerReferenceInformationType
                {
                    ptc = ["ADT"]
                };

                var travellers = new List<TravellerDetailsType>();
                for (int i = 0; i < paxDetails.Adult; i++)
                {
                    travellers.Add(new TravellerDetailsType
                    {
                        @ref = (++paxReferenceNo).ToString()
                    });
                }
                adutPax.traveller = [.. travellers];
                paxRefernce.Add(adutPax);
            }

            if (paxDetails.Child > 0)
            {
                var childPax = new TravellerReferenceInformationType
                {
                    ptc = ["CHD"]
                };

                var travellers = new List<TravellerDetailsType>();
                for (int i = 0; i < paxDetails.Child; i++)
                {
                    travellers.Add(new TravellerDetailsType
                    {
                        @ref = (++paxReferenceNo).ToString()
                    });
                }
                childPax.traveller = [.. travellers];
                paxRefernce.Add(childPax);
            }

            paxReferenceNo = 0;
            if (paxDetails.Infant > 0)
            {
                var infantPax = new TravellerReferenceInformationType
                {
                    ptc = ["INF"]
                };

                var travellers = new List<TravellerDetailsType>();
                for (int i = 0; i < paxDetails.Infant; i++)
                {
                    travellers.Add(new TravellerDetailsType
                    {
                        @ref = (++paxReferenceNo).ToString(),
                        infantIndicator = "1"
                    });
                }
                infantPax.traveller = [.. travellers];
                paxRefernce.Add(infantPax);
            }

            return [.. paxRefernce];
        }

        private Fare_MasterPricerTravelBoardSearchFareOptions AddFareDetails()
        {
            return new Fare_MasterPricerTravelBoardSearchFareOptions
            {
                pricingTickInfo = new PricingTicketingDetailsType
                {
                    pricingTicketing = ["RP", "RU", "TAC", "ET", "NSD", "OVN", "MNR"]   
                },
                feeIdDescription =
                [
                    new CodedAttributeInformationType_277155C
                    {
                        feeType = "FFI",
                        feeIdNumber = "3"
                    },
                    new CodedAttributeInformationType_277155C
                    {
                        feeType = "UPH",
                        feeIdNumber = "3"
                    },
                    new CodedAttributeInformationType_277155C
                    {
                        feeType = "UFH",
                        feeIdNumber = "1"
                    }
                ]
            };

        }

        private Fare_MasterPricerTravelBoardSearchItinerary[] AddItineraires(Itinerary[] itineraries)
        {
            var itinerariesList = new List<Fare_MasterPricerTravelBoardSearchItinerary>();
            byte segmentRefernceNo = 0;
            foreach (var itinerary in itineraries)
            {
                itinerariesList.Add(new Fare_MasterPricerTravelBoardSearchItinerary
                {
                    requestedSegmentRef = new OriginAndDestinationRequestType1
                    {
                        segRef = (++segmentRefernceNo).ToString()
                    },
                    departureLocalization = new DepartureLocationType
                    {
                        departurePoint = new ArrivalLocationDetailsType_120834C
                        {
                            locationId = itinerary.DepartureAirport
                        }
                    },
                    arrivalLocalization = new ArrivalLocalizationType
                    {
                        arrivalPointDetails = new ArrivalLocationDetailsType
                        {
                            locationId = itinerary.ArrivalAirport
                        }
                    },
                    timeDetails = new DateAndTimeInformationType_181295S
                    {
                        firstDateTimeDetail = new DateAndTimeDetailsTypeI
                        {
                            date = itinerary.DepartureDate.ToString("ddMMyy")
                        }
                    }
                });
            }

            return [.. itinerariesList];
        }

        private TransactionFlowLinkType GetTransactionFlowLinkType() => new()
        {
            Consumer = new ConsumerType
            {
                UniqueID = new UniqueId().ToString()
            }
        };

        private AMA_SecurityHostedUser GetAMA_SecurityHostedUser() => new()
        {
            UserID = new AMA_SecurityHostedUserUserID
            {
                POS_Type = "1",
                PseudoCityCode = configuration["AmadeusConfiguration:OfficeId"],
                AgentDutyCode = "SU",
                RequestorType = "U"
            }
        };
        

    }
}
