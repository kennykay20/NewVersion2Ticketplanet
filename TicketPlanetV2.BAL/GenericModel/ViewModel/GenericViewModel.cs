using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicketPlanetV2.BAL.MovieModel;
using TicketPlanetV2.BAL.Utilities;
using TicketPlanetV2.DAL.Entity;
using static TicketPlanetV2.BAL.CareerModel.CareerViewModel.CareerDto;
using static TicketPlanetV2.BAL.MovieModel.ViewModel.MovieViewModel;

namespace TicketPlanetV2.BAL.GenericModel.ViewModel
{
   public class GenericViewModel
    {
        public string FreeEventCustomerName { get; set; }
        public string FreeEventCustomerPhoneNo { get; set; }
        public string FreeEventCustomerEmail { get; set; }
        public int FreeEventNoofPersons { get; set; }
        public string times { get; set; }
        public int CanView { get; set; }
        public int CanAdd { get; set; }
        public int CanEdit { get; set; }
        public int CanAuth { get; set; }
        public int CanDelete { get; set; }
        public ReturnValues rv { get; set; }
        public tk_LoadingImages sliderImages { get; set; }
        public IEnumerable<CareerProperties> ListOfCareers { get; set; }
        public IEnumerable<tk_Routes> ListOfRoutes { get; set; }
        public IEnumerable<tk_Routes> ReturnRoute { get; set; }
        public IEnumerable<tk_Passenger> ListOfPassengers { get; set; }
        public IEnumerable<SelectListItem> drpTravellingFrom { get; set; }
        public IEnumerable<SelectListItem> drpFlightTravellingFrom { get; set; }
        public IEnumerable<SelectListItem> drpTravellingTo { get; set; }
        public IEnumerable<SelectListItem> drpEventCategory { get; set; }
        public string eventCategoryName { get; set; }
        public IEnumerable<SelectListItem> drpCinema { get; set; }
        public IEnumerable<SelectListItem> drpGenesisCinema { get; set; }
        public IEnumerable<SelectListItem> drpMaturionCinema { get; set; }
        public List<CinemaChainSite> CinemaChainSiteList { get; set; }
        public IEnumerable<SelectListItem> drpBusCompany { get; set; }
        public List<Film> CinemaChainFilms { get; set; }
        public string cinemaCompany { get; set; }
        public IEnumerable<SelectListItem> drpMovieDay { get; set; }
        public IEnumerable<SelectListItem> drpMovieTime { get; set; }
        public IEnumerable<SelectListItem> drpMovieCategory { get; set; }
        public IEnumerable<SelectListItem> drpMovieLocatn { get; set; }
        public List<tk_Event> EventList { get; set; }
        public List<tk_FreeEvents> FreeEventList { get; set; }
        public List<UrlConcat> MovieList { get; set; }
        public List<UrlConcat> MaturionMovieList { get; set; }
        public string MovieName { get; set; }
        public string MovieSynopsis { get; set; }
        public string MovieYouTube { get; set; }
        public string Img_Banner { get; set; }
        public int CinemaID { get; set; }

        public int MaturionCinemaID { get; set; }
        [Required(ErrorMessage = "Travelling From Cannot Be Empty")]

        public string TravellingFrom { get; set; }
        public string TravellingTo { get; set; }

        [Required(ErrorMessage = "Company Name Cannot Be Empty")]
        public string CompanyName { get; set; }
        public string MovieCategory { get; set; }

        public int SeatTotalNo { get; set; }
        public int oneWayTrip { get; set; }
        public int ReturnTrip1 { get; set; }
        public int ReturnTrip2 { get; set; }

        public string TripType { get; set; }
        public string TripType2 { get; set; }
        public string TransactionRef { get; set; }
        public string PromoRef { get; set; }
        public string MovieDay { get; set; }
        public string MovieTime { get; set; }
        public string IsFreeEvent { get; set; }
        public string ArrvDay { get; set; }
        public string DeptDay { get; set; }
        [Required(ErrorMessage = "Departure Date Cannot Be Empty")]
        public DateTime? DepartureDate { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string hotel { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public int adult { get; set; }
        public int BusTypeID { get; set; }
        public int child { get; set; }
        public string CinemaName { get; set; }
        public string Synopsis { get; set; }
        public string FreeMovieRef { get; set; }
        public tk_FreeEventCustomers freeCustEvent { get; set; }
       // public TicketPlanet.BAL.EventModel.EventClassModel.EventObjects eventObj { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public string EventTime { get; set; }
        public string ThreeDStatus { get; set; }
        public string EventDate { get; set; }
        public string EventDescription { get; set; }
        public string FeeCatorgory { get; set; }
        public string ExtraCharges { get; set; }
        public string EventImagePath { get; set; }
        public byte[] EventImage { get; set; }
        public string _FilmTitle { get; set; }
        public string FilmCode { get; set; }
        public int CinemaCompanyID { get; set; }
        public int TicketType { get; set; }
        public string referalId { get; set; }
        public int TicketCategory { get; set; }
        public tk_Event tk_Event { get; set; }
        public tk_EventCategory tk_EventCategory { get; set; }
        public tk_CinemaTransactionLog tk_CinemaTransactionLog { get; set; }
        public tk_EventCustomers tk_EventCustomers { get; set; }
        public tk_Routes tk_Routes { get; set; }
        public tk_Routes tk_RoutesReturn1 { get; set; }
        public tk_Routes tk_RoutesReturn2 { get; set; }
        public tk_Location tk_Location { get; set; }
        public tk_BusCompany tk_BusCompany { get; set; }


        public string GetFilmHouseLocation(string id)
        {
            return new MoviesModelClass().GetFilmHouseLocation(id);
        }

        public string GetGenesisorManturioLocation(int id)
        {
            return new MoviesModelClass().GetGenesisorManturioLocation(id);
        }
        
        public string FormattedAmount(decimal amount)
        {
            return amount.ToString("N2", CultureInfo.InvariantCulture);
        }
        public string FormatDate(DateTime? date)
        {
            return string.Format("{0:ddd, MMM d, yyyy}", date);

        }

        public string FormatDateCinema(string date)
        {
            DateTime FilmDate;
            //string fDate;

            if (string.IsNullOrEmpty(date))
            {
                return null;
            }
            else
            {
                return DateTime.TryParse(date, out FilmDate) ? string.Format("{0:dd-MMM-yyyy}", FilmDate) : null;

            }


        }


        public string DetermineSex(string sex)
        {
            return sex == "M" ? "MALE" : "FEMALE";
        }
        public string GetFathersDayCategory(int cat)
        {
            return cat == 9 ? "Family of Five" : "Couple";
        }


        public string ViewType(string viewType)
        {
            string ViewType = "";
            if (viewType == "1")
            {
                ViewType = "Regular 2D";
            }
            else if (viewType == "2")
            {
                ViewType = "Combo (Includes Large Popcorn & Drink)";

            }
            else if (viewType == "3")
            {
                ViewType = "Regular 3D";

            }
            else if (viewType == "4")
            {
                ViewType = "VIP 2D (Includes Large Popcorn & Drink)";

            }
            return ViewType;
        }


        //public List<CinemaChainSite> CinemaChainSiteList { get; set; }
        //public List<Film> CinemaChainFilms { get; set; }
        //public List<ShowTime> FilmShowTimes { get; set; }
        public List<DateTime> FilmShowDates { get; set; }
        public string CinemaChainSiteId { get; set; }
        public List<MoviesModelClass.ShowtimeList2> ShowtimeList { get; set; }

        /////////////////// Bus Booking  Setter / Getter Properties
        public string SiteId { get; set; }
        public string filmId { get; set; }
        public string OnewayTravellingFrom { get; set; }
        public string OnewayTravellingTo { get; set; }

        public string BookingId { get; set; }

        ///////  Setter and Getter for Passenger and contact Information
        public tk_Passenger tk_Passenger { get; set; }
        public tk_ContactInformation tk_ContactInformation { get; set; }


        ///Processing Bus Booking Form Details
        public string OnePassegerName { get; set; }
        public string OnePassegerSex { get; set; }

        public string TwoPassegerName { get; set; }
        public string TwoPassegerSex { get; set; }

        public string ThreePassegerName { get; set; }
        public string ThreePassegerSex { get; set; }

        public string FourPassegerName { get; set; }
        public string FourPassegerSex { get; set; }


        public string FivePassegerName { get; set; }
        public string FivePassegerSex { get; set; }





        //////////////////////////////////////////////////////////////////////////////
        public IEnumerable<tk_FlightRoutes> ListofFlightRoutes { get; set; }
        public IEnumerable<tk_FlightRoutes> ReturnFlightRoute { get; set; }
        public IEnumerable<tk_FlightPassenger> ListOfFlightPassengers { get; set; }


        public tk_FlightPassenger tk_FlightPassenger { get; set; }
        public tk_FlightContactInformation tk_FlightContactInformation { get; set; }
        public tk_FlightRoutes tk_FlightRoutes { get; set; }
        public tk_FlightRoutes tk_FlightRoutesReturn1 { get; set; }
        public tk_FlightRoutes tk_FlightRoutesReturn2 { get; set; }
        public tk_Flight_Location tk_FlightLocation { get; set; }
        public tk_FlightCompany tk_FlightCompany { get; set; }



    }
}
