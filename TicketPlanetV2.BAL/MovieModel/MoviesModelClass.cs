using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using TicketPlanet.DAL.RepositoryCalls;
using TicketPlanetV2.BAL.Utilities;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;
using TicketPlanetV2.DAL.Entity;
using static TicketPlanetV2.BAL.MovieModel.ViewModel.MovieViewModel;
using TicketPlanetV2.BAL.GenericModel.ViewModel;

namespace TicketPlanetV2.BAL.MovieModel
{
    public class MoviesModelClass
    {

        private const int NumberOfRetries = 3;
        private const int DelayOnRetry = 6000;
        public DapperCalls oDapperCalls;
        private readonly ICouponsRepository repoCoupon;
        private readonly ICouponCodeAssignmentRepository repoCouponAssign;
        private readonly ICouponsSetUpRepository repoCouponSetUp;
        private readonly IThePalmsFirm repoPalmsfilm;
        private readonly ISmsRepository repoSms;
        private readonly IFilmHouseFilmsRepository repoFilmHouseFilms;
        private readonly IAbujaFilmRepository repoAbujafilm;
        private readonly IThePalmsPerformanceFilm repoPalmsPerformance;
        private readonly IOtherFilmRepository repoOtherFilms;
        private readonly IAbujaPerformanceRepository repoAbujaPerformance;
        private readonly IAjahFilmRepository repoAjahFilm;
        private readonly IAjahPerformanceRepository repoAjahPerformance;
        private readonly IAsabaFilmRepository repoAsabaFilm;
        private readonly IAsabaPerformanceRepository repoAsabaPeformance;
        private readonly IGateWayFilmRepository repoGatewayFilm;
        private readonly IGateWayPerformanceRepository repoGatewayPerformance;
        private readonly IMaryLandFilmRepository repoMaryLandFilm;
        private readonly IMaryLandPerformanceRepository repoMaryLandPeformance;
        private readonly IOwerriFilmRepository repoOwerriFilm;
        private readonly IOwerriPerformanceRepository repoOwerriPerformance;
        private readonly IPortHarcourtFilmRepository repoPhFilms;
        private readonly IPortHarcourtPerformanceRepository repoPHPerformance;
        private readonly IWarriFilmRepository repoWarriFilms;
        private readonly IWarriPerformanceRepository repoWarriPerformance;
        IDbConnection db = null;
        public GenericViewModel oGenericViewModel;
        private readonly IPromoDayControlLogRepository repoPromoLog;
        private readonly ICinemaRepository repoCinema;
        private readonly IMoviesRepository repoMovies;
        private readonly IUserProfileRepository repoadmUserProfile;
        private readonly IClientServiceSetUpRepository repoServiceSetUp;
        private readonly IThePalmsFirm repoGenesisFilm;
        private readonly IThePalmsPerformanceFilm repoGenesisPerform;
        private readonly ICinemaTransactionLogRepository repoCinemaTranLog;
        private readonly ISliderRepository repoSliders;
        private readonly IBatchCounterRpository repotk_BatchCounter;
        private readonly ICinemaPricingRepository repoCinemaPricing;
        private readonly IClientProfileRepository repoClientProfileRepository;
        private readonly IFlightRoutesRepository repoRoutes;
        
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;

        public MoviesModelClass()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repoCinema = new CinemaRepository(idbfactory);
            repoadmUserProfile = new UserProfileRepository(idbfactory);
            repoMovies = new MoviesRepository(idbfactory);
            repoRoutes = new FlightRoutesRepository(idbfactory);
            repotk_BatchCounter = new BatchCounterRpository(idbfactory);
            repoServiceSetUp = new ClientServiceSetUpRepository(idbfactory);
            repoCinemaPricing = new CinemaPricingRepository(idbfactory);
            repoGenesisFilm = new ThePalmsFirm(idbfactory);
            repoFilmHouseFilms = new FilmHouseFilmsRepository(idbfactory);
            repoGenesisPerform = new ThePalmsPerformanceFilm(idbfactory);
            repoOtherFilms = new OtherFilmRepository(idbfactory);
            repoCinemaTranLog = new CinemaTransactionLogRepository(idbfactory);
            repoAbujafilm = new AbujaFilmRepository(idbfactory);
            repoSliders = new SliderRepository(idbfactory);
            repoAbujaPerformance = new AbujaPerformanceRepository(idbfactory);
            repoAjahFilm = new AjahFilmRepository(idbfactory);
            repoAjahPerformance = new AjahPerformanceRepository(idbfactory);
            repoAsabaFilm = new AsabaFilmRepository(idbfactory);
            repoAsabaPeformance = new AsabaPerformanceRepository(idbfactory);
            repoGatewayFilm = new GateWayFilmRepository(idbfactory);
            repoGatewayPerformance = new GateWayPerformanceRepository(idbfactory);
            repoMaryLandFilm = new MaryLandFilmRepository(idbfactory);
            repoMaryLandPeformance = new MaryLandPerformanceRepository(idbfactory);
            repoOwerriFilm = new OwerriFilmRepository(idbfactory);
            repoOwerriPerformance = new OwerriPerformanceRepository(idbfactory);
            repoPhFilms = new PortHarcourtFilmRepository(idbfactory);
            repoPHPerformance = new PortHarcourtPerformanceRepository(idbfactory);
            repoWarriFilms = new WarriFilmRepository(idbfactory);
            repoWarriPerformance = new WarriPerformanceRepository(idbfactory);
            repoCoupon = new CouponsRepository(idbfactory);
            repoSms = new SmsRepository(idbfactory);
            oGenericViewModel = new GenericViewModel();
            repoClientProfileRepository = new ClientProfileRepository(idbfactory);
            repoCouponAssign = new CouponCodeAssignmentRepository(idbfactory);
            repoCouponSetUp = new CouponsSetUpRepository(idbfactory);
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            oDapperCalls = new DapperCalls();
            repoPromoLog = new PromoDayControlLogRepository(idbfactory);
            repoPalmsfilm = new ThePalmsFirm(idbfactory);
        }



        public int ImageID { get; set; }
        public int ImageSize { get; set; }
        public string FileName { get; set; }
        public byte[] ImageData { get; set; }

        public string GetFilmHouseLocation(string siteId)
        {

            return repoCinema.GetNonAsync(o => o.SiteId == siteId).CinemaName;

        }

        public string GetGenesisorManturioLocation(int Id)
        {

            return repoCinema.GetNonAsync(o => o.Itbid == Id).CinemaName;

        }
        public class TicketTypesRtv
        {
            public string id { get; set; }
            public string areaCategoryCode { get; set; }
            public string description { get; set; }
            public string price { get; set; }
            public bool isAllocatedSeating { get; set; }
        }

        public async Task<tk_LoadingImages> GetLoadingImages()
        {
            try
            {


                var details =await repoSliders.Get(x => x.UserId == 1);
               // var details = repoSliders.Get(x => x.UserId == 1);

                return details;

            }
            catch (Exception ex)
            {


            }
            return null;
        }

        public IEnumerable<SelectListItem> ListOfCategories(string filmId, string siteId)
        {
            IEnumerable<System.Web.Mvc.SelectListItem> items = null;
            MovieCategory movie = new MovieCategory();
            List<MovieCategory> list = new List<MovieCategory>();

            string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetTicketTypes?siteId=" + siteId + "&cinemaChainId=93cb8143-6264-e811-80c3-0004ffb07dad";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json;";
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
            string result1 = string.Empty;
            result1 = responseStream.ReadToEnd();
            httpResponse.Close();
            string input = result1.Replace("\\", string.Empty);
            input = input.Trim('"');
            var obj = JsonConvert.DeserializeObject<List<RootObject>>(input);
            
            if (obj != null)
            {

                var vals = obj.Where(o => o.filmId == filmId && o.siteId == siteId).ToList();
                var tranList = new List<TicketType2>();
                if (vals != null)
                {
                    var tran = new TicketType2();
                    foreach (var item in vals.ToList())
                    {
                        var tks = item.ticketTypes.ToList();

                        foreach (var tk in tks)
                        {

                            tranList.Add(new TicketType2
                            {
                                description = tk.description,
                                price = tk.price,
                                id = tk.id
                            });

                        }

                    }


                    items = tranList.GroupBy(test => test.description)
                   .Select(grp => grp.First())
                   .AsEnumerable()
                         .Select(p => new System.Web.Mvc.SelectListItem
                         {
                             Text = p.description,
                             Value = p.price.ToString() + "_" + p.id
                         });



                }

            }


            return items;

        }
        //public IEnumerable<SelectListItem> ListOfCategories(string showtimeId)
        //{
        //    IEnumerable<System.Web.Mvc.SelectListItem> items = null;
        //    MovieCategory movie = new MovieCategory();
        //    List<MovieCategory> list = new List<MovieCategory>();

        //    string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetTicketTypes2?cinemaChainId=" + "93cb8143-6264-e811-80c3-0004ffb07dad" + "&showtimeId=" + showtimeId;
        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //    httpWebRequest.ContentType = "application/json;";
        //    httpWebRequest.Method = "GET";
        //    httpWebRequest.Accept = "application/json";
        //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

        //    Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
        //    StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
        //    string result1 = string.Empty;
        //    result1 = responseStream.ReadToEnd();
        //    httpResponse.Close();
        //    string input = result1.Replace("\\", string.Empty);
        //    input = input.Trim('"');
        //    var obj = JsonConvert.DeserializeObject<List<TicketTypesRtv>>(input);

        //    if (obj != null)
        //    {



        //        items = obj.GroupBy(test => test.description)
        //       .Select(grp => grp.First())
        //       .AsEnumerable()
        //             .Select(p => new System.Web.Mvc.SelectListItem
        //             {
        //                 Text = p.description,
        //                 Value = p.price.ToString() + "_" + p.id
        //             });



        //    }

        //    return items;

        //}


        public ShowtimeList2 GetMovieDetails(string id)
        {
            var rtv = new ShowtimeList2();
            var splitId = id.Split('*');
            string ids = splitId[0].ToString();
            var res = repoFilmHouseFilms.GetNonAsync(o => o.id == ids);
            if (res != null)
            {
                rtv.title = res.title;
                rtv.plot = res.plot;
                //rtv.youtube = res.MovieUrl;
                rtv.youtube = res.mxfReleaseId;
                return rtv;

            }
            return null;
        }
        public HttpPostedFileBase File { get; set; }

      

        public string SendFilmHouseEmail(tk_CinemaTransactionLog item)
        {
            try
            {
                string message = SmartObject.PopulateUserBody(2);
                message = message.Replace("{{user}}", item.ContactFullname)
                                 .Replace("{{amt}}", "(₦) " + oGenericViewModel.FormattedAmount((decimal)item.TotalAmount))
                                 .Replace("{{moviename}}", item.MovieName)
                                 .Replace("{{bookingID}}", item.BookingRef)
                                 .Replace("{{Fullname}}", item.ContactFullname)
                                 .Replace("{{amt}}", "(₦) " + oGenericViewModel.FormattedAmount((decimal)item.TotalAmount))
                                 .Replace("{{location}}", oGenericViewModel.GetGenesisorManturioLocation((int)item.CinemaCompanyLocation))
                                 .Replace("{{movieDate}}", item.MovieDate)
                                 .Replace("{{movieTime}}", item.MovieTime)
                                 .Replace("{{category}}", item.ViewType)
                                 .Replace("{{emailAddress}}", item.ContactEmail)
                                 .Replace("{{phone}}", item.ContactPhoneNo)
                                 .Replace("{{noOfTickets}}", item.Units.ToString())
                                 .Replace("{{phone}}", item.ContactPhoneNo)
                                 .Replace("{{tranDate}}", oGenericViewModel.FormatDate((DateTime)item.TransactionDate));
                return message;

            }
            catch (Exception)
            {

                return null;
            }


        }


        public string SendGenesisMaturionEmail(tk_CinemaTransactionLog item) 
        {
            try
            {
                string message = SmartObject.PopulateUserBody(0);
                message = message.Replace("{{user}}", item.ContactFullname)
                                 .Replace("{{amt}}", "(₦) " + oGenericViewModel.FormattedAmount((decimal)item.TotalAmount))
                                 .Replace("{{moviename}}", item.MovieName)
                                 .Replace("{{bookingID}}", item.ReferenceNo)
                                 .Replace("{{Fullname}}", item.ContactFullname)
                                 .Replace("{{amt}}", "(₦) " + oGenericViewModel.FormattedAmount((decimal)item.TotalAmount))
                                 .Replace("{{location}}", oGenericViewModel.GetGenesisorManturioLocation((int)item.CinemaCompanyLocation))
                                 .Replace("{{movieDate}}", item.MovieDate)
                                 .Replace("{{movieTime}}", item.MovieTime)
                                 .Replace("{{category}}", item.ViewType == "1" ? "Regular 2D" : (item.ViewType == "4" ? "VIP" : (item.ViewType == "3" ? "3D" : "Combo")))
                                 .Replace("{{emailAddress}}", item.ContactEmail)
                                 .Replace("{{phone}}", item.ContactPhoneNo)
                                  .Replace("{{noOfTickets}}", item.Units.ToString())
                                 .Replace("{{tranDate}}", oGenericViewModel.FormatDate((DateTime)item.TransactionDate));
                return message;

            }
            catch (Exception ex)
            {

                return null;
            }
        
        
        }

        public int GetCurrentCounter()
        {

            var counter = repotk_BatchCounter.GetNonAsync(null).BatchNo;
            return (int)counter;
        }
        public class ShowtimeList
        {
            public string id { get; set; }
            public string screenName { get; set; }
            public DateTime startTime { get; set; }
            public DateTime startTimeUtc { get; set; }
            public bool isAllocatedSeating { get; set; }
            public string siteId { get; set; }
            public string cinemaChainId { get; set; }
            public List<object> attributes { get; set; }
            public string filmId { get; set; }
            public int seatsAvailable { get; set; }
            public DateTime lastUpdatedUtc { get; set; }
        }


        public class GroupedTicket
        {
            public string ticketTypeId { get; set; }
            public int quantity { get; set; }
            public string ticketTypeDescription { get; set; }
            public double priceEach { get; set; }
            public IList<int> ticketIds { get; set; }
            public bool isPackageTicket { get; set; }
        }

        public class Showtime
        {
            public string showtimeId { get; set; }
            public string siteId { get; set; }
            public IList<GroupedTicket> groupedTickets { get; set; }
            public IList<object> seats { get; set; }
            public bool seatsRequireSelection { get; set; }
        }

        public class OrderProperties
        {
            public string OrderId { get; set; }

        }

        public class GroupedTicket2
        {
            public string ticketTypeId { get; set; }
            public int quantity { get; set; }
            public string ticketTypeDescription { get; set; }
            public double priceEach { get; set; }
            public List<int> ticketIds { get; set; }
            public bool isPackageTicket { get; set; }
        }

        public class Showtime2
        {
            public string showtimeId { get; set; }
            public string siteId { get; set; }
            public List<GroupedTicket2> groupedTickets { get; set; }
            public List<object> seats { get; set; }
            public bool seatsRequireSelection { get; set; }
        }

        public class BookingRefObject
        {
            public string bookingId { get; set; }
        }

        public class RootObject2
        {
            public string id { get; set; }
            public string cinemaChainId { get; set; }
            public double totalPrice { get; set; }
            public double bookingFee { get; set; }
            public List<Showtime2> showtimes { get; set; }
            public DateTime expirationTimeUtc { get; set; }
        }
        public class TicketOrder
        {
            public string id { get; set; }
            public string cinemaChainId { get; set; }
            public double totalPrice { get; set; }
            public double bookingFee { get; set; }
            public IList<Showtime> showtimes { get; set; }
            public DateTime expirationTimeUtc { get; set; }
        }

        public class ShowtimeList2
        {
            public string title { get; set; }
            public string url { get; set; }
            public string plot { get; set; }
            public string showtimeId { get; set; }
            public string id { get; set; }
            public string youtube { get; set; }
            public string screenName { get; set; }
            public string startTime { get; set; }
            public string startDate { get; set; }
            public string startTimeUtc { get; set; }
            public bool isAllocatedSeating { get; set; }
            public string siteId { get; set; }
            public string cinemaChainId { get; set; }
            public List<object> attributes { get; set; }
            public string filmId { get; set; }
            public int seatsAvailable { get; set; }
            public DateTime lastUpdatedUtc { get; set; }
        }

        public class TicketType
        {
            public string id { get; set; }
            public string areaCategoryCode { get; set; }
            public string description { get; set; }
            public double price { get; set; }
            public bool isAllocatedSeating { get; set; }
        }

        public class TicketType2
        {
            public string id { get; set; }
            public string areaCategoryCode { get; set; }
            public string description { get; set; }
            public double price { get; set; }
            public bool isAllocatedSeating { get; set; }
        }

        public class Attribute
        {
            public string name { get; set; }
        }

        public class RootObject
        {
            public List<TicketType> ticketTypes { get; set; }
            public DateTime lastUpdatedTicketTypesUtc { get; set; }
            public string id { get; set; }
            public int screenId { get; set; }
            public string screenName { get; set; }
            public DateTime startTime { get; set; }
            public DateTime startTimeUtc { get; set; }
            public bool isAllocatedSeating { get; set; }
            public string siteId { get; set; }
            public string cinemaChainId { get; set; }
            public List<Attribute> attributes { get; set; }
            public string filmId { get; set; }
            public DateTime lastUpdatedUtc { get; set; }
        }
        //public async Task<List<ShowtimeList2>> ListOfShowtimes(string siteId)
        //{
        //    var list = new List<ShowtimeList2>();
        //    IEnumerable<System.Web.Mvc.SelectListItem> items = null;
        //    try
        //    {
        //        string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetShowTimes?siteId=" + siteId;
        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //        httpWebRequest.ContentType = "application/json;";
        //        httpWebRequest.Method = "GET";
        //        httpWebRequest.Accept = "application/json";
        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

        //        Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
        //        StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
        //        string result1 = string.Empty;
        //        result1 = responseStream.ReadToEnd();
        //        httpResponse.Close();
        //        string input = result1.Replace("\\", string.Empty);
        //        input = input.Trim('"');
        //        var obj = JsonConvert.DeserializeObject<List<ShowtimeList>>(input);

        //        if (obj != null)


        //            foreach (var item in obj.GroupBy(o => o.filmId).Select(g => g.First()).ToList())
        //            {
        //                var mv = new ShowtimeList2();
        //                var res = repoFilmHouseFilms.Get(o => o.id == item.filmId);
        //                if (res != null)
        //                {

        //                    mv.id = res.id + "_" + item.id;
        //                    mv.showtimeId = item.id;
        //                    mv.screenName = item.screenName;
        //                    mv.title = res.title;
        //                    mv.url = res.MovieUrl;
        //                    mv.plot = res.plot;
        //                    mv.youtube = res.mxfReleaseId;
        //                    string[] date = item.startTimeUtc != null ? item.startTimeUtc.ToString().Split('T') : null;
        //                    if (date != null)
        //                    {
        //                        mv.startDate = date[0] != null ? String.Format("{0:dddd, MMMM d, yyyy}", Convert.ToDateTime(date[0])) : null;
        //                        mv.startTime = date[0] != null ? Convert.ToDateTime(date[0]).ToString("hh:mm:ss tt", CultureInfo.CurrentCulture) : null;
        //                        mv.siteId = item.siteId;
        //                        mv.cinemaChainId = item.cinemaChainId;

        //                    }

        //                    list.Add(mv);


        //                }


        //            }
        //        {


        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {


        //    }

        //    return null;
        //}

        public List<ShowtimeList2> ListOfShowtimes(string siteId)
        {
            var list = new List<ShowtimeList2>();
            IEnumerable<System.Web.Mvc.SelectListItem> items = null;
            try
            {
                string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetShowTimes?siteId=" + siteId;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
                string result1 = string.Empty;
                result1 = responseStream.ReadToEnd();
                httpResponse.Close();
                string input = result1.Replace("\\", string.Empty);
                input = input.Trim('"');
                var obj = JsonConvert.DeserializeObject<List<ShowtimeList>>(input);

                if (obj != null)

                    //
                    foreach (var item in obj.GroupBy(o => o.filmId).Select(g => g.First()).ToList())
                    {
                        var mv = new ShowtimeList2();
                        var res = repoFilmHouseFilms.GetNonAsync(o => o.id == item.filmId);
                        if (res != null)
                        {

                            mv.id = res.id;
                            mv.showtimeId = item.id;
                            mv.screenName = item.screenName;
                            mv.title = res.title;
                            mv.url = res.MovieUrl;
                            mv.plot = res.plot;
                            mv.youtube = res.mxfReleaseId;
                            string[] date = item.startTimeUtc != null ? item.startTimeUtc.ToString().Split('T') : null;
                            if (date != null)
                            {
                                mv.startDate = date[0] != null ? String.Format("{0:dddd, MMMM d, yyyy}", Convert.ToDateTime(date[0])) : null;
                                mv.startTime = date[0] != null ? Convert.ToDateTime(date[0]).ToString("hh:mm:ss tt", CultureInfo.CurrentCulture) : null;
                                mv.siteId = item.siteId;
                                mv.cinemaChainId = item.cinemaChainId;

                            }

                            list.Add(mv);


                        }


                    }
                {


                }
                return list;
            }
            catch (Exception ex)
            {


            }

            return null;
        }


        public string GetFilmHousePrice(string price, int NoOfPersons)
        {
            string[] fprice = price.Split('_');
            if (fprice != null)
            {
                return FormattedAmount(Convert.ToDecimal(fprice[0].ToString()) * NoOfPersons);

            }
            return null;

        }
        public async Task<String> CompleteTransaction(string orderId, string name, string email, string phoneNo, string amount)
        {

            try
            {
                // 
                string url = "http://filmhouse.ticketplanet.ng/api/Transaction/SetCustomerDetails?cinemaChainId=" + "93cb8143-6264-e811-80c3-0004ffb07dad"
+ "&orderId=" + orderId + "&name=" + name + "&email=" + email + "&phoneNo=" + phoneNo;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
                string result1 = string.Empty;
                result1 = responseStream.ReadToEnd();
                httpResponse.Close();


                string url2 = "http://filmhouse.ticketplanet.ng/api/Transaction/CompleteTransaction?cinemaChainId=" + "93cb8143-6264-e811-80c3-0004ffb07dad"
+ "&orderId=" + orderId + "&amount=" + amount;
                var httpWebRequest2 = (HttpWebRequest)WebRequest.Create(url2);
                httpWebRequest2.ContentType = "application/json;";
                httpWebRequest2.Method = "GET";
                httpWebRequest2.Accept = "application/json";
                var httpResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();
                Encoding enc2 = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream2 = new StreamReader(httpResponse2.GetResponseStream(), enc2);
                string result2 = string.Empty;
                result2 = responseStream2.ReadToEnd();
                httpResponse.Close();
                string input = result2.Replace("\\", string.Empty);
                input = input.Trim('"');
                var obj = JsonConvert.DeserializeObject<BookingRefObject>(input);
                if (obj != null)
                {
                    return obj.bookingId;
                }

            }
            catch (Exception ex)
            {


            }

            return null;
        }
        public ReturnValues SaveTicketDetailsFm(TicketRequestModel ctReqest, string Reference)
        {
            var returnValues = new ReturnValues();
            var counter = repotk_BatchCounter.GetAllNonAsync().FirstOrDefault();
            try
            {
                string[] id = ctReqest.MovieDate.Split('_');


                var cinemaTranLog = new tk_CinemaTransactionLog();
                cinemaTranLog.DateCreated = DateTime.Now;
                cinemaTranLog.ContactEmail = ctReqest.email;
                cinemaTranLog.Units = ctReqest.NoOfPersons;
                cinemaTranLog.TransactionDate = DateTime.Now;
                cinemaTranLog.ReferenceNo = Reference;
                cinemaTranLog.Status = "PENDING";
                cinemaTranLog.ContactFullname = ctReqest.Fullname;
                cinemaTranLog.ContactPhoneNo = ctReqest.phoneNo;
                cinemaTranLog.TotalAmount = Convert.ToDecimal(ctReqest.Amount);
                cinemaTranLog.MovieDate = id[0];
                cinemaTranLog.MovieTime = ctReqest.MovieTime;
                cinemaTranLog.CinemaCompany = 3;
                cinemaTranLog.OrderId = ctReqest.orderId;
                cinemaTranLog.SiteId = ctReqest.siteId;
                cinemaTranLog.ShowtimeId = ctReqest.showtimeId;
                cinemaTranLog.ContactCategory = "Film House Via Ticket Planet";
                cinemaTranLog.MovieName = ctReqest.MovieName;
                cinemaTranLog.ViewType = ctReqest.cat;
                cinemaTranLog.IsEmailSent = "N";
                cinemaTranLog.Retry = 0;
                cinemaTranLog.IsCoupon = ctReqest.IsCoupon;

                repoCinemaTranLog.Add(cinemaTranLog);
                var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
                if (retV1)
                {


                    //Update batch Counter
                    counter.BatchNo = counter.BatchNo + 1;
                    repotk_BatchCounter.Update(counter);
                    var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                    returnValues.nErrorCode = 0;
                    returnValues.sErrorText = "Success";
                    return returnValues;

                }
                else
                {
                    returnValues.nErrorCode = -1;
                    returnValues.sErrorText = "FailedInsert";
                }
                return returnValues;

            }
            catch (Exception ex)
            {


            }
            return returnValues;

        }


        public async Task<int> UpdateTransLogs(string payStackReference, string bookingRef)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("", payStackReference);
            param.Add("", bookingRef);
            var result = await db.QueryAsync<int>(sql: "spContestantInsert",

                param: param, commandType: CommandType.StoredProcedure);

            return 1;


        }

        public async Task<string> UpdateBookingRef(string bookingRef, string payStackReference)
        {
            var ContactDetails = repoCinemaTranLog.GetManyNonAsync(o => o.PayStackReference == payStackReference);
            if (ContactDetails != null)
            {
                foreach (var item in ContactDetails)
                {
                    await UpdateTransLogs(payStackReference, bookingRef);
                }
            }
            return null;
        }

        public async Task<string> UpdatebookingRefFilmHouse(string payStackReference, string bookingRef)
        {
            var ContactDetails = repoCinemaTranLog.GetManyNonAsync(o => o.PayStackReference == payStackReference);
            if (ContactDetails != null)
            {
                foreach (var item in ContactDetails)
                {
                    await UpdateTransLogs(payStackReference, bookingRef);
                }
            }
            return null;
        }

        //public void UpdatebookingRefFilmHouse(string payStackReference, string bookingRef)
        //{

        //    var ContactDetails = repoCinemaTranLog.GetMany(o => o.PayStackReference == payStackReference);
        //    foreach (var item in ContactDetails)
        //    {
        //        item.Status = "SUCCESSFULL";
        //        item.BookingRef = bookingRef;
        //        repoCinemaTranLog.Update(item);
        //        var ret = unitOfWork.Commit(1) > 0 ? true : false;

        //    }

        //}

        public async Task<String> AddTicketToOrder(string id, string showtimeId, int quantity)
        {

            try
            {
                // 
                string url = "http://filmhouse.ticketplanet.ng/api/Transaction/TransactionOrder?cinemaChainId=" + "93cb8143-6264-e811-80c3-0004ffb07dad" + "&id=" + id + "&quantity=" + quantity + "&showtimeId=" + showtimeId;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
                string result1 = string.Empty;
                result1 = responseStream.ReadToEnd();
                httpResponse.Close();
                string input = result1.Replace("\\", string.Empty);
                input = input.Trim('"');
                var obj = JsonConvert.DeserializeObject<RootObject2>(input);
                if (obj != null)
                {
                    return obj.id;
                }

            }
            catch (Exception ex)
            {


            }

            return null;
        }
        public async Task<List<ShowtimeList2>> ListOfTime(string siteId, string movieDay, string filmId)
        {

            var list = new List<ShowtimeList2>();
            try
            {
                string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetShowTimes?siteId=" + siteId;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
                string result1 = string.Empty;
                result1 = responseStream.ReadToEnd();
                httpResponse.Close();
                string input = result1.Replace("\\", string.Empty);
                input = input.Trim('"');
                var obj = JsonConvert.DeserializeObject<List<ShowtimeList>>(input);

                if (obj != null)


                    foreach (var item in obj.Where(o => o.filmId == filmId).ToList())
                    {
                        var mv = new ShowtimeList2();
                        var res = repoFilmHouseFilms.GetNonAsync(o => o.id == item.filmId);
                        if (res != null)
                        {

                            mv.id = res.id;
                            mv.showtimeId = item.id;
                            mv.screenName = item.screenName;
                            mv.title = res.title;
                            mv.url = res.MovieUrl;
                            mv.plot = res.plot;
                            string[] date = item.startTime != null ? item.startTime.ToString().Split('T') : null;
                            if (date != null)
                            {
                                mv.startDate = date[0] != null ? String.Format("{0:dddd, MMMM d, yyyy}", Convert.ToDateTime(date[0])) : null;
                                mv.startTime = date[0] != null ? Convert.ToDateTime(date[0]).ToString("hh:mm:ss tt", CultureInfo.CurrentCulture) : null;
                                mv.siteId = item.siteId;
                                mv.cinemaChainId = item.cinemaChainId;

                            }

                            list.Add(mv);

                        }

                    }

                if (list != null)
                {
                    if (list.Count() > 0)
                    {

                        return list.Where(o => o.startDate == movieDay).GroupBy(test => test.startTime)
                                   .Select(grp => grp.First()).ToList();

                    }

                }


            }
            catch (Exception ex)
            {


            }

            return null;
        }
        public async Task<IEnumerable<SelectListItem>> ListOfMovieDays(string siteId)
        {
            var list = new List<ShowtimeList2>();
            IEnumerable<System.Web.Mvc.SelectListItem> items = null;
            try
            {
                string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetShowTimes?siteId=" + siteId;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
                StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
                string result1 = string.Empty;
                result1 = responseStream.ReadToEnd();
                httpResponse.Close();
                string input = result1.Replace("\\", string.Empty);
                input = input.Trim('"');
                var obj = JsonConvert.DeserializeObject<List<ShowtimeList>>(input);

                if (obj != null)


                    foreach (var item in obj)
                    {
                        var mv = new ShowtimeList2();
                        var res = repoFilmHouseFilms.GetNonAsync(o => o.id == item.filmId);
                        if (res != null)
                        {

                            mv.id = res.id;
                            mv.showtimeId = item.id;
                            mv.screenName = item.screenName;
                            mv.title = res.title;
                            mv.url = res.MovieUrl;
                            mv.plot = res.plot;
                            string[] date = item.startTimeUtc != null ? item.startTimeUtc.ToString().Split('T') : null;
                            if (date != null)
                            {
                                mv.startDate = date[0] != null ? String.Format("{0:dddd, MMMM d, yyyy}", Convert.ToDateTime(date[0])) : null;
                                mv.startTime = date[0] != null ? Convert.ToDateTime(date[0]).ToString("hh:mm:ss tt", CultureInfo.CurrentCulture) : null;
                                mv.siteId = item.siteId;
                                mv.cinemaChainId = item.cinemaChainId;

                            }

                            list.Add(mv);


                        }


                    }

                if (list != null)
                {
                    items = list.GroupBy(test => test.startDate)
                  .Select(grp => grp.First()).AsEnumerable()
                        .Select(p => new System.Web.Mvc.SelectListItem
                        {
                            Text = p.startDate,
                            Value = p.startDate + "_" + p.showtimeId
                        });


                }


                return items;
            }
            catch (Exception ex)
            {


            }

            return null;
        }

        public IEnumerable<SelectListItem> ListOfCinemas()
        {
            IEnumerable<System.Web.Mvc.SelectListItem> items = repoCinema.GetManyNonAsync(o => o.Status == "Active" && o.SiteId != null).AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.CinemaName.Contains("Filmhouse Twinwaters") ? "Filmhouse Oniru (Twinwaters)" : p.CinemaName,
                     Value = p.SiteId

                 });
            return items;
        }
        public ReturnValues SavePromoTransactions(string PromoCode, string Fullname, string phoneNo, string email, string NoOfPersons, string MovieCategory,
                string CinemaCompanyID, string MovieDay, string MovieTime, string MovieName)
        {
            DateTime dt = new DateTime();
            var returnValues = new ReturnValues();

            if (DateTime.TryParse(MovieDay, out dt))
            {
                if (dt > Convert.ToDateTime("2018-07-30"))
                {
                    returnValues.nErrorCode = -1;
                    returnValues.sErrorText = MovieDay + "  is Beyond our Awoof weekend Package that ends on the 30th July, 2019";
                    return returnValues;
                }


                var counter = repotk_BatchCounter.GetAllNonAsync().FirstOrDefault();
                try
                {

                    var cinemaTranLog = new tk_CinemaTransactionLog();
                    cinemaTranLog.DateCreated = DateTime.Now;
                    cinemaTranLog.ContactEmail = email;
                    cinemaTranLog.Units = Convert.ToInt32(NoOfPersons);
                    cinemaTranLog.TransactionDate = DateTime.Now;
                    cinemaTranLog.ReferenceNo = RefferenceGenerator.GenerateReference(GetCurrentCounter());
                    cinemaTranLog.Status = "SUCCESSFULL";
                    cinemaTranLog.ContactFullname = Fullname;
                    cinemaTranLog.ContactPhoneNo = phoneNo;
                    cinemaTranLog.TotalAmount = 1500;
                    cinemaTranLog.MovieDate = MovieDay;
                    cinemaTranLog.MovieTime = MovieTime;
                    cinemaTranLog.CinemaCompany = Convert.ToInt32(CinemaCompanyID);
                    cinemaTranLog.CinemaCompanyLocation = Convert.ToInt32(CinemaCompanyID);
                    cinemaTranLog.MovieName = MovieName;
                    cinemaTranLog.ViewType = MovieCategory;
                    cinemaTranLog.IsEmailSent = "N";
                    cinemaTranLog.Retry = 0;
                    cinemaTranLog.IsCoupon = "Y";


                    repoCinemaTranLog.Add(cinemaTranLog);
                    var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
                    if (retV1)
                    {
                        //Update batch Counter
                        counter.BatchNo = counter.BatchNo + 1;
                        repotk_BatchCounter.Update(counter);
                        var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                        returnValues.nErrorCode = 0;
                        returnValues.sErrorText = "Success";

                        // Insert Into PromoLog Table
                        var promoLog = new tk_PromoDayControlLog();
                        promoLog.DateCreated = DateTime.Now;
                        promoLog.ContactEmail = email;
                        promoLog.OriginalRef = cinemaTranLog.ReferenceNo;
                        promoLog.Units = Convert.ToInt32(NoOfPersons);
                        promoLog.TransactionDate = DateTime.Now;
                        promoLog.ReferenceNo = RefferenceGenerator.GenerateReference(GetCurrentCounter());
                        promoLog.Status = "SUCCESS";
                        promoLog.ContactFullname = Fullname;
                        promoLog.ContactPhoneNo = phoneNo;
                        promoLog.TotalAmount = 1500;
                        promoLog.MovieDate = MovieDay;
                        promoLog.MovieTime = MovieTime;
                        promoLog.CinemaCompany = Convert.ToInt32(CinemaCompanyID);
                        promoLog.CinemaCompanyLocation = Convert.ToInt32(CinemaCompanyID);
                        promoLog.MovieName = MovieName;
                        promoLog.ViewType = "Y";

                        repoPromoLog.Add(promoLog);
                        var retV3 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;



                        // Update Counter Again
                        var counter1 = repotk_BatchCounter.GetAllNonAsync().FirstOrDefault();
                        counter1.BatchNo = counter1.BatchNo + 1;
                        repotk_BatchCounter.Update(counter1);
                        var ret1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                        returnValues.TransactionRef = cinemaTranLog.ReferenceNo;
                        returnValues.origRef = PromoCode;
                        return returnValues;

                    }
                    else
                    {
                        returnValues.nErrorCode = -1;
                        returnValues.sErrorText = "FailedInsert";
                    }
                    return returnValues;

                }
                catch (Exception ex)
                {


                }
            }
            return returnValues;
        }


        public CouponObject ValidatePromoCode(string coupon)
        {
            var returnValues = new CouponObject();
            var rtv = repoPromoLog.GetNonAsync(o => o.ReferenceNo == coupon && o.ViewType == "N");
            if (rtv != null)
            {
                if (rtv.MovieName != null)
                {
                    if (rtv.MovieName.Contains("MISSION"))
                    {
                        returnValues.nErrorCode = -4;
                        returnValues.sErrorText = "Ticket Planet Awoof Cannot be Applied to MISSION IMPOSSIBLE 6, Kindly select any another Movie";
                        return returnValues;
                    }
                    else
                    {
                        returnValues.nErrorCode = 0;
                        returnValues.sErrorText = "Promo Code Applied Successfully. Kindly Proceed without Payment to Complete the Process";
                        returnValues.PromoCode = coupon;
                        return returnValues;

                    }
                }


            }

            returnValues.nErrorCode = -5;
            returnValues.sErrorText = "Voucher Code does not Exist";
            return returnValues;
        }

        public CouponObject ValidateCoupon(string coupon)
        {
            var returnValues = new CouponObject();
            var rtv = repoCoupon.GetNonAsync(o => o.CouponCode == coupon);
            if (rtv != null)
            {


                var rtn = repoCouponAssign.GetNonAsync(o => o.CouponCode == coupon);
                if (rtn != null)
                {


                    var setUp = repoCouponSetUp.GetNonAsync(null);
                    if (setUp != null)
                    {
                        if (setUp.TimeBased == "D")
                        {
                            if (DateTime.Now <= setUp.DateCreated.Value.AddDays(Convert.ToInt32(setUp.CutOffTime)))
                            {
                                if (setUp.CouponType == "P")
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + setUp.CouponValue + "%" + " has Successully Been Applied";
                                    returnValues.CouponId = rtv.Itbid;
                                    returnValues.CouponAssignId = rtn.Itbid;
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;
                                }
                                else
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + "N" + setUp.CouponValue + " has Successully Been Applied";
                                    returnValues.CouponId = rtv.Itbid;
                                    returnValues.CouponAssignId = rtn.Itbid;
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;


                                }
                            }
                        }
                        else if (setUp.TimeBased == "H")
                        {
                            if (DateTime.Now <= setUp.DateCreated.Value.AddHours(Convert.ToInt32(setUp.CutOffTime)))
                            {
                                if (setUp.CouponType == "P")
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + setUp.CouponValue + "%" + " has Successully Been Applied";
                                    returnValues.CouponId = rtv.Itbid;
                                    returnValues.CouponAssignId = rtn.Itbid;
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;
                                }
                                else
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + "N" + setUp.CouponValue + " has Successully Been Applied";
                                    returnValues.CouponId = rtv.Itbid;
                                    returnValues.CouponAssignId = rtn.Itbid;
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;


                                }
                            }


                        }

                        else if (setUp.TimeBased == "M")
                        {
                            if (DateTime.Now <= setUp.DateCreated.Value.AddMonths(Convert.ToInt32(setUp.CutOffTime)))
                            {
                                if (setUp.CouponType == "P")
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + setUp.CouponValue + "%" + " has Successully Been Applied";
                                    returnValues.CouponId = rtv.Itbid;
                                    returnValues.CouponAssignId = rtn.Itbid;
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;
                                }
                                else
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + "N" + setUp.CouponValue + " has Successully Been Applied";
                                    returnValues.CouponId = rtv.Itbid;
                                    returnValues.CouponAssignId = rtn.Itbid;
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;


                                }
                            }


                        }
                    }





                }




            }


            returnValues.nErrorCode = -5;
            returnValues.sErrorText = "Voucher Code does not Exist";
            return returnValues;
        }

        public MovieCategory getIs3DPalms(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3DPalms",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DPortHarcourt(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3DPortHarcourt",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DMaryLand(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3DMaryLand",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DAbuja(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3DAbuja",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DWarri(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3Warri",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DOwerri(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3Owerri",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DAjah(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3Ajah",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DAsaba(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3Asaba",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }
        public MovieCategory getIs3DGateWay(string filmCode)
        {
            try
            {


                DynamicParameters param = new DynamicParameters();
                param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCategory>(sql: "Isp_GetIs3Gateway",
                    param: param, commandType: CommandType.StoredProcedure);


                return result.FirstOrDefault();
            }
            catch (Exception)
            {


            }
            return null;


        }

        public IEnumerable<SelectListItem> ListofCinemaCategory(int cinemaID, string filmCode)
        {
            IEnumerable<System.Web.Mvc.SelectListItem> items = null;
            MovieCategory movie = new MovieCategory();
            List<MovieCategory> list = new List<MovieCategory>();

            if (cinemaID == 1)
            {
                var rtn = getIs3DPalms(filmCode);

                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {
                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });
                        }
                    }
                    else
                    {



                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });
                    }

                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });

                    return items;



                }
            }
            else if (cinemaID == 2)
            {
                var rtn = getIs3DPortHarcourt(filmCode);
                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {
                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });
                        }


                    }
                    else
                    {
                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });
                    }


                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });

                    return items;

                }

            }

            else if (cinemaID == 3)
            {
                var rtn = getIs3DMaryLand(filmCode);
                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });

                            //list.Add(new MovieCategory
                            //{
                            //    Category = "VIP 2D",
                            //    itbid = 2

                            //});

                        }



                    }
                    else
                    {


                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });
                    }

                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });

                    return items;
                }
            }


            else if (cinemaID == 4)
            {
                var rtn = getIs3DAbuja(filmCode);
                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });

                            //list.Add(new MovieCategory
                            //{
                            //    Category = "VIP 2D",
                            //    itbid = 2

                            //});


                        }


                    }
                    else
                    {



                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });

                    }

                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });
                }
                return items;

            }

            else if (cinemaID == 5)
            {
                var rtn = getIs3DWarri(filmCode);
                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });

                            //list.Add(new MovieCategory
                            //{
                            //    Category = "VIP 2D",
                            //    itbid = 2

                            //});


                        }


                    }
                    else
                    {

                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });

                    }
                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });
                }
                return items;

            }


            else if (cinemaID == 6)
            {
                var rtn = getIs3DOwerri(filmCode);
                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });

                            //list.Add(new MovieCategory
                            //{
                            //    Category = "VIP 2D",
                            //    itbid = 2

                            //});


                        }


                    }
                    else
                    {



                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });

                    }

                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });

                }
                return items;



            }


            else if (cinemaID == 7)
            {
                var rtn = getIs3DAjah(filmCode);
                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });

                            //list.Add(new MovieCategory
                            //{
                            //    Category = "VIP 2D",
                            //    itbid = 2

                            //});


                        }


                    }
                    else
                    {

                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });

                    }

                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });
                }
                return items;
            }

            else if (cinemaID == 8)
            {
                var rtn = getIs3DAsaba(filmCode);

                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {

                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });

                            //list.Add(new MovieCategory
                            //{
                            //    Category = "VIP 2D",
                            //    itbid = 2

                            //});


                        }


                    }
                    else
                    {



                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });

                    }

                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });
                }
                return items;
            }

            else if (cinemaID == 9)
            {
                var rtn = getIs3DGateWay(filmCode);
                if (rtn != null)
                {
                    if (rtn.Is3d == "N")
                    {
                        if (rtn.FilmTitle.Contains("Advance"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });

                        }
                        else if (rtn.FilmTitle.Contains("VIP"))
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "VIP",
                                itbid = 4
                            });
                        }
                        else
                        {

                            list.Add(new MovieCategory
                            {
                                Category = "Regular 2D (Ticket Only)",
                                itbid = 1

                            });
                            list.Add(new MovieCategory
                            {
                                Category = "Combo",
                                itbid = 2

                            });


                        }
                    }
                    else
                    {


                        list.Add(new MovieCategory
                        {
                            Category = "3D",
                            itbid = 3

                        });


                    }

                    items = list.AsEnumerable()
                          .Select(p => new System.Web.Mvc.SelectListItem
                          {
                              Text = p.Category,
                              Value = p.itbid.ToString()

                          });
                }
                return items;
            }
            else
            {
                list.Add(new MovieCategory
                {
                    Category = "Adult",
                    itbid = 1

                });

                //list.Add(new MovieCategory
                //{
                //    Category = "Children",
                //    itbid = 2

                //});

            }

            items = list.AsEnumerable()
                  .Select(p => new System.Web.Mvc.SelectListItem
                  {
                      Text = p.Category,
                      Value = p.itbid.ToString()

                  });

            return items;

        }

         

        public List<tk_Cinema> GetCinemaLocations(int cinemaID) 
        {
            if (cinemaID == 1)
            {
                return repoCinema.GetManyNonAsync(o => o.Status == "Active" && o.CinemaName.Contains("Genesis Delux") && o.SiteId == null).OrderBy(o=>o.UserId).ToList();

            }
            else if (cinemaID == 2)
            {
                return repoCinema.GetManyNonAsync(o => o.Status == "Active" && o.CinemaName.Contains("Marturion Cinema Igando") && o.SiteId == null).OrderBy(s => s.CinemaName).ToList();

            }
            else 
            {
                return repoCinema.GetManyNonAsync(o => o.Status == "Active" && o.SiteId != null).ToList();
            }
            
        }


        public IEnumerable<SelectListItem> ListOfCinemas(int cinemaID)
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoCinema.GetManyNonAsync(o => o.Status == "Active" && o.CinemaName.Contains("Genesis Delux") && o.SiteId == null).AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.CinemaName,
                     Value = p.Itbid.ToString()

                 });
            return items;
        }

        public IEnumerable<SelectListItem> ListOfMatCinemas(int cinemaID)
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoCinema.GetManyNonAsync(o => o.Status == "Active" && o.CinemaName.Contains("Marturion Cinema Igando") && o.SiteId == null).OrderBy(s => s.CinemaName).AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.CinemaName,
                     Value = p.Itbid.ToString()

                 });
            return items;
        }
        public class UrlContentModel
        {
            public string ShortFilmTitle { get; set; }
            public string FilmTitle { get; set; }
            public string ComingSoon { get; set; }
            public int CinemaId { get; set; }
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
            // public Img_1s Img_1s { get; set; }
            public string Digital { get; set; }
            public string Genre { get; set; }
            public string GenreCode { get; set; }
            public string StartDate { get; set; }
            public string Img_app { get; set; }
            public string IMDBCode { get; set; }


        }

        public class MovieCategory
        {
            public string Is3d { get; set; }
            public int itbid { get; set; }
            public string Category { get; set; }
            public string FilmTitle { get; set; }

        }

        //public async Task<IEnumerable<SelectListItem>> GetMovieDays(string filmCode, int CinemaId)
        // {
        //     List<UrlConcat> list = new List<UrlConcat>();
        //     List<UrlContentModel> Film = new List<UrlContentModel>();
        //     UrlContentModel urlCnt = new UrlContentModel();
        //     IEnumerable<System.Web.Mvc.SelectListItem> items = null;


        //     var urlContent = repoCinema.Get(o => o.Itbid == CinemaId).ApiUrl;



        //     //UrlContentModel urlCnt = new UrlContentModel();
        //     UrlContentModel2 urlCnt1 = new UrlContentModel2();
        //     List<UrlContentModel2> Performance = new List<UrlContentModel2>();
        //     try
        //     {

        //         if (CinemaId == 1)
        //         {

        //             var palms = repoGenesisFilm.GetMany(k => k.CinemaId == CinemaId && k.FilmCode.Trim() == filmCode);


        //             var selectList = palms.GroupBy(x => x.PerformDate)
        //                    .Select(g => g.First()).AsEnumerable();

        //                 items = selectList
        //               .Select(p => new System.Web.Mvc.SelectListItem
        //               {
        //                   Text = p.PerformDate,
        //                   Value = p.PerformDate

        //               });
        //         }




        //         return items;



        //     }
        //     catch (Exception e)
        //     {

        //     }


        //     return items;



        // }

        public List<UrlConcat> getPalmsDates(string filmCode)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetPalmsMovieDate",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> getAbujaDates(string filmCode)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetAbujaMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();


        }
        public List<UrlConcat> getAjahDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetAjahMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();



        }


        public List<UrlConcat> getAsabaDates(string filmCode)
        {



            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetAsabaMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();




        }
        public List<UrlConcat> getAkureDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getAkureDates",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();

        }
        public List<UrlConcat> getKanoDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getKanoDates",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();

        }
        public List<UrlConcat> getBeninDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getBeninDates",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();

        }
        public List<UrlConcat> getOniruDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getOniruDates",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();

        }
        public List<UrlConcat> getAdeniranDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getAdeniranDates",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();

        }
        public List<UrlConcat> getDugbeDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getDugbeDates",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();

        }
        public List<UrlConcat> getSamondaDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getSamondaDates",
                param: param, commandType: CommandType.StoredProcedure);


            return result.ToList();

        }

        public List<UrlConcat> getSurulereDates(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "getSurulereDates",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();





        }


        public List<UrlConcat> getGatewayDates(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetgateWayMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();





        }
        public List<UrlConcat> getMaryLandDates(string filmCode)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetMaryLandMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();


        }
        public List<UrlConcat> getOwerriLandDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetOwerriMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();




        }

        public List<UrlConcat> getPHDates(string filmCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetPHMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();




        }

        public List<UrlConcat> getWarriDates(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@filmCode", filmCode);

            var result = db.Query<UrlConcat>(sql: "Isp_GetWarriMovieDate",
                param: param, commandType: CommandType.StoredProcedure);




            return result.ToList();




        }



        public MovieNameObj getPalmsMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNamePalms",
                param: param, commandType: CommandType.StoredProcedure);

            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            o.imgBanner = result.FirstOrDefault().imgBanner;
            return o;


        }


        public MovieNameObj getPHMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNamePH",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getMaryLandMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameMaryLand",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getAbujaMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameAbj",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getWarriMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameWarri",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getOwerriMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameOwerri",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }
        public MovieNameObj getAjahMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameAjah",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getAsabaMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameAsaba",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }
        public MovieNameObj getBeninMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameBenin",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }
        public MovieNameObj getOniruMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameOniru",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }
        public MovieNameObj getDugbeMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameDugbe",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getAdeniranMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameAdeniran",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getSamondaMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameSamonda",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }
        public MovieNameObj getSurulereMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameSurulere",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }
        public MovieNameObj getAkureMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameAkure",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }
        public MovieNameObj getKanoMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameKano",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public MovieNameObj getGetwayMovieName(string filmCode)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@code", filmCode);

            var result = db.Query<MovieNameObj>(sql: "Isp_GetMovieNameGateway",
                param: param, commandType: CommandType.StoredProcedure);


            MovieNameObj o = new MovieNameObj();
            o.FilmTitle = result.FirstOrDefault().FilmTitle;
            o.Synopsis = result.FirstOrDefault().Synopsis;
            o.Youtube = result.FirstOrDefault().Youtube;
            return o;


        }

        public List<MovieTimeObj> getPalmsMovieTime(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DateTime dt;
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);


            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimePalms",
                param: param, commandType: CommandType.StoredProcedure);

            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))

                foreach (var item in result)
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    if (!string.IsNullOrEmpty(item.StartTime))
                    {

                        DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);


                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                        time.Add(Mtime);

                    }
                }

            return time.ToList(); //ToList();


        }
        public List<MovieTimeObj> getPalmsMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();

            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimePalms",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {

                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;
                    var hourTime = DateTime.Now.Hour;
                    var currentMinute = DateTime.UtcNow.Minute;
                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour

                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;
                    var itemMinute = TimeSpan.Parse(item.StartTime).Minutes;
                    //TimeSpan start = new TimeSpan(10, 0, 0);
                    //currentHour += 1;

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);

                    }

                }
            }
            return time.ToList();
        }


        public ReturnValues checkPalmsMovieTimeToday(string PerformDate, string MovieDay)
        {
            ReturnValues rtv = new ReturnValues();

            //string times = PerformDate.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
            string val = PerformDate.Remove(9);

            // DateTime dateTime = DateTime.ParseExact(val, "HH:mm:ss",
            //CultureInfo.InvariantCulture);

            var itemUtc = DateTime.UtcNow.ToString("hh:mm:ss");
            var c = DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
            //get the current hour of today
            //var currentHourUtc = DateTime.UtcNow.Hour;
            //var currentHour = DateTime.Now.Hour;
            //var currentMinute = DateTime.Now.Minute;
            //var currentMinuteUtc = DateTime.UtcNow.Minute;

            var itemHour = TimeSpan.Parse(val).Hours;
            var itemMinute = TimeSpan.Parse(val).Minutes;
            var currentHour = TimeSpan.Parse(itemUtc).Hours;
            var currentMinute = DateTime.UtcNow.Minute;

            var time = DateTime.Now.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

            currentHour += 1;
            var tt = DateTime.UtcNow.ToString("hh:mm:ss");

            DateTime dt = new DateTime();
            DateTime.TryParse(MovieDay, out dt);
            string movDate = String.Format("{0:yyyy-MM-dd}", dt);
            DateTime ft = DateTime.UtcNow;

            //DateTime.TryParse(ft.ToString(), out today);

            string todaysDate = String.Format("{0:yyyy-MM-dd}", ft);

            if (todaysDate == movDate)
            {
                if (currentHour.Equals(itemHour))
                {
                    if (itemMinute < currentMinute)
                    {
                        rtv.nErrorCode = 0;
                        rtv.sErrorText = "The current Movie time has expired";
                        rtv.TransactionRef = tt;
                        rtv.origRef = time;
                        rtv.UserId = currentHour;
                        return rtv;
                    }
                }
            }



            return null;
        }
        public List<MovieTimeObj> getAbujaMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAbuja",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();


        }

        public List<MovieTimeObj> getAbujaMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAbuja",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }

        public List<MovieTimeObj> getAjahMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAjah",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();


        }

        public List<MovieTimeObj> getAjahMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAjah",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }
                }
            }

            return time.ToList();
        }


        public List<MovieTimeObj> getAsabaMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAsaba",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();


        }

        public List<MovieTimeObj> getAsabaMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAsaba",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }

        public List<MovieTimeObj> getGatewayMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeGateway",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();


        }

        public List<MovieTimeObj> getGatewayMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeGateway",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }

        public List<MovieTimeObj> getMaryLandMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeMaryLand",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();




        }

        public List<MovieTimeObj> getMaryLandMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeMaryLand",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {

                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }

        public List<MovieTimeObj> getOwerriMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeMaryOwerri",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getOwerriMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeMaryOwerri",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour


                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }


        public List<MovieTimeObj> getPHMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimePH",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getPHMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimePH",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }


        public List<MovieTimeObj> getWarriMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeWarri",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getWarriMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeWarri",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }


        public List<MovieTimeObj> getSurulereMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeSurulere",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getSalmondaMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeSalmonda",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getSalmondaMovieTimeToday(string PerformDate, string filmCode)
        {
            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeSalmonda",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                //MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    MovieTimeObj Mtime = new MovieTimeObj();
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                            CultureInfo.InvariantCulture);

                    //get the current hour of today
                    var currentHour = DateTime.UtcNow.Hour;

                    //var ch = string.Format("hh:mm:ss", datet);
                    //the item hour
                    var itemHour = TimeSpan.Parse(item.StartTime).Hours;

                    //TimeSpan start = new TimeSpan(10, 0, 0);

                    var c = DateTime.UtcNow.ToString("hh:mm:ss");

                    //check if the current hour is lesser than the item hour
                    if (currentHour < itemHour)
                    {
                        Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);
                        time.Add(Mtime);
                    }

                }
            }

            return time.ToList();
        }

        public List<MovieTimeObj> getAdeniranMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAdeniran",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getDugbeMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeDugbe",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getOniruMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeOniru",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getBeninMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeBenin",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getKanoMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeKano",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }

        public List<MovieTimeObj> getAkureMovieTime(string PerformDate, string filmCode)
        {

            List<MovieTimeObj> time = new List<MovieTimeObj>();
            DynamicParameters param = new DynamicParameters();
            param.Add("@PerformDate", PerformDate);
            param.Add("@FilmCode", filmCode);

            var result = db.Query<MovieTimeObj>(sql: "Isp_GetMovieTimeAkure",
                param: param, commandType: CommandType.StoredProcedure);


            foreach (var item in result)
            {
                MovieTimeObj Mtime = new MovieTimeObj();
                if (!string.IsNullOrEmpty(item.StartTime))
                {
                    DateTime dateTime = DateTime.ParseExact(item.StartTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);


                    Mtime.StartTime = dateTime.ToString("hh:mm:ss tt", CultureInfo.CurrentCulture);

                    time.Add(Mtime);

                }
            }

            return time.ToList(); //ToList();

        }


        public List<UrlConcat> RaiseGetPHFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetPHFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> RaiseGetPalmsFilmsPerformance()
        {
            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetPalmsFilmAndPeformance",
                commandType: CommandType.StoredProcedure);

            return result.ToList();


        }

        public List<UrlConcat> RaiseGetMaryLandFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetMaryLandFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }
        public List<UrlConcat> RaiseGetAbujaFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetAbujaFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> RaiseGetWarriFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetWarriFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> RaiseGetOwerriFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetOwerriFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> RaiseGetAjahFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetAjahFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> RaiseGetAsabaFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetAsabaFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }
        public List<UrlConcat> RaiseGetFilmHouseDugbeFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetFilmHouseDugbeFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }
        public List<UrlConcat> RaiseGetFilmHouseOniruFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetFilmHouseOniruFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }
        public List<UrlConcat> RaiseGetFilmHouseBeninFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetFilmHouseBeninFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }
        public List<UrlConcat> RaiseGetFilmHouseAkureFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetFilmHouseAkureFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> RaiseGetFilmHouseKanoFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetFilmHouseKanoFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }


        public List<UrlConcat> RaiseGetFilmHouseAdeniranFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetFilmHouseAdeniranFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }
        public List<UrlConcat> RaiseGetFilmHouseSamondaFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetFilmHouseSamondaFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public List<UrlConcat> RaiseOtherCinemaFilmsPerform(int cinemaId)
        {


            DynamicParameters param = new DynamicParameters();
            param.Add("@cinemaId", cinemaId);

            var result = db.Query<UrlConcat>(sql: "Isp_GetOtherFilmAndPeformance",
               param: param, commandType: CommandType.StoredProcedure);

            return result.ToList();


        }

        public List<UrlConcat> RaiseGetGatewayFilmsPerformance()
        {


            DynamicParameters param = new DynamicParameters();

            var result = db.Query<UrlConcat>(sql: "Isp_GetGatewayFilmAndPeformance",
                commandType: CommandType.StoredProcedure);


            return result.ToList();


        }

        public string GetCinemaName(int CinemaID)
        {

            return repoCinema.GetNonAsync(o => o.Itbid == CinemaID).CinemaName;

        }

        public string GetCinemaSynopsis(string filmCode)
        {
            var sys = repoPalmsfilm.GetNonAsync(o => o.Code == filmCode);
            return sys.Synopsis;

        }

        public string GetIspromoDay(string filmCode, int cinemaId)
        {
            MoviesModelList md = new MoviesModelList();

            if (cinemaId == 1)
            {
                var sys = repoGenesisFilm.GetNonAsync(o => o.Code == filmCode).ispromoDay;
                return md.ispromoDay = sys;
            }
            else if (cinemaId == 2)
            {
                var sys = repoPhFilms.GetNonAsync(o => o.Code == filmCode).ispromoDay;
                return md.ispromoDay = sys;
            }
            else if (cinemaId == 3)
            {
                var sys = repoMaryLandFilm.GetNonAsync(o => o.Code == filmCode);
                return md.ispromoDay = sys.ispromoDay;
            }
            else if (cinemaId == 4)
            {
                var sys = repoAbujafilm.GetNonAsync(o => o.Code == filmCode);
                return md.ispromoDay = sys.ispromoDay;
            }
            else if (cinemaId == 5)
            {
                var sys = repoWarriFilms.GetNonAsync(o => o.Code == filmCode);
                return md.ispromoDay = sys.ispromoDay;
            }
            else if (cinemaId == 6)
            {
                var sys = repoOwerriFilm.GetNonAsync(o => o.Code == filmCode);
                return md.ispromoDay = sys.ispromoDay;
            }
            else if (cinemaId == 7)
            {
                var sys = repoAjahFilm.GetNonAsync(o => o.Code == filmCode);
                return md.ispromoDay = sys.ispromoDay;
            }
            else if (cinemaId == 8)
            {
                var sys = repoAsabaFilm.GetNonAsync(o => o.Code == filmCode);
                return md.ispromoDay = sys.ispromoDay;
            }
            else if (cinemaId == 9)
            {
                var sys = repoAjahFilm.GetNonAsync(o => o.Code == filmCode);
                return md.ispromoDay = sys.ispromoDay;
            }
            //sys = repoPalmsfilm.Get(o => o.Code == filmCode);
            //return sys.ispromoDay;
            return "";
        }


        public string getMovieName(string filmCode, int CinemaId)
        {
            MovieNameObj o = new MovieNameObj();
            string movieName = "";
            if (CinemaId == 1)
            {


                movieName = getPalmsMovieName(filmCode).FilmTitle;



            }
            else if (CinemaId == 2)
            {

                movieName = getPHMovieName(filmCode).FilmTitle; ;



            }
            else if (CinemaId == 3)
            {


                movieName = getMaryLandMovieName(filmCode).FilmTitle; ;

            }
            else if (CinemaId == 4)
            {

                movieName = getAbujaMovieName(filmCode).FilmTitle;


            }
            else if (CinemaId == 5)
            {

                movieName = getWarriMovieName(filmCode).FilmTitle; ;

            }
            else if (CinemaId == 6)
            {
                movieName = getOwerriMovieName(filmCode).FilmTitle; ;

            }
            else if (CinemaId == 7)
            {

                movieName = getAjahMovieName(filmCode).FilmTitle; ;



            }

            else if (CinemaId == 8)
            {
                movieName = getAsabaMovieName(filmCode).FilmTitle; ;

            }

            else if (CinemaId == 9)
            {

                movieName = getGetwayMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 10)
            {

                movieName = getSurulereMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 11)
            {

                movieName = getSamondaMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 12)
            {

                movieName = getAdeniranMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 13)
            {

                movieName = getDugbeMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 14)
            {

                movieName = getOniruMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 15)
            {

                movieName = getBeninMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 16)
            {

                movieName = getKanoMovieName(filmCode).FilmTitle;

            }
            else if (CinemaId == 17)
            {

                movieName = getAkureMovieName(filmCode).FilmTitle;

            }

            return movieName;


        }

        public string getMovieBanner(string filmCode, int CinemaId)
        {
            MovieNameObj o = new MovieNameObj();
            string movieBanner = "";
            if (CinemaId == 1)
            {
                movieBanner = repoGenesisFilm.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 2)
            {
                movieBanner = repoPhFilms.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 3)
            {
                movieBanner = repoMaryLandFilm.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 4)
            {
                movieBanner = repoAbujafilm.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 5)
            {
                movieBanner = repoWarriFilms.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 6)
            {
                movieBanner = repoOwerriFilm.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 7)
            {
                movieBanner = repoAjahFilm.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 8)
            {
                movieBanner = repoAsabaFilm.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 9)
            {
                movieBanner = repoGatewayFilm.GetNonAsync(x => x.Code == filmCode).Img_title;
            }
            else if (CinemaId == 11)
            {
                movieBanner = repoOtherFilms.GetNonAsync(x => x.Code == filmCode).Img_title;
            }

            return movieBanner;
        }
        public string GetMovieSynopsis(string filmCode, int CinemaId)
        {
            MovieNameObj o = new MovieNameObj();
            string movieName = "";
            if (CinemaId == 1)
            {


                movieName = getPalmsMovieName(filmCode).Synopsis;



            }
            else if (CinemaId == 2)
            {

                movieName = getPHMovieName(filmCode).Synopsis; ;



            }
            else if (CinemaId == 3)
            {


                movieName = getMaryLandMovieName(filmCode).Synopsis; ;

            }
            else if (CinemaId == 4)
            {

                movieName = getAbujaMovieName(filmCode).Synopsis;


            }
            else if (CinemaId == 5)
            {

                movieName = getWarriMovieName(filmCode).Synopsis; ;

            }
            else if (CinemaId == 6)
            {
                movieName = getOwerriMovieName(filmCode).Synopsis; ;

            }
            else if (CinemaId == 7)
            {

                movieName = getAjahMovieName(filmCode).Synopsis; ;



            }

            else if (CinemaId == 8)
            {
                movieName = getAsabaMovieName(filmCode).Synopsis; ;

            }

            else if (CinemaId == 9)
            {

                movieName = getGetwayMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 10)
            {

                movieName = getSurulereMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 11)
            {

                movieName = getSamondaMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 12)
            {

                movieName = getAdeniranMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 13)
            {

                movieName = getDugbeMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 14)
            {

                movieName = getOniruMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 15)
            {

                movieName = getBeninMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 16)
            {

                movieName = getKanoMovieName(filmCode).Synopsis;

            }
            else if (CinemaId == 17)
            {

                movieName = getAkureMovieName(filmCode).Synopsis;

            }

            return movieName;


        }


        public string GetYouTubeLink(string filmCode, int CinemaId)
        {
            MovieNameObj o = new MovieNameObj();
            string movieName = "";
            if (CinemaId == 1)
            {


                movieName = getPalmsMovieName(filmCode).Youtube;



            }
            else if (CinemaId == 2)
            {

                movieName = getPHMovieName(filmCode).Youtube; ;



            }
            else if (CinemaId == 3)
            {


                movieName = getMaryLandMovieName(filmCode).Youtube; ;

            }
            else if (CinemaId == 4)
            {

                movieName = getAbujaMovieName(filmCode).Youtube;


            }
            else if (CinemaId == 5)
            {

                movieName = getWarriMovieName(filmCode).Youtube; ;

            }
            else if (CinemaId == 6)
            {
                movieName = getOwerriMovieName(filmCode).Youtube; ;

            }
            else if (CinemaId == 7)
            {

                movieName = getAjahMovieName(filmCode).Youtube; ;



            }

            else if (CinemaId == 8)
            {
                movieName = getAsabaMovieName(filmCode).Youtube; ;

            }

            else if (CinemaId == 9)
            {

                movieName = getGetwayMovieName(filmCode).Youtube;

            }

            else if (CinemaId == 10)
            {

                movieName = getSurulereMovieName(filmCode).Youtube;

            }
            else if (CinemaId == 11)
            {

                movieName = getSamondaMovieName(filmCode).Youtube;

            }
            else if (CinemaId == 12)
            {

                movieName = getAdeniranMovieName(filmCode).Youtube;

            }
            else if (CinemaId == 13)
            {

                movieName = getDugbeMovieName(filmCode).Youtube;

            }
            else if (CinemaId == 14)
            {

                movieName = getOniruMovieName(filmCode).Youtube;

            }
            else if (CinemaId == 15)
            {

                movieName = getBeninMovieName(filmCode).Youtube;

            }
            else if (CinemaId == 16)
            {

                movieName = getKanoMovieName(filmCode).Youtube;

            }
            else if (CinemaId == 17)
            {

                movieName = getAkureMovieName(filmCode).Youtube;

            }

            return movieName;


        }

        public async Task<IEnumerable<SelectListItem>> GetMovieDays(string filmCode, int CinemaId)
        {
            List<UrlConcat> list = new List<UrlConcat>();
            List<UrlContentModel> Film = new List<UrlContentModel>();
            UrlContentModel urlCnt = new UrlContentModel();
            IEnumerable<System.Web.Mvc.SelectListItem> items = null;


            var urlContent = repoCinema.GetNonAsync(o => o.Itbid == CinemaId).ApiUrl;



            //UrlContentModel urlCnt = new UrlContentModel();
            UrlContentModel2 urlCnt1 = new UrlContentModel2();
            List<UrlContentModel2> Performance = new List<UrlContentModel2>();
            try
            {
                DateTime dt;

                if (CinemaId == 1)
                {


                
                    items = getPalmsDates(filmCode)
                  .Select(p => new System.Web.Mvc.SelectListItem
                  {
                      Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                      Value = p.PerformDate

                  });


                }
                else if (CinemaId == 2)
                {

                    items = getPHDates(filmCode)
                  .Select(p => new System.Web.Mvc.SelectListItem
                  {
                      Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                      Value = p.PerformDate

                  });


                }
                else if (CinemaId == 3)
                {

                    items = getMaryLandDates(filmCode)
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                     Value = p.PerformDate

                 });

                }
                else if (CinemaId == 4)
                {
                    items = getAbujaDates(filmCode)
                                 .Select(p => new System.Web.Mvc.SelectListItem
                                 {
                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                     Value = p.PerformDate

                                 });





                }
                else if (CinemaId == 5)
                {

                    items = getWarriDates(filmCode)
                                  .Select(p => new System.Web.Mvc.SelectListItem
                                  {
                                      Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                      Value = p.PerformDate

                                  });


                }
                else if (CinemaId == 6)
                {
                    items = getOwerriLandDates(filmCode)
                               .Select(p => new System.Web.Mvc.SelectListItem
                               {
                                   Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                   Value = p.PerformDate

                               });

                }
                else if (CinemaId == 7)
                {

                    items = getAjahDates(filmCode)
                                                  .Select(p => new System.Web.Mvc.SelectListItem
                                                  {
                                                      Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                      Value = p.PerformDate

                                                  });


                }

                else if (CinemaId == 8)
                {

                    items = getAsabaDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }

                else if (CinemaId == 9)
                {


                    items = getGatewayDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }
                else if (CinemaId == 10)
                {


                    items = getSurulereDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }
                else if (CinemaId == 11)
                {


                    items = getSamondaDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }
                else if (CinemaId == 12)
                {


                    items = getAdeniranDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }
                else if (CinemaId == 13)
                {


                    items = getDugbeDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }

                else if (CinemaId == 14)
                {


                    items = getOniruDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }
                else if (CinemaId == 15)
                {


                    items = getBeninDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }
                else if (CinemaId == 16)
                {


                    items = getKanoDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }
                else if (CinemaId == 17)
                {


                    items = getAkureDates(filmCode)
                                                 .Select(p => new System.Web.Mvc.SelectListItem
                                                 {
                                                     Text = DateTime.TryParse(p.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : p.PerformDate,
                                                     Value = p.PerformDate

                                                 });

                }



                return items;



            }
            catch (Exception e)
            {

            }


            return items;

        }

        public async Task<List<MovieTimeObj>> GetMovieTimeByToday(string MovieDate, int CinemaId, string FilmCode)
        {
            // List<UrlConcat> list = new List<UrlConcat>();
            List<MovieTimeObj> time = new List<MovieTimeObj>();

            List<tk_GenesisPalmsPerformance> list = new List<tk_GenesisPalmsPerformance>();

            var urlContent = repoCinema.GetNonAsync(o => o.Itbid == CinemaId).ApiUrl;



            //UrlContentModel urlCnt = new UrlContentModel();
            UrlContentModel2 urlCnt1 = new UrlContentModel2();
            List<UrlContentModel2> Performance = new List<UrlContentModel2>();
            try
            {

                DateTime dt;
                if (CinemaId == 1)
                {
                    time = getPalmsMovieTimeToday(MovieDate, FilmCode);
                    //TimeSpan times = new TimeSpan();
                }
                else if (CinemaId == 2)
                {

                    time = getPHMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList();


                }
                else if (CinemaId == 3)
                {

                    time = getMaryLandMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;
                }
                else if (CinemaId == 4)
                {
                    time = getAbujaMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;


                }
                else if (CinemaId == 5)
                {

                    time = getWarriMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 6)
                {
                    time = getOwerriMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 7)
                {

                    time = getAjahMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 8)
                {

                    time = getAsabaMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 9)
                {


                    time = getGatewayMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 10)
                {


                    time = getSurulereMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 11)
                {


                    time = getSalmondaMovieTimeToday(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 12)
                {


                    time = getAdeniranMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 13)
                {


                    time = getDugbeMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 14)
                {


                    time = getOniruMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 15)
                {


                    time = getBeninMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 16)
                {


                    time = getKanoMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 17)
                {


                    time = getAkureMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }

                // return time;

                return time;

            }
            catch (Exception ex)
            {

            }

            return time;

        }

        public async Task<List<MovieTimeObj>> GetMovieTime(string MovieDate, int CinemaId, string FilmCode)
        {
            // List<UrlConcat> list = new List<UrlConcat>();
            List<MovieTimeObj> time = new List<MovieTimeObj>();

            List<tk_GenesisPalmsPerformance> list = new List<tk_GenesisPalmsPerformance>();

            var urlContent = repoCinema.GetNonAsync(o => o.Itbid == CinemaId).ApiUrl;



            //UrlContentModel urlCnt = new UrlContentModel();
            UrlContentModel2 urlCnt1 = new UrlContentModel2();
            List<UrlContentModel2> Performance = new List<UrlContentModel2>();
            try
            {

                DateTime dt;
                if (CinemaId == 1)
                {


                    time = getPalmsMovieTime(MovieDate, FilmCode);

                }
                else if (CinemaId == 2)
                {

                    time = getPHMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList();


                }
                else if (CinemaId == 3)
                {

                    time = getMaryLandMovieTime(MovieDate, FilmCode).OrderBy(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;
                }
                else if (CinemaId == 4)
                {
                    time = getAbujaMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;


                }
                else if (CinemaId == 5)
                {

                    time = getWarriMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 6)
                {
                    time = getOwerriMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 7)
                {

                    time = getAjahMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }

                else if (CinemaId == 8)
                {

                    time = getAsabaMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }

                else if (CinemaId == 9)
                {


                    time = getGatewayMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 10)
                {


                    time = getSurulereMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 11)
                {


                    time = getSalmondaMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 12)
                {


                    time = getAdeniranMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 13)
                {


                    time = getDugbeMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 14)
                {


                    time = getOniruMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }

                else if (CinemaId == 15)
                {


                    time = getBeninMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }

                else if (CinemaId == 16)
                {


                    time = getKanoMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }
                else if (CinemaId == 17)
                {


                    time = getAkureMovieTime(MovieDate, FilmCode).OrderByDescending(x => DateTime.TryParse(x.StartTime, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); ;

                }


                // return time;




                return time;



            }
            catch (Exception e)
            {

            }


            return time;



        }

        public async Task<string> Get3DStatus(string filmCode, int CinemaId)
        {
            string ThreeDMovie = "";
            List<UrlConcat> list = new List<UrlConcat>();
            List<UrlContentModel> Film = new List<UrlContentModel>();
            UrlContentModel urlCnt = new UrlContentModel();

            var urlContent = repoCinema.GetNonAsync(o => o.Itbid == CinemaId).ApiUrl;



            //UrlContentModel urlCnt = new UrlContentModel();
            UrlContentModel2 urlCnt1 = new UrlContentModel2();
            List<UrlContentModel2> Performance = new List<UrlContentModel2>();
            try
            {
                var http = new HttpClient();
                var response = await http.GetAsync(urlContent);
                var result = await response.Content.ReadAsStringAsync();

                XDocument xDoc = XDocument.Load(new StringReader(result));
                XDocument xDoc1 = XDocument.Load(new StringReader(result));



                var decendats = xDoc.Descendants("Film");

                var s = from c in decendats
                        select new UrlContentModel
                        {
                            ShortFilmTitle = string.IsNullOrWhiteSpace(c.Element("ShortFilmTitle").Value) ? "" : c.Element("ShortFilmTitle").Value,
                            FilmTitle = string.IsNullOrWhiteSpace(c.Element("FilmTitle").Value) ? "" : c.Element("FilmTitle").Value,
                            ComingSoon = string.IsNullOrWhiteSpace(c.Element("ComingSoon").Value) ? "" : c.Element("ComingSoon").Value,
                            Code = string.IsNullOrWhiteSpace(c.Element("Code").Value) ? "" : c.Element("Code").Value,
                            Certificate = string.IsNullOrWhiteSpace(c.Element("Certificate").Value) ? "" : c.Element("Certificate").Value,
                            Is3d = string.IsNullOrWhiteSpace(c.Element("Is3d").Value) ? "" : c.Element("Is3d").Value,
                            Img_title = string.IsNullOrWhiteSpace(c.Element("Img_title").Value) ? "" : c.Element("Img_title").Value,
                            Rentrak = string.IsNullOrWhiteSpace(c.Element("Rentrak").Value) ? "" : c.Element("Rentrak").Value,
                            Youtube = string.IsNullOrWhiteSpace(c.Element("Youtube").Value) ? "" : c.Element("Youtube").Value,
                            ReleaseDate = string.IsNullOrWhiteSpace(c.Element("ReleaseDate").Value) ? "" : c.Element("ReleaseDate").Value,
                            RunningTime = string.IsNullOrWhiteSpace(c.Element("RunningTime").Value) ? "" : c.Element("RunningTime").Value,
                            Synopsis = string.IsNullOrWhiteSpace(c.Element("Synopsis").Value) ? "" : c.Element("Synopsis").Value,
                            Certificate_desc = string.IsNullOrWhiteSpace(c.Element("Certificate_desc").Value) ? "" : c.Element("Certificate_desc").Value,
                            Img_1s = string.IsNullOrWhiteSpace(c.Element("Img_1s").Value) ? "" : c.Element("Img_1s").Value,
                            Digital = string.IsNullOrWhiteSpace(c.Element("Digital").Value) ? "" : c.Element("Digital").Value,
                            Genre = string.IsNullOrWhiteSpace(c.Element("Genre").Value) ? "" : c.Element("Genre").Value,
                            GenreCode = string.IsNullOrWhiteSpace(c.Element("GenreCode").Value) ? "" : c.Element("GenreCode").Value,
                            StartDate = string.IsNullOrWhiteSpace(c.Element("StartDate").Value) ? "" : c.Element("StartDate").Value,
                            IMDBCode = string.IsNullOrWhiteSpace(c.Element("IMDBCode").Value) ? "" : c.Element("IMDBCode").Value,

                        };
                foreach (var c in s)
                {
                    Film.Add(new UrlContentModel
                    {
                        ShortFilmTitle = c.ShortFilmTitle,
                        FilmTitle = c.FilmTitle,
                        ComingSoon = c.ComingSoon,
                        Code = c.Code,
                        Certificate = c.Certificate,
                        Is3d = c.Is3d,
                        Img_title = c.Img_title,
                        Rentrak = c.Rentrak,
                        Youtube = c.Youtube,
                        ReleaseDate = c.ReleaseDate,
                        RunningTime = c.RunningTime,
                        Synopsis = c.Synopsis,
                        Certificate_desc = c.Certificate_desc,
                        Img_1s = c.Img_1s,
                        Digital = c.Digital,
                        Genre = c.Genre,
                        GenreCode = c.GenreCode,
                        StartDate = c.StartDate,
                        IMDBCode = c.IMDBCode

                    });

                }

                ThreeDMovie = Film.Where(o => o.Code == filmCode).FirstOrDefault().Is3d;



                return ThreeDMovie;



            }
            catch (Exception e)
            {

            }


            return ThreeDMovie;



        }

        //public async Task<List<UrlConcat>> ListofMovies(int CinemaId)
        //{
        //    List<UrlConcat> list = new List<UrlConcat>();
        //    List<UrlContentModel> Film = new List<UrlContentModel>();
        //    UrlContentModel urlCnt = new UrlContentModel();
        //    //for (int i = 1; i <= NumberOfRetries; ++i)
        //    //{
        //    //var urlContent = "http://roemeo.com/xml/pfapache.py?type=MEDIA2&sitekey=3BFXv06X6285GeuG";
        //    var urlContent = repoCinema.Get(o => o.Itbid == CinemaId).ApiUrl.Trim();



        //    //UrlContentModel urlCnt = new UrlContentModel();
        //    UrlContentModel2 urlCnt1 = new UrlContentModel2();
        //    List<UrlContentModel2> Performance = new List<UrlContentModel2>();
        //    try
        //    {
        //        var http = new HttpClient();
        //        var response = await http.GetAsync(urlContent);
        //        var result = await response.Content.ReadAsStringAsync();




        //        XDocument xDoc = XDocument.Load(new StringReader(result));
        //        XDocument xDoc1 = XDocument.Load(new StringReader(result));

        //        var decendats = xDoc.Descendants("Film");

        //        var s = from c in decendats
        //                select new UrlContentModel
        //            {
        //                ShortFilmTitle = string.IsNullOrWhiteSpace(c.Element("ShortFilmTitle").Value) ? "" : c.Element("ShortFilmTitle").Value,
        //                FilmTitle = string.IsNullOrWhiteSpace(c.Element("FilmTitle").Value) ? "" : c.Element("FilmTitle").Value,
        //                ComingSoon = string.IsNullOrWhiteSpace(c.Element("ComingSoon").Value) ? "" : c.Element("ComingSoon").Value,
        //                Code = string.IsNullOrWhiteSpace(c.Element("Code").Value) ? "" : c.Element("Code").Value,
        //                Certificate = string.IsNullOrWhiteSpace(c.Element("Certificate").Value) ? "" : c.Element("Certificate").Value,
        //                Is3d = string.IsNullOrWhiteSpace(c.Element("Is3d").Value) ? "" : c.Element("Is3d").Value,
        //                Img_title = string.IsNullOrWhiteSpace(c.Element("Img_title").Value) ? "" : c.Element("Img_title").Value,
        //                Rentrak = string.IsNullOrWhiteSpace(c.Element("Rentrak").Value) ? "" : c.Element("Rentrak").Value,
        //                Youtube = string.IsNullOrWhiteSpace(c.Element("Youtube").Value) ? "" : c.Element("Youtube").Value,
        //                ReleaseDate = string.IsNullOrWhiteSpace(c.Element("ReleaseDate").Value) ? "" : c.Element("ReleaseDate").Value,
        //                RunningTime = string.IsNullOrWhiteSpace(c.Element("RunningTime").Value) ? "" : c.Element("RunningTime").Value,
        //                Synopsis = string.IsNullOrWhiteSpace(c.Element("Synopsis").Value) ? "" : c.Element("Synopsis").Value,
        //                Certificate_desc = string.IsNullOrWhiteSpace(c.Element("Certificate_desc").Value) ? "" : c.Element("Certificate_desc").Value,
        //                Img_1s = string.IsNullOrWhiteSpace(c.Element("Img_1s").Value) ? "" : c.Element("Img_1s").Value,
        //                Digital = string.IsNullOrWhiteSpace(c.Element("Digital").Value) ? "" : c.Element("Digital").Value,
        //                Genre = string.IsNullOrWhiteSpace(c.Element("Genre").Value) ? "" : c.Element("Genre").Value,
        //                GenreCode = string.IsNullOrWhiteSpace(c.Element("GenreCode").Value) ? "" : c.Element("GenreCode").Value,
        //                StartDate = string.IsNullOrWhiteSpace(c.Element("StartDate").Value) ? "" : c.Element("StartDate").Value,
        //                IMDBCode = string.IsNullOrWhiteSpace(c.Element("IMDBCode").Value) ? "" : c.Element("IMDBCode").Value,

        //            };
        //        foreach (var c in s)
        //        {
        //            Film.Add(new UrlContentModel
        //            {
        //                ShortFilmTitle = c.ShortFilmTitle,
        //                FilmTitle = c.FilmTitle,
        //                ComingSoon = c.ComingSoon,
        //                Code = c.Code,
        //                Certificate = c.Certificate,
        //                Is3d = c.Is3d,
        //                Img_title = c.Img_title,
        //                Rentrak = c.Rentrak,
        //                Youtube = c.Youtube,
        //                ReleaseDate = c.ReleaseDate,
        //                RunningTime = c.RunningTime,
        //                Synopsis = c.Synopsis,
        //                Certificate_desc = c.Certificate_desc,
        //                Img_1s = c.Img_1s,
        //                Digital = c.Digital,
        //                Genre = c.Genre,
        //                GenreCode = c.GenreCode,
        //                StartDate = c.StartDate,
        //                IMDBCode = c.IMDBCode,
        //                CinemaId = CinemaId,

        //            });

        //        }

        //        var decendats1 = xDoc1.Descendants("Performance");

        //        var k = from p in decendats1
        //                select new UrlContentModel2

        //                    {
        //                        PerformDate = p.Element("PerformDate").Value.Trim(),
        //                        Passes = p.Element("Passes").Value.Trim(),
        //                        PerfFlags = p.Element("PerfFlags").Value,
        //                        SoldOutLevel = p.Element("SoldOutLevel").Value,
        //                        PerfCat = p.Element("PerfCat").Value,
        //                        DoorsOpen = p.Element("DoorsOpen").Value,
        //                        AD = p.Element("AD").Value,
        //                        Screen = p.Element("Screen").Value,
        //                        BookingURL = p.Element("BookingURL").Value,
        //                        FilmCode = p.Element("FilmCode").Value,
        //                        SellonInternet = p.Element("SellonInternet").Value,
        //                        TrailerTime = p.Element("TrailerTime").Value,
        //                        WheelchairAccessible = p.Element("WheelchairAccessible").Value,
        //                        ReservedSeating = p.Element("ReservedSeating").Value,
        //                        SalesStopped = p.Element("SalesStopped").Value,
        //                        PerformanceNumberSlot = p.Element("PerformanceNumberSlot").Value,
        //                        InternalBookingURLDesktop = p.Element("InternalBookingURLDesktop").Value,
        //                        Code = p.Element("Code").Value,
        //                        Subs = p.Element("Subs").Value,
        //                        PerfFlagsDescription = p.Element("PerfFlagsDescription").Value,
        //                        ScreenCode = p.Element("ScreenCode").Value,
        //                        StartTime = p.Element("StartTime").Value,
        //                        ManagerWarningLevel = p.Element("ManagerWarningLevel").Value,

        //                    };

        //        foreach (var o in k)
        //        {
        //            Performance.Add(new UrlContentModel2
        //            {
        //                PerformDate = o.PerformDate,
        //                Passes = o.Passes,
        //                PerfFlags = o.PerfFlags,
        //                SoldOutLevel = o.SoldOutLevel,
        //                PerfCat = o.PerfCat,
        //                DoorsOpen = o.DoorsOpen,
        //                AD = o.AD,
        //                Screen = o.Screen,
        //                BookingURL = o.BookingURL,
        //                FilmCode = o.FilmCode,
        //                SellonInternet = o.SellonInternet,
        //                TrailerTime = o.TrailerTime,
        //                WheelchairAccessible = o.WheelchairAccessible,
        //                ReservedSeating = o.ReservedSeating,
        //                SalesStopped = o.SalesStopped,
        //                PerformanceNumberSlot = o.PerformanceNumberSlot,
        //                InternalBookingURLDesktop = o.InternalBookingURLDesktop,
        //                Code = o.Code,
        //                Subs = o.Subs,
        //                InternalBookingURLMobile = o.InternalBookingURLMobile,
        //                PerfFlagsDescription = o.PerfFlagsDescription,
        //                ScreenCode = o.ScreenCode,
        //                StartTime = o.StartTime,
        //                ManagerWarningLevel = o.ManagerWarningLevel,
        //                CinemaId = CinemaId,

        //            });

        //        }




        //        var tt = Film.GroupJoin(
        //                      Performance,
        //                      film => film.Code,
        //                      performance => performance.FilmCode,
        //                      (x, y) => new { Category = x, Products = y })
        //                  .SelectMany(
        //                      xy => xy.Products.DefaultIfEmpty(),
        //                      (x, y) => new { Category = x.Category, Product = y })
        //                  .Select(w => new UrlConcat
        //                  {

        //                      _FilmTitle = w.Category.FilmTitle,
        //                      ComingSoon = w.Category.ComingSoon,
        //                      Is3d = w.Category.Is3d,
        //                      filmCode = string.IsNullOrWhiteSpace(w.Category.Code) ? 0 : Convert.ToInt32(w.Category.Code),
        //                      Img_1s = w.Category.Img_1s,
        //                      StartDate = w.Category.StartDate,
        //                      PerformDate = w.Product.PerformDate,
        //                      SoldOutLevel = w.Product.SoldOutLevel,
        //                      PerfCat = w.Product.PerfCat,
        //                      Screen = w.Product.Screen,
        //                      Synopsis = w.Category.Synopsis,
        //                      SalesStopped = w.Product.SalesStopped,
        //                      CinemaId = w.Category.CinemaId
        //                  }).ToList();

        //        list = tt.GroupBy(x => x._FilmTitle)
        //            .Select(g => g.First()).ToList();


        //    }
        //    catch (Exception e)
        //    {
        //        //// DO BETTER THAN THIS! Catch what you want to handle,
        //        //// not all exceptions worth a retry. Documentation and many
        //        //// tests will help you to narrow a limited subset of
        //        //// exceptions and error codes.

        //        //// Last one, (re)throw exception and exit
        //        //if (i == NumberOfRetries)
        //        //    throw;

        //        //// Many network related errors will recover "automatically"
        //        //// after some time, exact delay is pretty arbitrary and
        //        //// should be determined with some tests. 1 second is pretty
        //        //// "good" for local I/O operations but network issues may
        //        //// need longer delays.
        //        Thread.Sleep(DelayOnRetry);
        //    }

        //    //}
        //    return list;

        //}

        public List<UrlConcat> ListofMovies(int CinemaId)
        {
            List<UrlConcat> list = new List<UrlConcat>();
            List<UrlContentModel> Film = new List<UrlContentModel>();
            UrlContentModel urlCnt = new UrlContentModel();

            UrlContentModel2 urlCnt1 = new UrlContentModel2();
            List<UrlContentModel2> Performance = new List<UrlContentModel2>();
            try
            {
                DateTime dt;
                if (CinemaId == 1)
                {

                
                    var tt = RaiseGetPalmsFilmsPerformance().OrderByDescending(o => o.StartDate);

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();
                    //.OrderByDescending(x => DateTime.TryParse(x.StartDate, out dt) ? String.Format("{0:t}", dt) : x.StartTime).ToList(); //ToList();;


                }
                else if (CinemaId == 2)
                {

                    var tt = RaiseGetPHFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();

                    //list = tt.GroupBy(x => x._FilmTitle)
                    //    .Select(g => g.First()).ToList();

                }
                else if (CinemaId == 3)
                {

                    var tt = RaiseGetMaryLandFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();




                }
                else if (CinemaId == 4)
                {



                    var tt = RaiseGetAbujaFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();






                }
                else if (CinemaId == 5)
                {

                    var tt = RaiseGetWarriFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();



                }
                else if (CinemaId == 6)
                {
                    var tt = RaiseGetOwerriFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();




                }
                else if (CinemaId == 7)
                {


                    var tt = RaiseGetAjahFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();



                }

                else if (CinemaId == 8)
                {

                    var tt = RaiseGetAsabaFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();


                }

                else if (CinemaId == 9)
                {

                    var tt = RaiseGetGatewayFilmsPerformance().OrderByDescending(o => o.StartDate); ;

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();



                }
                else
                {

                    var tt = RaiseOtherCinemaFilmsPerform(CinemaId).OrderByDescending(o => o.StartDate);

                    list = tt.GroupBy(x => x._FilmTitle)
                        .Select(g => g.First()).ToList();

                }


            }
            catch (Exception e)
            {

            }

            //}
            return list;

        }


        public string FormattedAmount(decimal amount)
        {
            return amount.ToString("N2", CultureInfo.InvariantCulture);
        }

        public string GetMovieDescription(int MovieId)
        {
            return repoMovies.GetNonAsync(o => o.Itbid == MovieId).MovieDescription;
        }
        public byte[] GetMovieImage(int MovieId)
        {
            return repoMovies.GetNonAsync(o => o.Itbid == MovieId).MovieImage;
        }

        public string GetStartDate(int MovieId)
        {
            var startDate = repoMovies.GetNonAsync(o => o.Itbid == MovieId).StartDate;

            return string.Format("{0:ddd, MMM d, yyyy}", startDate);
        }
        public string GetMovieLocation(int MovieId)
        {
            return repoMovies.GetNonAsync(o => o.Itbid == MovieId).MovieLocation;
        }

        public string GetMovieTime(int MovieId)
        {
            return repoMovies.GetNonAsync(o => o.Itbid == MovieId).MovieTime;
        }
        public string FormatDateCurrProcessing(DateTime? dt)
        {
            if (dt != null)
            {
                return string.Format("{0:yyyy-MM-dd }", dt);
            }
            return null;
        }

        public PriceObject GetTicketAmount(string MovieCategory, int NoOfPersons, int CinemaCompanyID, string Date, string filmCode, string CouponValue, string MovieTime, string ispromoDay)
        {
            PriceObject rtv = new PriceObject();
            // DateTime dt = new DateTime();
            try
            {
                //TimeSpan mtime;                
                string cuttOfftime = System.Configuration.ConfigurationManager.AppSettings["CutOfftime"];
                int time = Convert.ToInt32(cuttOfftime);
                TimeSpan start = new TimeSpan(time, 0, 0); //10 o'clock

                using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                    
                        cmd.Connection = con;
                        cmd.CommandText = "IspFetchMoviePrice";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter mCat = new SqlParameter("MovieCategory", SqlDbType.VarChar) { Value = MovieCategory };
                        SqlParameter NoPersons = new SqlParameter("NoOfPersons", SqlDbType.Int) { Value = NoOfPersons };
                        SqlParameter CinemaID = new SqlParameter("CinemaCompanyID", SqlDbType.Int) { Value = CinemaCompanyID };
                        SqlParameter date = new SqlParameter("Date", SqlDbType.Date) { Value = FormatDateCurrProcessing(Convert.ToDateTime(Date)) };
                        SqlParameter code = new SqlParameter("filmCode", SqlDbType.VarChar) { Value = filmCode };
                        SqlParameter coup = new SqlParameter("CouponValue", SqlDbType.VarChar) { Value = CouponValue };
                        SqlParameter cutOffTime = new SqlParameter("CutOffTime", SqlDbType.Time) { Value = start };

                        TimeSpan _time = TimeSpan.Parse(MovieTime.Replace(" AM", "").Replace(" PM", ""));
                        DateTime d = DateTime.Parse(MovieTime);
                        SqlParameter MTime = new SqlParameter("psMovieTime", SqlDbType.Time) { Value = d.ToString("HH:mm") };
                        SqlParameter promoDay = new SqlParameter("ispromoDay", SqlDbType.VarChar) { Value = ispromoDay };

                        cmd.Parameters.Add(mCat);
                        cmd.Parameters.Add(NoPersons);
                        cmd.Parameters.Add(CinemaID);
                        cmd.Parameters.Add(date);
                        cmd.Parameters.Add(code);
                        cmd.Parameters.Add(coup);
                        cmd.Parameters.Add(cutOffTime);
                        cmd.Parameters.Add(MTime);
                        cmd.Parameters.Add(promoDay);

                        var dr = cmd.ExecuteReader();

                        var values = dr;

                        while (dr.Read())
                        {
                            //var rtv = ValidateCoupon(CouponValue);
                            if (!string.IsNullOrEmpty(CouponValue))
                            {
                                var setUp = repoCouponSetUp.GetNonAsync(null);
                                if (setUp.CouponType == "P")
                                {
                                    rtv.OrigAmount = FormattedAmount(Convert.ToDecimal(dr[0].ToString()));
                                    rtv.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    rtv.Amount = FormattedAmount((Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(dr[0].ToString()));
                                    rtv.amtCharge = FormattedAmount((Convert.ToDecimal(dr[0].ToString()) / 100) * Convert.ToDecimal(1.4));
                                    return rtv;
                                }
                                else
                                {
                                    rtv.OrigAmount = FormattedAmount(Convert.ToDecimal(dr[0].ToString()));
                                    rtv.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    rtv.Amount = FormattedAmount(Convert.ToDecimal(dr[0].ToString()) - Convert.ToDecimal(CouponValue));
                                    rtv.amtCharge = FormattedAmount((Convert.ToDecimal(dr[0].ToString()) / 100) * Convert.ToDecimal(1.4));
                                    return rtv;


                                }

                            }
                            else
                            {

                                rtv.Amount = FormattedAmount(Convert.ToDecimal(dr[0].ToString()));
                                rtv.amtCharge = FormattedAmount((Convert.ToDecimal(dr[0].ToString()) / 100) * Convert.ToDecimal(1.4));
                                return rtv;
                            }



                        }

                        // Close the reader and the connection
                        dr.Close();



                        con.Close();

                    }
                }
                return null;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        //public string GetTicketAmount(string MovieCategory, int NoOfPersons, int CinemaCompanyID, string Date, string filmCode, string CouponValue)
        //{
        //    string IsBlockBuster = oDapperCalls.GetBlockBusterStatus(filmCode, CinemaCompanyID);

        //    if (IsBlockBuster == "Y")
        //    {

        //        DateTime dt;
        //        string DayOftheWeek = string.Empty;
        //        if (DateTime.TryParse(Date, out dt))
        //        {

        //            DayOftheWeek = dt.DayOfWeek.ToString();

        //        }

        //        string IsRegularMovie = "N";
        //        string IsVip = "N";
        //        string Is3D = "N";
        //        string IsCombo = "N";
        //        if (MovieCategory == "1")
        //        {

        //            IsRegularMovie = "Y";
        //            IsVip = "N";
        //            Is3D = "N";
        //            IsCombo = "N";
        //        }
        //        else if (MovieCategory == "2")
        //        {
        //            IsRegularMovie = "N";
        //            IsVip = "Y";
        //            Is3D = "N";
        //            IsCombo = "N";
        //        }
        //        else if (MovieCategory == "4")
        //        {
        //            IsRegularMovie = "N";
        //            IsVip = "N";
        //            Is3D = "N";
        //            IsCombo = "Y";
        //        }
        //        else
        //        {
        //            Is3D = "Y";

        //        }

        //        if (oDapperCalls.CheckWeekend(dt))
        //        {
        //            string cuttOfftime = System.Configuration.ConfigurationManager.AppSettings["CutOfftime"];
        //            int time = Convert.ToInt32(cuttOfftime);
        //            TimeSpan start = new TimeSpan(time, 0, 0); //10 o'clock

        //            TimeSpan now = DateTime.Now.TimeOfDay;



        //            var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsVIP2D == IsVip && o.Is3D == Is3D && o.IsCombo == IsCombo && o.DayofWeek == DayOftheWeek);
        //                if (ticketId != null)
        //                {
        //                    if (!string.IsNullOrEmpty(CouponValue))
        //                    {
        //                        Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.BlockBlusterUnitPrice);
        //                        return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                    }
        //                    else
        //                    {

        //                        return FormattedAmount(Convert.ToDecimal(ticketId.BlockBlusterUnitPrice * NoOfPersons));
        //                    }
        //                }


        //        }
        //        else
        //        {
        //            string cuttOfftime = System.Configuration.ConfigurationManager.AppSettings["CutOfftime"];
        //            int time = Convert.ToInt32(cuttOfftime);
        //            TimeSpan start = new TimeSpan(time, 0, 0); //10 o'clock



        //            if(dt.Date ==DateTime.Now.Date)
        //            {
        //                TimeSpan now = DateTime.Now.Date.TimeOfDay;

        //                if (now < start)
        //                {
        //                    var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsVIP2D == IsVip && o.Is3D == Is3D && o.IsCombo == IsCombo && o.DayofWeek == DayOftheWeek);
        //                    if (ticketId != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(CouponValue))
        //                        {
        //                            Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.BlockBlusterUnitPrice);
        //                            return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                        }
        //                        else 
        //                        {
        //                            return FormattedAmount(Convert.ToDecimal(ticketId.BlockBlusterUnitPrice * NoOfPersons));

        //                        }

        //                    }

        //                }
        //                else
        //                {

        //                    var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsVIP2D == IsVip && o.IsCombo == IsCombo && o.Is3D == Is3D && o.DayofWeek == DayOftheWeek);
        //                    if (ticketId != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(CouponValue))
        //                        {
        //                            Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.BlockBlusterUnitPrice);
        //                            return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                        }
        //                        else
        //                        {
        //                            return FormattedAmount(Convert.ToDecimal(ticketId.BlockBlusterUnitPrice * NoOfPersons));
        //                        }
        //                    }

        //                }

        //            }
        //            else
        //            {

        //                var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsVIP2D == IsVip && o.IsCombo == IsCombo && o.Is3D == Is3D && o.DayofWeek == DayOftheWeek);
        //                if (ticketId != null)
        //                {
        //                    if (!string.IsNullOrEmpty(CouponValue))
        //                    {
        //                        Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.BlockBlusterUnitPrice);
        //                        return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                    }
        //                    else 
        //                    {

        //                        return FormattedAmount(Convert.ToDecimal(ticketId.BlockBlusterUnitPrice * NoOfPersons));
        //                    }

        //                }

        //            }



        //        }

        //    }
        //    else 
        //    {

        //        DateTime dt;
        //        string DayOftheWeek = string.Empty;
        //        if (DateTime.TryParse(Date, out dt))
        //        {

        //            DayOftheWeek = dt.DayOfWeek.ToString();

        //        }

        //        string IsRegularMovie = "N";
        //        string IsVip = "N";
        //        string Is3D = "N";
        //        string IsCombo = "N";
        //        if (MovieCategory == "1")
        //        {

        //            IsRegularMovie = "Y";
        //            IsVip = "N";
        //            Is3D = "N";
        //            IsCombo = "N";
        //        }
        //        else if (MovieCategory == "2")
        //        {
        //            IsRegularMovie = "N";
        //            IsVip = "Y";
        //            Is3D = "N";
        //            IsCombo = "N";
        //        }
        //        else if (MovieCategory == "4")
        //        {
        //            IsRegularMovie = "N";
        //            IsVip = "N";
        //            Is3D = "N";
        //            IsCombo = "Y";
        //        }
        //        else
        //        {
        //            Is3D = "Y";

        //        }

        //        if (oDapperCalls.CheckWeekend(dt))
        //        {

        //            var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsVIP2D == IsVip && o.IsCombo == IsCombo && o.Is3D == Is3D && o.DayofWeek == DayOftheWeek);
        //            if (ticketId != null)
        //            {
        //                if (!string.IsNullOrEmpty(CouponValue))
        //                {
        //                    Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.UnitPrice);
        //                    return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                }
        //                else
        //                {
        //                    return FormattedAmount(Convert.ToDecimal(ticketId.UnitPrice * NoOfPersons));
        //                }
        //            }


        //        }
        //        else 
        //        {
        //            string cuttOfftime = System.Configuration.ConfigurationManager.AppSettings["CutOfftime"];
        //            int time = Convert.ToInt32(cuttOfftime);
        //            TimeSpan start = new TimeSpan(time, 0, 0); //10 o'clock



        //            if (dt.Date == DateTime.Now.Date)
        //            {
        //                TimeSpan now = DateTime.Now.TimeOfDay;

        //                if (now < start)
        //                {
        //                    var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsCombo == IsCombo && o.IsVIP2D == IsVip && o.Is3D == Is3D && o.DayofWeek == DayOftheWeek);
        //                    if (ticketId != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(CouponValue))
        //                        {
        //                            Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.UnitPrice);
        //                            return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                        }
        //                        else 
        //                        {
        //                            return FormattedAmount(Convert.ToDecimal(ticketId.UnitPrice * NoOfPersons));
        //                        }

        //                    }

        //                }
        //                else
        //                {

        //                    var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsCombo == IsCombo && o.IsVIP2D == IsVip && o.Is3D == Is3D && o.DayofWeek == DayOftheWeek);
        //                    if (ticketId != null)
        //                    {
        //                        if (!string.IsNullOrEmpty(CouponValue))
        //                        {
        //                            Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.CutOffTimeUnitPrice);
        //                            return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                        }
        //                        else 
        //                        {
        //                            return FormattedAmount(Convert.ToDecimal(ticketId.CutOffTimeUnitPrice * NoOfPersons));

        //                        }

        //                    }

        //                }

        //            }
        //            else
        //            {

        //                var ticketId = repoCinemaPricing.Get(o => o.CinemaCompanyID == CinemaCompanyID && o.IsRegular2D == IsRegularMovie && o.IsCombo == IsCombo && o.IsVIP2D == IsVip && o.Is3D == Is3D && o.DayofWeek == DayOftheWeek);
        //                if (ticketId != null)
        //                {
        //                    if (!string.IsNullOrEmpty(CouponValue))
        //                    {
        //                        Decimal value = (Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.UnitPrice);
        //                        return FormattedAmount(Convert.ToDecimal(value * NoOfPersons));
        //                    }
        //                    else 
        //                    {

        //                        return FormattedAmount(Convert.ToDecimal(ticketId.UnitPrice * NoOfPersons));
        //                    }

        //                }

        //            }


        //        }



        //    }



        //    return null;
        //}

        public int CalculatePayStackAmount(string Amount, int NoOfPerson, string CouponValue)
        {
            string amt = Amount.Replace(",", "");

            decimal amount = 0.0m;
            if (decimal.TryParse(amt, out amount))
            {
              //  var discountedAmount = Convert.ToDecimal(0.5) * amount;
                return (Convert.ToInt32(amount) * 100);
            }
            return 0;

        }


        public void UpdatePayStackReference(string TicketPlanetReference, string payStackReference)
        {

            var ContactDetails = repoCinemaTranLog.GetManyNonAsync(o => o.ReferenceNo == TicketPlanetReference);
            foreach (var item in ContactDetails)
            {
                item.PayStackReference = payStackReference;
                //item.Status = "SUCCESSFULL";
                repoCinemaTranLog.Update(item);
                var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
            }
        }

        public int CalculateFlutterAmount(string Amount, int NoOfPerson, string CouponValue,string MovieDate = null)
        {
            string amt = Amount.Replace(",", "");
            decimal amount = 0.0m;
            DateTime dt = new DateTime();
            if (decimal.TryParse(amt, out amount))
            {
                //if (!string.IsNullOrEmpty(MovieDate) && DateTime.TryParse(MovieDate, out dt)) 
                //{
                //    if (dt.DayOfWeek != DayOfWeek.Wednesday) 
                //    {
                //        var discountedAmount = Convert.ToDecimal(0.5) * amount;
                //        return (Convert.ToInt32(discountedAmount));
                //    }
                    
                //}
                return (Convert.ToInt32(amount));
            }
            return 0;

        }

        public async Task<int> UpdateTransLog(string payStackReference, string TicketPlanetReference)
        {

            DynamicParameters param = new DynamicParameters();

            if (payStackReference != null)
            {

                param.Add("@pspayStackReference", payStackReference);

            }

            if (TicketPlanetReference != null)
            {

                param.Add("@psTkReference", TicketPlanetReference);

            }


            var result = await db.QueryAsync<int>(sql: "UpdateCinemaTranLog",

                param: param, commandType: CommandType.StoredProcedure);

            return 1;


        }


        public async Task<string> UpdateFlutterReference(string TicketPlanetReference, string payStackReference)
        {

            var ContactDetails = repoCinemaTranLog.GetManyNonAsync(o => o.ReferenceNo == TicketPlanetReference);
            if (ContactDetails != null)
            {
                foreach (var item in ContactDetails)
                {
                    await UpdateTransLog(payStackReference, TicketPlanetReference);

                }

            }
            return null;
        }

        public void UpdateFlutterWaveReference(string TicketPlanetReference, string flwReference)
        {

            var ContactDetails = repoCinemaTranLog.GetManyNonAsync(o => o.ReferenceNo == TicketPlanetReference);
            foreach (var item in ContactDetails)
            {
                item.PayStackReference = flwReference;
                item.Status = "SUCCESSFULL";
                repoCinemaTranLog.Update(item);
                var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
            }
        }
        public tk_CinemaTransactionLog GetTicketDetails2(string TranRef, string promoCode)
        {
            var rcd = repoPromoLog.GetNonAsync(o => o.ReferenceNo == promoCode);
            if (rcd != null)
            {
                rcd.ViewType = "Y";
                repoPromoLog.Update(rcd);
                var retV3 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
            }

            var custList = repoCinemaTranLog.GetNonAsync(o => o.ReferenceNo == TranRef);
            return custList;
        }

        public async Task<tk_ClientProfile> GetClientProfileDetails(string code)
        {

            var tk = await repoClientProfileRepository.Get(x => x.ClientCode == code);
            return tk;

        }



        public tk_CinemaTransactionLog GetTicketDetails(string TranRef)
        {

            var custList = repoCinemaTranLog.GetNonAsync(o => o.PayStackReference == TranRef);
            return custList;
        }

        public void SaveSms(string TransRef)
        {
            var custList = repoCinemaTranLog.GetNonAsync(o => o.PayStackReference == TransRef);

            if (custList != null)
            {
                // mails 
                var mails = new tk_Sms();
                mails.DateCreated = DateTime.Now;
                mails.Retry = 0;
                mails.Message = "Dear " + custList.ContactFullname + ", Thank You For Your Ticket Purchase From <a href= 'https://www.ticketplanet.ng'> Ticket Planet</ a >.We Are Happy To Have You On Board!. Buy Movie Tickets Up To 3 Times This Month For FilmHouse Cinema Or IMax And Receive 5% Off Any Event Ticket Purchase Of Your Choice And Stay Tuned For For More Exciting Offers And Discount From Us!. Feel Free To Contact Us On 09070111115 For Any Questions And To Join The Whatsapp Group Where Special Offers And Discounts Are Shared.";
                mails.PhoneNo = custList.ContactPhoneNo;
                mails.IsSent = "N";
                repoSms.Add(mails);
                var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                // Insert Into PromoLog Table
                var sms = new tk_Sms();
                sms.DateCreated = DateTime.Now;
                sms.Retry = 0;
                sms.Message = "Dear " + custList.ContactFullname + ", Your Ticket Planet Ref is: " + custList.ReferenceNo + " this RefNo  is required for Confirmation at the Mall";
                sms.PhoneNo = custList.ContactPhoneNo;
                sms.IsSent = "N";
                repoSms.Add(sms);
                var retV3 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
            }


        }


        public void UpdatePaymentStatus(string payStackReference)
        {

            var ContactDetails = repoCinemaTranLog.GetManyNonAsync(o => o.PayStackReference == payStackReference);
            foreach (var item in ContactDetails)
            {
                item.Status = "SUCCESSFULL";
                repoCinemaTranLog.Update(item);
                var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

            }

            #region Update Coupon
            //var details = repoCinemaTranLog.Get(o => o.PayStackReference == payStackReference);
            //if (details.Coupon != null)
            //{
            //    var coup = repoCoupon.Get(o => o.CouponCode == details.Coupon);
            //    if (coup != null)
            //    {
            //        coup.IsUsed = "Y";
            //        coup.DateUsed = DateTime.Now;
            //        repoCoupon.Update(coup);
            //        var ret = unitOfWork.Commit(1) > 0 ? true : false;


            //        var coupAss = repoCouponAssign.Get(o => o.CouponCode == details.Coupon);
            //        if (coupAss != null)
            //        {

            //            coupAss.IsUsed = "Y";
            //            coupAss.DateUsed = DateTime.Now;
            //            repoCouponAssign.Update(coupAss);
            //            var ret2 = unitOfWork.Commit(1) > 0 ? true : false;


            //        }

            //    }

            //}
            #endregion

        }

        public ReturnValues SaveTicketDetails(TicketRequestModel ctReqest, string Reference)
        {
            var returnValues = new ReturnValues();
            var counter = repotk_BatchCounter.GetAllNonAsync().FirstOrDefault();
            try
            {


                var cinemaTranLog = new tk_CinemaTransactionLog();
                cinemaTranLog.DateCreated = DateTime.UtcNow;
                cinemaTranLog.ContactEmail = ctReqest.email;
                cinemaTranLog.Units = ctReqest.NoOfPersons;
                cinemaTranLog.TransactionDate = DateTime.UtcNow;
                cinemaTranLog.ReferenceNo = Reference;
                cinemaTranLog.Status = "PENDING";
                cinemaTranLog.ContactFullname = ctReqest.Fullname;
                cinemaTranLog.ContactPhoneNo = ctReqest.phoneNo;
                cinemaTranLog.TotalAmount = Convert.ToDecimal(ctReqest.Amount);
                cinemaTranLog.MovieDate = ctReqest.MovieDate;
                cinemaTranLog.MovieTime = ctReqest.MovieTime;
                cinemaTranLog.CinemaCompany = ctReqest.CinemaCompanyID;
                cinemaTranLog.CinemaCompanyLocation = ctReqest.CinemaLocation;
                cinemaTranLog.MovieName = ctReqest.MovieName;
                cinemaTranLog.ViewType = ctReqest.TicketCategory;
                cinemaTranLog.IsEmailSent = "N";
                cinemaTranLog.Retry = 0;
                cinemaTranLog.IsCoupon = ctReqest.IsCoupon;
                if (ctReqest.IsCoupon == "Y")
                {
                    cinemaTranLog.CouponAgentId = ctReqest.CouponAgentId;
                    cinemaTranLog.CouponAssignId = ctReqest.CouponAssignId;
                    cinemaTranLog.CouponID = ctReqest.CouponID;
                    cinemaTranLog.Coupon = ctReqest.Coupon;
                }

                repoCinemaTranLog.Add(cinemaTranLog);
                var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
                if (retV1)
                {
                    //Update batch Counter
                    counter.BatchNo = counter.BatchNo + 1;
                    repotk_BatchCounter.Update(counter);
                    var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                    returnValues.nErrorCode = 0;
                    returnValues.sErrorText = "Success";
                    return returnValues;

                }
                else
                {
                    returnValues.nErrorCode = -1;
                    returnValues.sErrorText = "FailedInsert";
                }
                return returnValues;

            }
            catch (Exception ex)
            {


            }
            return returnValues;

        }

        public List<LocationListObj> FetchCinemaList(string movieName)
        {


            DynamicParameters param = new DynamicParameters();

            string moviePreffix = !string.IsNullOrEmpty(movieName) && movieName.Count() > 2 ? movieName.Substring(0, 2) : movieName;
            moviePreffix = Regex.Replace(moviePreffix, "(?<=')(.*?)'(?=.*')", "$1");
            moviePreffix = moviePreffix.Replace("'", "");
            param.Add("@psMovieName", "%" + moviePreffix + "%");

            var result = db.Query<LocationListObj>(sql: "Proc_CinemaLocations1",
                param: param, commandType: CommandType.StoredProcedure);

            return result.ToList();

        }

      

        public IEnumerable<SelectListItem> ListOfCinemas(string movieName)
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = FetchCinemaList(movieName).AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.CinemaName,
                     Value = p.CinemaId.ToString()

                 });
            return items;
        }


        public MovieCodeObj FetchMovieCode(string movieName, int cinemaID)
        {
            MovieCodeObj o = new MovieCodeObj();

            try
            {


                DynamicParameters param = new DynamicParameters();

                string moviePreffix = !string.IsNullOrEmpty(movieName) ? (movieName.Length > 6 ? movieName.Substring(0, 6) : movieName) : null;

                param.Add("@psMovieName", movieName);
                param.Add("@pnCinemaLocation", cinemaID);
                //param.Add("@filmCode", filmCode);

                var result = db.Query<MovieCodeObj>(sql: "Proc_GetFilmCode",
                    param: param, commandType: CommandType.StoredProcedure);


                o.code = result.FirstOrDefault().code;

                return o;
            }
            catch (Exception)
            {


            }
            return o;
        }

        public async Task<List<MovieListProp>> FetchMovieDays(string movieName, int CinemaId)
        {
            List<MovieListProp> list = new List<MovieListProp>();
            MovieListProp lt = new MovieListProp();
            List<UrlContentModel> Film = new List<UrlContentModel>();
            UrlContentModel urlCnt = new UrlContentModel();
            string filmCode = string.Empty;

            var resp = FetchMovieCode(movieName, CinemaId);

            if (resp != null)
            {

                filmCode = resp.code;


                List<UrlContentModel2> Performance = new List<UrlContentModel2>();
                try
                {
                    DateTime dt;

                    if (CinemaId == 1)
                    {



                        var rec = getPalmsDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode
                            });
                        }

                    }
                    else if (CinemaId == 2)
                    {

                        var rec = getPHDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }


                    }
                    else if (CinemaId == 3)
                    {

                        var rec = getMaryLandDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 4)
                    {
                        var rec = getAbujaDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 5)
                    {

                        var rec = getWarriDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 6)
                    {
                        var rec = getOwerriLandDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 7)
                    {

                        var rec = getAjahDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }


                    }

                    else if (CinemaId == 8)
                    {

                        var rec = getAsabaDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }
                    }

                    else if (CinemaId == 9)
                    {


                        var rec = getGatewayDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 10)
                    {


                        var rec = getSurulereDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 11)
                    {


                        var rec = getSamondaDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 12)
                    {


                        var rec = getAdeniranDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 13)
                    {


                        var rec = getDugbeDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 14)
                    {


                        var rec = getOniruDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }

                    else if (CinemaId == 15)
                    {


                        var rec = getBeninDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }

                    else if (CinemaId == 16)
                    {


                        var rec = getKanoDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }
                    else if (CinemaId == 17)
                    {


                        var rec = getAkureDates(filmCode).Select(x => new { PerformDate = DateTime.TryParse(x.PerformDate, out dt) ? String.Format("{0:dddd, MMMM d, yyyy}", dt) : x.PerformDate });
                        foreach (var item in rec)
                        {
                            list.Add(new MovieListProp()
                            {
                                PerformDate = item.PerformDate,
                                FilmCode = filmCode

                            });
                        }

                    }



                    return list;



                }
                catch (Exception e)
                {

                }


                return list;
            }
            return list;

        }

        public class MovieCodeObj
        {

            public string code { get; set; }
        }

        public class LocationListObj
        {

            public int CinemaId { get; set; }
            public string CinemaName { get; set; }

        }

    }
}
