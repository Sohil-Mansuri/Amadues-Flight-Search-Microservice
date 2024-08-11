using AmadeusWebService;
using Musafir.AmaduesAPI.Response;
using System.Globalization;

namespace Musafir.AmaduesAPI.Handler
{
    public class FlightResponseHandler : IFlightResponseHandler
    {
        public AirItineraryInfo[]? GetFlightResponse(Fare_MasterPricerTravelBoardSearchReply reply)
        {
            List<AirItineraryInfo> flights = [];


            #region Handling Reply
            bool isErrorResponse = HandleErrorForTravelBoardFlightSearch(reply);
            if (isErrorResponse)
                return null;

            #endregion

            //Init 
            var allFlights = GetAllFlightDetails(reply);

            //Process each recommendation
            for (int i = 0; i < reply.recommendation.Length; i++)
            {
                var recommendation = reply.recommendation[i];

                for (int z = 0; z < recommendation.segmentFlightRef.Length; z++)
                {
                    var segmentFlightRef = recommendation.segmentFlightRef[z];
                    var flight = new AirItineraryInfo
                    {
                    };

                    //Filling the appropriate ODs for each recommended itinerary
                    var airOriginDestinationList = new List<AirOriginDestinationInfo>();
                    for (int k = 0; k < segmentFlightRef.referencingDetail.Length; k++)
                    {
                        #region Reference Qualifier codesets (Ref: 1153 1A 06.1.1)
                        //Value 	Description
                        //C 	Conversion details reference number
                        //R 	Fee/reduction reference number
                        //S 	Segment/service reference number
                        //U		UpSell reference number
                        #endregion
                        if (segmentFlightRef.referencingDetail[k].refQualifier == "S")
                        {
                            airOriginDestinationList.Add((allFlights[k][Convert.ToInt32(segmentFlightRef.referencingDetail[k].refNumber) - 1]).Clone());
                        }
                    }

                    flight.AirOriginDestinations = airOriginDestinationList;

                    //Get flight amount for all the passengers 
                    foreach (var paxFare in recommendation.paxFareProduct)
                    {
                        flight.TotalAmount += paxFare.paxFareDetail.totalFareAmount;
                    }

                    flights.Add(flight);
                }
            }

            return [.. flights];
        }

        private bool HandleErrorForTravelBoardFlightSearch(Fare_MasterPricerTravelBoardSearchReply reply)
        {
            if (reply.errorMessage != null)
            {
                return reply.errorMessage.applicationError.applicationErrorDetail.error switch
                {
                    //System unable to process
                    "118" or "866" or "931" or "977" or "996" or "899" => true,
                    _ => false,
                };
            }

            return false;
        }

        private List<List<AirOriginDestinationInfo>> GetAllFlightDetails(Fare_MasterPricerTravelBoardSearchReply reply)
        {
            //HandleError
            if (reply.errorMessage is not null)
            {
                throw new System.Exception(string.Join(',', reply.errorMessage.errorMessageText.description));
            }

            var allFlights = new List<List<AirOriginDestinationInfo>>();
            for (int i = 0; i < reply.flightIndex.Length; i++)  //flightIndex = List of AirOriginDestinations representing the same index values of the requested ODs
            {
                if (Convert.ToInt32(reply.flightIndex[i].requestedSegmentRef.segRef) != i + 1)  //requestedSegment = original AirOriginDestination request
                {
                    throw new System.Exception("llogical flightIndex order");
                }

                var airOriginDestinationList = new List<AirOriginDestinationInfo>();
                allFlights.Add(airOriginDestinationList);

                for (int j = 0; j < reply.flightIndex[i].groupOfFlights.Length; j++)    //groupOfFlights = AirOriginDestination
                {
                    var airOriginDestination = new AirOriginDestinationInfo
                    {

                    };

                    var groupOfFlights = reply.flightIndex[i].groupOfFlights[j];
                    for (int k = 0; k < groupOfFlights.propFlightGrDetail.flightProposal.Length; k++)
                    {
                        #region Type of Units Qualifier codesets (Ref: 6353 1A 01.0.10)
                        //Value 	Description
                        //EFT 	Elapse Flying Time
                        //MCX 	Majority carrier
                        #endregion
                        switch (groupOfFlights.propFlightGrDetail.flightProposal[k].unitQualifier)
                        {
                            case "EFT":
                                airOriginDestination.TotalTripTime = TimeSpan.FromHours(Convert.ToDouble(groupOfFlights.propFlightGrDetail.flightProposal[k].@ref.Substring(0, 2))) +
                                    TimeSpan.FromMinutes(Convert.ToDouble(groupOfFlights.propFlightGrDetail.flightProposal[k].@ref.Substring(2, 2)));
                                break;

                            default:
                                break;
                        }
                    }

                    for (int k = 0; k < groupOfFlights.flightDetails.Length; k++)
                    {
                        var flightInformation = groupOfFlights.flightDetails[k].flightInformation;
                        var flightSegment = new FlightSegmentInfo
                        {
                            DepartureAirport = flightInformation.location[0].locationId,
                            ArrivalAirport = flightInformation.location[1].locationId,
                            DepartureDateTime = DateTime.ParseExact(flightInformation.productDateTime.dateOfDeparture + flightInformation.productDateTime.timeOfDeparture, "ddMMyyHHmm", DateTimeFormatInfo.InvariantInfo),
                            ArrivalDateTime = DateTime.ParseExact(flightInformation.productDateTime.dateOfArrival + flightInformation.productDateTime.timeOfArrival, "ddMMyyHHmm", DateTimeFormatInfo.InvariantInfo),
                            MarketingAirline = flightInformation.companyId.marketingCarrier,
                            FlightNumber = flightInformation.flightOrtrainNumber,
                            Stops = (flightInformation.productDetail.techStopNumber != null) ? Convert.ToByte(flightInformation.productDetail.techStopNumber) : (byte)0,
                        };

                        //Add flight segment
                        airOriginDestination.FlightSegments.Add(flightSegment);
                    }


                    airOriginDestinationList.Add(airOriginDestination);
                }
            }

            return allFlights;
        }
    }
}
