using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TicketPlanetV2.BAL.MovieModel.ViewModel
{
    public class MovieViewModel
    {
        public class CouponObject
        {

            public int nErrorCode { get; set; }
            public string sErrorText { get; set; }
            public int CouponAgentId { get; set; }
            public int CouponId { get; set; }
            public int CouponAssignId { get; set; }
            public string CouponValue { get; set; }
            public string PromoCode { get; set; }

        }
        public class TicketRequestModel
        {
            public string showtimeId { get; set; }
            public string orderId { get; set; }
            public string cat { get; set; }
            public string siteId { get; set; }
            public string Fullname { get; set; }
            public string phoneNo { get; set; }
            public string email { get; set; }
            public int NoOfPersons { get; set; }
            public int MovieID { get; set; }
            public string TicketCategory { get; set; }
            public string Amount { get; set; }
            public int TicketType { get; set; }
            public string comments { get; set; }
            public string MovieName { get; set; }
            public string MovieDate { get; set; }
            public string MovieTime { get; set; }
            public int CinemaCompanyID { get; set; }
            public int CinemaLocation { get; set; }
            public string IsCoupon { get; set; }
            public int CouponAgentId { get; set; }
            public int CouponAssignId { get; set; }
            public int CouponID { get; set; }
            public string Coupon { get; set; }
            public string Validated { get; set; }
            public string ReferalId { get; set; }


        }

        public class MovieNameObj
        {
            public string FilmTitle { get; set; }
            public string Synopsis { get; set; }
            public string Youtube { get; set; }
            public string imgBanner { get; set; }

        }

        public class MovieBanner
        {
            public string imgBanner { get; set; }
        }

        public class MovieTimeObj
        {
            public string StartTime { get; set; }

        }

        public class PriceObject
        {
            public int nErrorCode { get; set; }
            public string OrigAmount { get; set; }
            public string Amount { get; set; }
            public string CouponValue { get; set; }
            public string amtCharge { get; set; }
        }
        public class MovieListProp
        {

            public string PerformDate { get; set; }
            public string FilmCode { get; set; }
        }
        public class UrlConcat
        {

            public string _FilmTitle { get; set; }
            public string ComingSoon { get; set; }
            public string Is3d { get; set; }
            public string PerformDate { get; set; }
            public string StartDate { get; set; }
            public string Img_1s { get; set; }
            public string SoldOutLevel { get; set; }
            public string Youtube { get; set; }
            public string PerfCat { get; set; }
            public int filmCode { get; set; }
            public int Itbid { get; set; }
            public string Screen { get; set; }
            public string SalesStopped { get; set; }
            public string StartTime { get; set; }
            public string Synopsis { get; set; }
            public string Genre { get; set; }
            public int CinemaId { get; set; }

        }
        public class UrlContentModel2
        {
            public string PerformDate { get; set; }
            public string Passes { get; set; }
            public string PerfFlags { get; set; }

            public string SoldOutLevel { get; set; }
            public string PerfCat { get; set; }
            public string DoorsOpen { get; set; }
            public string AD { get; set; }
            public string Screen { get; set; }
            public string BookingURL { get; set; }
            public string FilmCode { get; set; }
            public string SellonInternet { get; set; }
            public string TrailerTime { get; set; }
            public string WheelchairAccessible { get; set; }
            public string ReservedSeating { get; set; }
            // public Img_1s Img_1s { get; set; }
            public string SalesStopped { get; set; }
            public string PerformanceNumberSlot { get; set; }
            public string InternalBookingURLDesktop { get; set; }
            public string Code { get; set; }
            public string Subs { get; set; }
            public string InternalBookingURLMobile { get; set; }
            public string PerfFlagsDescription { get; set; }
            public string ScreenCode { get; set; }
            public string StartTime { get; set; }
            public string ManagerWarningLevel { get; set; }
            public int CinemaId { get; set; }

        }
        public class Img_1s
        {
            public string value { get; set; }
            public string ReleaseDate { get; set; }
            public string RunningTime { get; set; }
            public string Synopsis { get; set; }

        }
        public class MoviesModel
        {
            public int itbid { set; get; }
            public int ImageID { get; set; }
            public int ImageSize { get; set; }
            public string FileName { get; set; }
            public byte[] ImageData { get; set; }
            public HttpPostedFileBase File { get; set; }
        }

        public class MoviesModelList
        {
            public int itbid { get; set; }
            public Nullable<int> CinemaId { get; set; }
            public string ShortFilmTitle { get; set; }
            public string FilmTitle { get; set; }
            public string ComingSoon { get; set; }
            public string Code { get; set; }
            public string Certificate { get; set; }
            public string Is3d { get; set; }
            public string Img_title { get; set; }
            public string Rentrak { get; set; }
            public string Youtube { get; set; }
            public string ReleaseDate { get; set; }
            public string RunningTime { get; set; }
            public string Synopsis { get; set; }
            public string Certificate_desc { get; set; }
            public string Img_1s { get; set; }
            public string Digital { get; set; }
            public string Genre { get; set; }
            public string GenreCode { get; set; }
            public string StartDate { get; set; }
            public string IMDBCode { get; set; }
            public Nullable<decimal> WeekDayPrice { get; set; }
            public Nullable<decimal> WeekDayComboPrice { get; set; }
            public Nullable<decimal> WeekDayVIPPrice { get; set; }
            public Nullable<decimal> WeekDayThreeDPrice { get; set; }
            public Nullable<decimal> WeekDayCutOffPrice { get; set; }
            public Nullable<decimal> WeekDayCutOffComboPrice { get; set; }
            public Nullable<decimal> WeekDayCutOffVipPrice { get; set; }
            public Nullable<decimal> WeekDayCutOff3D { get; set; }
            public Nullable<decimal> WeekendPrice { get; set; }
            public Nullable<decimal> WeekEndComboPrice { get; set; }
            public Nullable<decimal> WeekEndVIPPrice { get; set; }
            public Nullable<decimal> WeekEndThreeDPrice { get; set; }
            public Nullable<decimal> WeekEndCutOffPrice { get; set; }
            public Nullable<decimal> WeekEndCutOffComboPrice { get; set; }
            public Nullable<decimal> WeekEndCutOffVipPrice { get; set; }
            public Nullable<decimal> WeekEndCutOff3D { get; set; }
            public Nullable<decimal> publicHolidayPrice { get; set; }
            public Nullable<decimal> promoDayPrice { get; set; }
            public string ispromoDay { get; set; }
        }
    }
}
