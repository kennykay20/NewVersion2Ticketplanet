using Hangfire;
using Paystack.Net.SDK.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TicketPlanetV2.BAL.GenericModel.ViewModel;
using TicketPlanetV2.BAL.MovieModel;
using TicketPlanetV2.BAL.Utilities;
using static TicketPlanetV2.BAL.EventModel.EventClassModel;
using static TicketPlanetV2.BAL.MovieModel.ViewModel.MovieViewModel;

namespace TicketPlanetV2.Web.Controllers
{
    public class MoviesController : Controller
    {
        public GenericViewModel oGenericViewModel;
        public MoviesModelClass oMoviesModelClass;
        public FilmHouseModel oFilmHouseModel;
        public MoviesController()
        {
            oGenericViewModel = new GenericViewModel();
            oMoviesModelClass = new MoviesModelClass();
            oFilmHouseModel = new FilmHouseModel();
        }
        // GET: Movies

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult MoviesHub()
        {

            oGenericViewModel.rv = new ReturnValues();

            return View(oGenericViewModel);
        }
        public ActionResult Index()
        {
            #region Genesis 
            oGenericViewModel.drpGenesisCinema = oMoviesModelClass.ListOfCinemas(3);
            oGenericViewModel.MovieList = oMoviesModelClass.ListofMovies(3);
            oGenericViewModel.CinemaID = oGenericViewModel.MovieList.Count > 0 ? oGenericViewModel.MovieList.FirstOrDefault().CinemaId : 0;
            #endregion
            return View(oGenericViewModel);
        }

        public ActionResult GetCinemaLocation(string cinema)
        {
           
            if (!string.IsNullOrEmpty(cinema)) 
            {
                int cmpy = Convert.ToInt32(cinema);
                var res =oMoviesModelClass.GetCinemaLocations(cmpy);
               
                return Json(res, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        public ActionResult GetMoviesViaLocation(string company, string location)
        {

            if (!string.IsNullOrEmpty(company) && !string.IsNullOrEmpty(location))
            {
                oGenericViewModel.cinemaCompany = company;
                int cmpy = Convert.ToInt32(company);
                if (cmpy == 3)
                {
                    oGenericViewModel.ShowtimeList = oMoviesModelClass.ListOfShowtimes(location);
                    return PartialView("_FilmHouse", oGenericViewModel);
                }
                else
                {
                    int lctn = Convert.ToInt32(location);
                    oGenericViewModel.MovieList = oMoviesModelClass.ListofMovies(Convert.ToInt32(location));
                    return PartialView("_FilmHouse", oGenericViewModel);
                }


            }
            return null;
        }


        [HttpPost]
        public JsonResult GetFilmHousePrice(string price, int NoOfPersons)
        {

            var locationList = oMoviesModelClass.GetFilmHousePrice(price, NoOfPersons);

            return Json(locationList);

        }
        [HttpPost]
        public async Task<JsonResult> GetFilmHouseMovieTime(string MovieDay, string siteId, string filmId)
        { 

            string[] res = MovieDay.Split('_');
            var locationList = await oMoviesModelClass.ListOfTime(siteId, res[0], filmId);

            return Json(locationList);

        }


        //Create an order or Adding ticket(s) to and order
        //FilmHouse AddtoOrder for transaction - Ticketing which contains endpoint to order tickets and 
        // complete a booking ready to be collected at a site

        [HttpPost]
        public JsonResult AddToOrder(string MovieCategory, string MovieTime, string NoOfPersons)
        {
            string[] id = MovieCategory.Split('_');
            if (id != null)
            {

                var locationList = oMoviesModelClass.AddTicketToOrder(id[1], MovieTime, int.Parse(NoOfPersons));

                if (locationList.Result == null)
                {
                    var data = 0;
                    return Json(data);
                }
                else
                {
                    return Json(locationList.Result);
                }
            }
            return null;

        }

       

        [Route("PaymentConfirmationFm/Movies")]
        public async Task<ActionResult> PaymentConfirmationFm()
        {
            var tk = await oMoviesModelClass.GetClientProfileDetails("001");
            if (tk == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string secretKey = tk.ClientPayStackSecretKey;
            var paystackTransactionAPI = new PaystackTransaction(secretKey);
            var tranxRef = HttpContext.Request.QueryString["reference"];

            if (tranxRef != null)
            {
                var response = await paystackTransactionAPI.VerifyTransaction(tranxRef);
                if (response.status)
                {
                    oMoviesModelClass.SaveSms(tranxRef);
                    oMoviesModelClass.UpdatePaymentStatus(tranxRef);
                    oGenericViewModel.tk_CinemaTransactionLog = oMoviesModelClass.GetTicketDetails(tranxRef);
                    oGenericViewModel.TransactionRef = tranxRef;
                    var BatchCounter = oMoviesModelClass.GetCurrentCounter();

                    #region FilmHouse Call for Setting Customer Details and Getting Booking Reference
                    if (oGenericViewModel.tk_CinemaTransactionLog != null)
                    {

                        oGenericViewModel.BookingId = await oMoviesModelClass.CompleteTransaction(oGenericViewModel.tk_CinemaTransactionLog.OrderId,
                        oGenericViewModel.tk_CinemaTransactionLog.ContactFullname, oGenericViewModel.tk_CinemaTransactionLog.ContactEmail,
                        oGenericViewModel.tk_CinemaTransactionLog.ContactPhoneNo, oGenericViewModel.tk_CinemaTransactionLog.TotalAmount.ToString());


                        if (oGenericViewModel.BookingId != null)
                        {
                            await oMoviesModelClass.UpdateBookingRef(oGenericViewModel.BookingId, tranxRef);
                            var res = oMoviesModelClass.SendFilmHouseEmail((oGenericViewModel.tk_CinemaTransactionLog));
                            if (res != null) 
                            {
                                BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailPlain(oGenericViewModel.tk_CinemaTransactionLog.ContactEmail, "Payment Receipt - " + oGenericViewModel.BookingId, res, null, "enwakire@ticketplanet.ng"));

                            }

                        }
                    }
                    #endregion

                    return View(oGenericViewModel);
                }
            }

            return View(oGenericViewModel);
        }

        public async Task<ActionResult> FilmHouseTickets(string filmId, string SiteId)
        {
            

            if (string.IsNullOrEmpty(filmId))
            {
                return RedirectToAction("Index", "Home");
            }


            var val = oMoviesModelClass.GetMovieDetails(filmId);

            if (val != null)
            {
                oGenericViewModel.MovieName = val.title;
                oGenericViewModel.MovieSynopsis = val.plot;
                oGenericViewModel.MovieYouTube = val.youtube;
            }
            oGenericViewModel.SiteId = SiteId;
            var splitId = filmId.Split('*');
            string ids = splitId[0].ToString();
            oGenericViewModel.filmId = ids;
            oGenericViewModel.CinemaName = oMoviesModelClass.GetFilmHouseLocation(SiteId);
            oGenericViewModel.drpMovieCategory = oMoviesModelClass.ListOfCategories(ids, SiteId);
            oGenericViewModel.drpMovieDay = await oMoviesModelClass.ListOfMovieDays(SiteId);
            //oGenericViewModel.MovieYouTube = "";
            //oGenericViewModel.MovieYouTube = oMoviesModelClass.GetYouTubeLink(movieName[0], int.Parse(CinemaId));
            //oBusViewModel.drpMovieTime = await oMoviesModelClass.ListOfTime(busViewModel.SiteId);

            return View(oGenericViewModel);
        }


        public async Task<ActionResult> MovieTicket(string filmCode, string CinemaId)
        {
            
            if (filmCode == null || CinemaId == null)
            {
                return RedirectToAction("Index", "Movies");
            }

            string[] movieName = filmCode.Split('*');
            if (movieName == null) 
            {
                return RedirectToAction("Index", "Movies");

            }

            oGenericViewModel.drpMovieCategory = oMoviesModelClass.ListofCinemaCategory(int.Parse(CinemaId), movieName[0]);
            oGenericViewModel.MovieName = oMoviesModelClass.getMovieName(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.Img_Banner = oMoviesModelClass.getMovieBanner(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.drpMovieLocatn = oMoviesModelClass.ListOfCinemas(movieName[1]);
            oGenericViewModel.MovieSynopsis = oMoviesModelClass.GetMovieSynopsis(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.MovieYouTube = oMoviesModelClass.GetYouTubeLink(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.CinemaName = oMoviesModelClass.GetCinemaName(int.Parse(CinemaId));
            oGenericViewModel.drpMovieDay = await oMoviesModelClass.GetMovieDays(movieName[0], int.Parse(CinemaId));
            //oBusViewModel.Synopsis = oMoviesModelClass.GetCinemaSynopsis(filmCode);
            oGenericViewModel.CinemaCompanyID = int.Parse(CinemaId);
            oGenericViewModel.FilmCode = movieName[0];

            return View(oGenericViewModel);
        }

       
        public async Task<ActionResult> MarturionTicket(string filmCode, string CinemaId)
        {
           
            if (filmCode == null || filmCode == null)
            {
                return RedirectToAction("Index", "Movies");
            }
            string[] movieName = filmCode.Split('*');
            if (movieName == null)
            {
                return RedirectToAction("Index", "Movies");

            }

            oGenericViewModel.drpMovieCategory = oMoviesModelClass.ListofCinemaCategory(int.Parse(CinemaId), movieName[0]);
            oGenericViewModel.MovieName = oMoviesModelClass.getMovieName(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.Img_Banner = oMoviesModelClass.getMovieBanner(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.drpMovieLocatn = oMoviesModelClass.ListOfCinemas(movieName[1]);
            oGenericViewModel.MovieSynopsis = oMoviesModelClass.GetMovieSynopsis(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.MovieYouTube = oMoviesModelClass.GetYouTubeLink(movieName[0], int.Parse(CinemaId));
            oGenericViewModel.CinemaName = oMoviesModelClass.GetCinemaName(int.Parse(CinemaId));
            oGenericViewModel.drpMovieDay = await oMoviesModelClass.GetMovieDays(movieName[0], int.Parse(CinemaId));
         
            oGenericViewModel.CinemaCompanyID = int.Parse(CinemaId);
          

            return View(oGenericViewModel);
        }


        [HttpPost]
        public JsonResult VerifyCoupon(string coupon)
        {

            var rtvValues = oMoviesModelClass.ValidateCoupon(coupon);


            return Json(rtvValues);

        }
        [HttpPost]
        public JsonResult SavePromoTransaction(string PromoCode, string Fullname, string phoneNo, string email, string NoOfPersons, string MovieCategory,
                string CinemaCompanyID, string MovieDay, string MovieTime, string MovieName)
        {

            var rtvValues = oMoviesModelClass.SavePromoTransactions(PromoCode, Fullname, phoneNo, email, NoOfPersons, MovieCategory,
                CinemaCompanyID, MovieDay, MovieTime, MovieName);

            return Json(rtvValues);

        }

        [HttpPost]
        public JsonResult VerifyPromoCode(string PromoCode)
        {

            var rtvValues = oMoviesModelClass.ValidatePromoCode(PromoCode);


            return Json(rtvValues);

        }

        [HttpPost]
        public JsonResult GetMovieDays(string filmName, int cinemaLocation)
        {

            var locationList = oMoviesModelClass.FetchMovieDays(filmName, cinemaLocation);

            if (locationList.Result.Count > 0)
            {
                return Json(new { list = locationList.Result, FCode = locationList.Result.FirstOrDefault().FilmCode }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { list = 0 });

            }

        }

        [HttpPost]
        public JsonResult GetMovieAmountViaTime(string MovieCategory, int NoOfPersons, int CinemaCompanyID, string MovieDay, string FilmCode, string CouponValue, string MovieTime)
        {
            var ispromoDay = oMoviesModelClass.GetIspromoDay(FilmCode, CinemaCompanyID);

            var locationList = oMoviesModelClass.GetTicketAmount(MovieCategory, NoOfPersons, CinemaCompanyID, MovieDay, FilmCode, CouponValue, MovieTime, ispromoDay);

            if (locationList != null)
            {
                return Json(locationList);
            }
            return Json(locationList);

        }

        [HttpPost]
        public JsonResult GetMovieAmount(string MovieCategory, int NoOfPersons, int CinemaCompanyID, string MovieDay, string FilmCode, string CouponValue, string MovieTime)
        {

            var ispromoDay = oMoviesModelClass.GetIspromoDay(FilmCode, CinemaCompanyID);

            var locationList = oMoviesModelClass.GetTicketAmount(MovieCategory, NoOfPersons, CinemaCompanyID, MovieDay, FilmCode, CouponValue, MovieTime, ispromoDay);

            if (locationList != null)
            {
                return Json(locationList);
            }
            return Json(locationList);

        }

        [HttpPost]
        public JsonResult GetMovieTime(string MovieDay, int CinemaCompanyID, string FilmCode)
        {

            DateTime dt = new DateTime();
            if (DateTime.TryParse(MovieDay, out dt))
            {

                DateTime today = new DateTime();

                string movDate = String.Format("{0:yyyy-MM-dd}", dt);

                //
                DateTime ft = DateTime.UtcNow;

                //DateTime.TryParse(ft.ToString(), out today);

                string todaysDate = String.Format("{0:yyyy-MM-dd}", ft);

                if (todaysDate == movDate)
                {
                    var location = oMoviesModelClass.GetMovieTimeByToday(todaysDate, CinemaCompanyID, FilmCode);
                    if (location.Result.Count > 0)
                    {
                        return Json(location.Result);
                    }
                    else
                    {
                        return Json(new { location = 0 });
                    }

                }
                else
                {
                    var locationList = oMoviesModelClass.GetMovieTime(movDate, CinemaCompanyID, FilmCode);

                    return Json(locationList.Result);
                }
                // get movieTime by Day

                //return Json(locationList.Result);
            }
            return null;

        }

        public JsonResult CheckMovieTime(string movieTime, string movieDay)
        {

            if (movieTime != "")
            {
                var check = oMoviesModelClass.checkPalmsMovieTimeToday(movieTime, movieDay);
                if (check != null)
                {
                    return Json(check);
                }

            }
            var time = DateTime.UtcNow.ToString("hh:mm:ss");
            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }
     
        public async Task<JsonResult> InitializeMoviePaymentFM(string Fullname, string phoneNo, string email, string NoOfPersons
    , string MovieCategory, string Amount, string MovieDay, string MovieTime, string MovieName,
         string siteId, string orderId, string showtimeId, string MovieCategoryText)
        {

            oGenericViewModel.rv = new ReturnValues();
            string[] id = MovieCategory.Split('_');
            string[] mDay = MovieDay.Split('_');


            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;
            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);
            ct.Amount = Amount;
            //ct.Amount = "2.00";
            ct.MovieDate = MovieDay;
            ct.MovieTime = MovieTime;
            ct.siteId = siteId;
            ct.orderId = orderId;

            ct.cat = MovieCategoryText;
            ct.showtimeId = showtimeId;
            ct.MovieName = MovieName;
            ct.TicketCategory = id[1];
            var BatchCounter = oMoviesModelClass.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReference(BatchCounter);
            var rtn = oMoviesModelClass.SaveTicketDetailsFm(ct, Reference);
            if (rtn.sErrorText == "Success")
            {

                var tk = await oMoviesModelClass.GetClientProfileDetails("001");
                int PayStackAmount = oMoviesModelClass.CalculatePayStackAmount(Amount, Convert.ToInt32(NoOfPersons), "");
                //int PayStackAmount = oMoviesModelClass.CalculatePayStackAmount("5.00", Convert.ToInt32(NoOfPersons), "");
                PayStackRequestModel rt = new PayStackRequestModel();
                rt.amount = PayStackAmount;
                rt.email = email;

                rt.firstName = Fullname;
                rt.lastName = Fullname;


                var paystackTransactionAPI = new PaystackTransaction(tk.ClientPayStackSecretKey);
                var response = await paystackTransactionAPI.InitializeTransaction(rt.email, rt.amount, rt.firstName, rt.lastName, "https://www.ticketplanet.ng/Movies/PaymentConfirmationFM", Reference);
                //Note that callback url is optional
                if (response.status == true)
                {
                    oMoviesModelClass.UpdatePayStackReference(Reference, response.data.reference);
                    return Json(new { error = false, result = response }, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> FlutterwaveMoviePaymentFM(string Fullname, string phoneNo, string email, string NoOfPersons
    , string MovieCategory, string Amount, string MovieDay, string MovieTime, string MovieName,
         string siteId, string orderId, string showtimeId, string MovieCategoryText)
        {
            oGenericViewModel.rv = new ReturnValues();
            string[] id = MovieCategory.Split('_');
            string[] mDay = MovieDay.Split('_');


            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;
            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);
            ct.Amount = Amount;
            ct.MovieDate = MovieDay;
            ct.MovieTime = MovieTime;
            ct.siteId = siteId;
            ct.orderId = orderId;

            ct.cat = MovieCategoryText;
            ct.showtimeId = showtimeId;
            ct.MovieName = MovieName;

            ct.TicketCategory = id[1];
            var BatchCounter = oMoviesModelClass.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReferenceFLW(BatchCounter);
            var rtn = oMoviesModelClass.SaveTicketDetailsFm(ct, Reference);
            if (rtn.sErrorText == "Success")
            {

                var tk = await oMoviesModelClass.GetClientProfileDetails("002");

                int flutterwaveAmount = oMoviesModelClass.CalculateFlutterAmount(Amount, Convert.ToInt32(NoOfPersons), "");

                FlutterWaveRequestModel rt = new FlutterWaveRequestModel();
                rt.amount = flutterwaveAmount;
                //rt.amount = 5;
                rt.email = email;
                rt.firstName = Fullname;
                rt.lastName = Fullname;
                rt.publicKey = tk.MobileClientPayStackSecretKey;
                rt.secretKey = tk.ClientPayStackSecretKey;
                //rt.publicKey = testSecretKey;
                //rt.secretKey = testPublicKeys;
                rt.redirectUrl = tk.TicketPlanetEventCallBackUrl;
                rt.Reference = Reference;
                rt.phoneNo = phoneNo;

                return Json(new { result = rt, error = false }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> FlutterwaveMoviePayment(string Fullname, string phoneNo, string email, string NoOfPersons
        , string MovieCategory, string Amount, string comments, int CinemaLocation, int CinemaCompanyID, string MovieDay, string MovieTime, string MovieName,
            string IsCoupon, string Coupon, string CouponAgentId, string CouponAssignId, string CouponID, string nErrorCode, string CouponValue)
        {
            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;
            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);
            ct.Amount = Amount;
            //ct.Amount = "5.00";
            ct.MovieDate = MovieDay;
            ct.MovieTime = MovieTime;
            ct.CinemaCompanyID = CinemaCompanyID;
            ct.CinemaLocation = CinemaLocation;
            ct.MovieName = MovieName;
            ct.TicketCategory = MovieCategory;

            if (!string.IsNullOrEmpty(nErrorCode))
            {
                if (nErrorCode == "1")
                {
                    ct.Coupon = Coupon;
                    ct.CouponAgentId = !string.IsNullOrEmpty(CouponAgentId) ? Convert.ToInt32(CouponAgentId) : 0;
                    ct.CouponAssignId = !string.IsNullOrEmpty(CouponAssignId) ? Convert.ToInt32(CouponAssignId) : 0;
                    ct.CouponID = !string.IsNullOrEmpty(CouponID) ? Convert.ToInt32(CouponID) : 0;
                    ct.IsCoupon = IsCoupon;

                }
            }
            else
            {
                ct.IsCoupon = "N";
            }

            var BatchCounter = oMoviesModelClass.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReferenceFLW(BatchCounter);
            var rtn = oMoviesModelClass.SaveTicketDetails(ct, Reference);

            if (rtn.sErrorText == "Success")
            {
                //string testSecretKey = "FLWSECK_TEST-b252b05cd42022b753045159413ad2ad-X";
                //string testPublicKeys = "FLWPUBK_TEST-1f5f2f2bdd4e5928fdc59fc29296ea91-X";
                var tk = await oMoviesModelClass.GetClientProfileDetails("002");

                int flutterwaveAmount = oMoviesModelClass.CalculateFlutterAmount(Amount, Convert.ToInt32(NoOfPersons), CouponValue, MovieDay);

               

                FlutterWaveRequestModel rt = new FlutterWaveRequestModel();
                //var percent = 0.1 * flutterwaveAmount;
                //var decrease = Convert.ToInt32((flutterwaveAmount - percent));
                //rt.amount = decrease;
                rt.amount = flutterwaveAmount;
                //rt.amount = 5;
                rt.email = email;
                rt.firstName = Fullname;
                rt.lastName = Fullname;
                rt.publicKey = tk.MobileClientPayStackSecretKey;
                rt.secretKey = tk.ClientPayStackSecretKey;
                //rt.publicKey = testSecretKey;
                //rt.secretKey = testPublicKeys;
                rt.redirectUrl = tk.TicketPlanetEventCallBackUrl;
                rt.Reference = Reference;
                rt.phoneNo = phoneNo;

                return Json(new { result = rt, error = false }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> FlutterwaveMoviePaymentPercent(string Fullname, string phoneNo, string email, string NoOfPersons
        , string MovieCategory, string Amount, string comments, int CinemaLocation, int CinemaCompanyID, string MovieDay, string MovieTime, string MovieName,
            string IsCoupon, string Coupon, string CouponAgentId, string CouponAssignId, string CouponID, string nErrorCode, string CouponValue)
        {
            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;
            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);

            ct.Amount = Amount;
            var per = 0.1 * Convert.ToInt32(Amount);
            var dec = Convert.ToInt32((Convert.ToInt32(Amount) - per));
            ct.Amount = dec.ToString();
            //ct.Amount = "5.00";
            ct.MovieDate = MovieDay;
            ct.MovieTime = MovieTime;
            ct.CinemaCompanyID = CinemaCompanyID;
            ct.CinemaLocation = CinemaLocation;
            ct.MovieName = MovieName;
            ct.TicketCategory = MovieCategory;

            if (!string.IsNullOrEmpty(nErrorCode))
            {
                if (nErrorCode == "1")
                {
                    ct.Coupon = Coupon;
                    ct.CouponAgentId = !string.IsNullOrEmpty(CouponAgentId) ? Convert.ToInt32(CouponAgentId) : 0;
                    ct.CouponAssignId = !string.IsNullOrEmpty(CouponAssignId) ? Convert.ToInt32(CouponAssignId) : 0;
                    ct.CouponID = !string.IsNullOrEmpty(CouponID) ? Convert.ToInt32(CouponID) : 0;
                    ct.IsCoupon = IsCoupon;

                }
            }
            else
            {
                ct.IsCoupon = "N";
            }

            var BatchCounter = oMoviesModelClass.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReferenceFLW(BatchCounter);
            var rtn = oMoviesModelClass.SaveTicketDetails(ct, Reference);

            if (rtn.sErrorText == "Success")
            {
                //string testSecretKey = "FLWSECK_TEST-b252b05cd42022b753045159413ad2ad-X";
                //string testPublicKeys = "FLWPUBK_TEST-1f5f2f2bdd4e5928fdc59fc29296ea91-X";
                var tk = await oMoviesModelClass.GetClientProfileDetails("002");

                int flutterwaveAmount = oMoviesModelClass.CalculateFlutterAmount(Amount, Convert.ToInt32(NoOfPersons), CouponValue);

                FlutterWaveRequestModel rt = new FlutterWaveRequestModel();
                var percent = 0.1 * flutterwaveAmount;
                var decrease = Convert.ToInt32((flutterwaveAmount - percent));
                rt.amount = decrease;
                //rt.amount = flutterwaveAmount;
                //rt.amount = 5;
                rt.email = email;
                rt.firstName = Fullname;
                rt.lastName = Fullname;
                rt.publicKey = tk.MobileClientPayStackSecretKey;
                rt.secretKey = tk.ClientPayStackSecretKey;
                //rt.publicKey = testSecretKey;
                //rt.secretKey = testPublicKeys;
                rt.redirectUrl = tk.TicketPlanetEventCallBackUrl;
                rt.Reference = Reference;
                rt.phoneNo = phoneNo;

                return Json(new { result = rt, error = false }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> updateFlutterwave(string reference, string flwRef)
        {
            var tk = await oMoviesModelClass.GetClientProfileDetails("002");

            if (reference != "" && flwRef != "")
            {
                await oMoviesModelClass.UpdateFlutterReference(reference, flwRef);
                oMoviesModelClass.SaveSms(flwRef);
                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> paymentConfirmationFlw(string reference, string fltRef)
        {
            var tk = await oMoviesModelClass.GetClientProfileDetails("002");

            if (tk == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (fltRef != "" && reference != "")
            {

                oGenericViewModel.tk_CinemaTransactionLog = oMoviesModelClass.GetTicketDetails(fltRef);
                oGenericViewModel.TransactionRef = fltRef;
                var BatchCounter = oMoviesModelClass.GetCurrentCounter();

                var res = oMoviesModelClass.SendGenesisMaturionEmail((oGenericViewModel.tk_CinemaTransactionLog));
                if (res != null)
                {
                    BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailPlain(oGenericViewModel.tk_CinemaTransactionLog.ContactEmail, "Payment Receipt - " + oGenericViewModel.tk_CinemaTransactionLog.ReferenceNo, res, null, "enwakire@ticketplanet.ng"));

                }

                return View(oGenericViewModel);
                //return Json(new { error = false, location_url = tk.TicketPlanetEventCallBackUrl }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            //return View(oBusViewModel);
            //return Json(new { error = true}, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> InitializeMoviePayment(string Fullname, string phoneNo, string email, string NoOfPersons
        , string MovieCategory, string Amount, string comments, int CinemaLocation, int CinemaCompanyID, string MovieDay, string MovieTime, string MovieName,
            string IsCoupon, string Coupon, string CouponAgentId, string CouponAssignId, string CouponID, string nErrorCode, string CouponValue)
        {

            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;
            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);

            ct.Amount = Amount;
            ct.MovieDate = MovieDay;
            ct.MovieTime = MovieTime;
            ct.CinemaCompanyID = CinemaCompanyID;
            ct.CinemaLocation = CinemaLocation;
            ct.MovieName = MovieName;
            ct.TicketCategory = MovieCategory;

            if (!string.IsNullOrEmpty(nErrorCode))
            {
                if (nErrorCode == "1")
                {
                    ct.Coupon = Coupon;
                    ct.CouponAgentId = !string.IsNullOrEmpty(CouponAgentId) ? Convert.ToInt32(CouponAgentId) : 0;
                    ct.CouponAssignId = !string.IsNullOrEmpty(CouponAssignId) ? Convert.ToInt32(CouponAssignId) : 0;
                    ct.CouponID = !string.IsNullOrEmpty(CouponID) ? Convert.ToInt32(CouponID) : 0;
                    ct.IsCoupon = IsCoupon;

                }
            }
            else
            {
                ct.IsCoupon = "N";
            }

            var BatchCounter = oMoviesModelClass.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReference(BatchCounter);
            var rtn = oMoviesModelClass.SaveTicketDetails(ct, Reference);
            if (rtn.sErrorText == "Success")
            {

                var tk = await oMoviesModelClass.GetClientProfileDetails("001");
                int PayStackAmount = oMoviesModelClass.CalculatePayStackAmount(Amount, Convert.ToInt32(NoOfPersons), CouponValue);

                PayStackRequestModel rt = new PayStackRequestModel();
                //var percent = 0.1 * PayStackAmount;
                //var decrease = Convert.ToInt32((PayStackAmount - percent));

                rt.amount = PayStackAmount;
                //rt.amount = 50;
                rt.email = email;

                rt.firstName = Fullname;   
                rt.lastName = Fullname;


                //
                var paystackTransactionAPI = new PaystackTransaction(tk.ClientPayStackSecretKey);
                var response = await paystackTransactionAPI.InitializeTransaction(rt.email, rt.amount, rt.firstName, rt.lastName, "https://ticketplanet.ng/Movies/PaymentConfirmation", Reference);
                //Note that callback url is optional
                if (response.status == true)
                {
                    oMoviesModelClass.UpdatePayStackReference(Reference, response.data.reference);
                    return Json(new { error = false, result = response }, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);

        }

        [Route("PaymentConfirmationViaPromo/Movies")]
        public async Task<ActionResult> PaymentConfirmationViaPromo(string tranRef, string origRef)
        {
            var tranxRef = HttpContext.Request.QueryString["origRef"];
            oGenericViewModel.tk_CinemaTransactionLog = oMoviesModelClass.GetTicketDetails2(tranRef, origRef);
            oGenericViewModel.TransactionRef = tranRef;


            return View(oGenericViewModel);
        }

        
        public async Task<ActionResult> PaymentConfirmationFmflw(string reference, string fltRef)
        {
            //var paystackTransactionAPI = new PaystackTransaction(secretKey);
            var tk = await oMoviesModelClass.GetClientProfileDetails("002");

            if (tk == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (fltRef != "" && reference != "")
            {

                //var response = await paystackTransactionAPI.VerifyTransaction(tranxRef);
                //if (response.status)
                //{
                //    oMoviesModelClass.SaveSms(tranxRef);
                //    oMoviesModelClass.UpdatePaymentStatus(tranxRef);
                //    oBusViewModel.tk_CinemaTransactionLog = oMoviesModelClass.GetTicketDetails(tranxRef);
                //    oBusViewModel.TransactionRef = tranxRef;
                //    var BatchCounter = oTicketPlanetModel.GetCurrentCounter();
                //    return View(oBusViewModel);
                //}

                oGenericViewModel.tk_CinemaTransactionLog = oMoviesModelClass.GetTicketDetails(reference);
                oGenericViewModel.TransactionRef = reference;
                var BatchCounter = oMoviesModelClass.GetCurrentCounter();

                // FilmHouse Call for Setting Customer Details and Getting Booking Reference
                if (oGenericViewModel.tk_CinemaTransactionLog != null)
                {

                    oGenericViewModel.BookingId = await oMoviesModelClass.CompleteTransaction(oGenericViewModel.tk_CinemaTransactionLog.OrderId,
                    oGenericViewModel.tk_CinemaTransactionLog.ContactFullname, oGenericViewModel.tk_CinemaTransactionLog.ContactEmail,
                    oGenericViewModel.tk_CinemaTransactionLog.ContactPhoneNo, oGenericViewModel.tk_CinemaTransactionLog.TotalAmount.ToString());


                    if (oGenericViewModel.BookingId != null)
                    {
                        await oMoviesModelClass.UpdateBookingRef(oGenericViewModel.BookingId, reference);
                        var res = oMoviesModelClass.SendFilmHouseEmail((oGenericViewModel.tk_CinemaTransactionLog));
                        if (res != null)
                        {
                            BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailPlain(oGenericViewModel.tk_CinemaTransactionLog.ContactEmail, "Payment Receipt - " + oGenericViewModel.BookingId, res, null, "enwakire@ticketplanet.ng"));

                        }

                    }
                }

                return View(oGenericViewModel);
            }

            return View(oGenericViewModel);
        }

        [Route("PaymentConfirmation/Movies")]
        public async Task<ActionResult> PaymentConfirmation()
        {

            var tk =await oMoviesModelClass.GetClientProfileDetails("001");
            if (tk == null)
            {
                return RedirectToAction("Index", "Home");
            }

            string secretKey = tk.ClientPayStackSecretKey;
            var paystackTransactionAPI = new PaystackTransaction(secretKey);
            var tranxRef = HttpContext.Request.QueryString["reference"];

            if (tranxRef != null)
            {
                var response = await paystackTransactionAPI.VerifyTransaction(tranxRef);
                if (response.status)
                {
                    oMoviesModelClass.SaveSms(tranxRef);
                    oMoviesModelClass.UpdatePaymentStatus(tranxRef);
                    oGenericViewModel.tk_CinemaTransactionLog = oMoviesModelClass.GetTicketDetails(tranxRef);
                    oGenericViewModel.TransactionRef = tranxRef;
                    var BatchCounter = oMoviesModelClass.GetCurrentCounter();

                  
                    var res = oMoviesModelClass.SendGenesisMaturionEmail((oGenericViewModel.tk_CinemaTransactionLog));
                    if (res != null)
                    {
                        BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailPlain(oGenericViewModel.tk_CinemaTransactionLog.ContactEmail, "Payment Receipt - " + oGenericViewModel.tk_CinemaTransactionLog.ReferenceNo, res, null, "enwakire@ticketplanet.ng"));

                    }


                    return View(oGenericViewModel);
                }
            }

            return View(oGenericViewModel);
        }
    }
}