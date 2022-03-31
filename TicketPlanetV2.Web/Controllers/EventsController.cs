using Hangfire;
using Paystack.Net.SDK.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TicketPlanetV2.BAL.EventModel;
using TicketPlanetV2.BAL.GenericModel.ViewModel;
using TicketPlanetV2.BAL.MovieModel;
using TicketPlanetV2.BAL.Utilities;
using TicketPlanetV2.DAL.Entity;
using static TicketPlanetV2.BAL.EventModel.EventClassModel;
using static TicketPlanetV2.BAL.MovieModel.ViewModel.MovieViewModel;

namespace TicketPlanetV2.Web.Controllers
{
    public class EventsController : Controller
    {
        public GenericViewModel oGenericViewModel;
        public MoviesModelClass oMoviesModelClass;
        public FilmHouseModel oFilmHouseModel;
        public EventClassModel oEventClassModel;
        public EventsController()
        {
            oGenericViewModel = new GenericViewModel();
            oMoviesModelClass = new MoviesModelClass();
            oFilmHouseModel = new FilmHouseModel();
            oEventClassModel = new EventClassModel();
        }
        // GET: Events
        public async Task<ActionResult> Index()
        {

            oGenericViewModel.tk_Event = new tk_Event();
            oGenericViewModel.tk_EventCategory = new tk_EventCategory();
            oGenericViewModel.tk_EventCustomers = new tk_EventCustomers();
            oGenericViewModel.EventList =await oEventClassModel.ListofEvents();
            oGenericViewModel.FreeEventList = oEventClassModel.ListOfFreeEvents();
            oGenericViewModel.rv = new ReturnValues();
            return View(oGenericViewModel);
            
        }

        public ActionResult Flights()
        {
            return View();
        }

            public ActionResult EventTicket(int? TicketType, string referalId)
        {
            if (TicketType == 0 || TicketType == null)
            {
                return RedirectToAction("Index", "Home");
            }
          
            oGenericViewModel.tk_Event = new tk_Event();
            oGenericViewModel.tk_EventCategory = new tk_EventCategory();
            oGenericViewModel.tk_EventCustomers = new tk_EventCustomers();
            oGenericViewModel.drpEventCategory = oEventClassModel.ListOfTicketCategory((int)TicketType);
            oGenericViewModel.EventName = oEventClassModel.GetEventName((int)TicketType);
            oGenericViewModel.EventImagePath = oEventClassModel.GetEventImagePath((int)TicketType);
            oGenericViewModel.IsFreeEvent = oEventClassModel.GetEventType((int)TicketType);
            oGenericViewModel.EventTime = oEventClassModel.GetEventTime((int)TicketType);
            oGenericViewModel.EventDate = oEventClassModel.GetStartDate((int)TicketType);
            oGenericViewModel.EventDescription = oEventClassModel.GetEventDescription((int)TicketType);
            oGenericViewModel.FeeCatorgory = oEventClassModel.EventFeeCategory((int)TicketType);
           
            oGenericViewModel.TicketType = (int)TicketType;
            oGenericViewModel.referalId = referalId == null ? "" : referalId;
            oGenericViewModel.rv = new ReturnValues();
            return View(oGenericViewModel);
        }

        [HttpPost]
        public JsonResult GetEventAmount(int TicketCategoryID, string TicketCategoryName, int NoOfPersons, string CouponValue, string EventName)
        {

            var locationList = oEventClassModel.GetTicketAmount(TicketCategoryID, TicketCategoryName, NoOfPersons, CouponValue, EventName);


            return Json(locationList);


        }

        
        [HttpPost]
        public JsonResult VerifyCoupon(string coupon)
        {

            var rtvValues = oEventClassModel.ValidateCoupon(coupon);


            return Json(rtvValues);

        }

        [HttpPost]
        public JsonResult GetFathersDayEventAmount(string TicketCategoryID)
        {

            if (!string.IsNullOrEmpty(TicketCategoryID))
            {
                var locationList = oEventClassModel.GetFathersDayTicketAmount(Convert.ToInt32(TicketCategoryID));
                return Json(locationList);

            }

            return null;

        }

        public async Task<JsonResult> InitializeFathersDayEventPayment(string Fullname, string phoneNo, string email
        , string TicketCategory, string Amount, string TicketType)
        {

            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;
            ct.TicketType = Convert.ToInt32(TicketType);
            ct.email = email;
            ct.TicketCategory = TicketCategory;
            ct.Amount = Amount;
            //ct.Validated = "N";

            var BatchCounter = oEventClassModel.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReference(BatchCounter);
            var rtn = oEventClassModel.SaveFathersTicketDetails(ct, Reference);
            if (rtn.sErrorText == "Success")
            {

                var tk = oEventClassModel.GetClientProfileDetails("001");
                int PayStackAmount = oEventClassModel.CalculateFathersDayPayStackAmount(Convert.ToInt32(TicketCategory));

                PayStackRequestModel rt = new PayStackRequestModel();
                rt.amount = PayStackAmount;
                rt.email = email;

                rt.firstName = Fullname;
                rt.lastName = Fullname;


                var paystackTransactionAPI = new PaystackTransaction(tk.ClientPayStackSecretKey);
                var response = await paystackTransactionAPI.InitializeTransaction(rt.email, rt.amount, rt.firstName, rt.lastName, "https://ticketplanet.ng/Events/FathersDayPaymentConfirmation", Reference,null,null,true);
                //Note that callback url is optional
                if (response.status == true)
                {
                    await oEventClassModel.UpdatePayStackReference(Reference, response.data.reference);
                    return Json(new { error = false, result = response }, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> InitializeFreeEventPayment(string Fullname, string phoneNo, string email, string NoOfPersons,
 int TicketType)
        {

            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;

            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);

            ct.TicketType = TicketType;

            var BatchCounter = oEventClassModel.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReference(BatchCounter);
            var rtn = oEventClassModel.SaveFreeTicketDetails(ct, Reference);
            if (rtn.Itbid != 0)
            {
                var res = oEventClassModel.SendEvent((rtn));
                if (res != null)
                {
                    BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailPlain(rtn.Email, "Payment Receipt - " + rtn.ReferenceNo, res, null, "enwakire@ticketplanet.ng"));

                }

                return Json(new { nErrorCode = 0, sErrorText = "Event Registered Successfully" }, JsonRequestBehavior.AllowGet);

            }

            return Json(new { nErrorCode = -1, sErrorText = "Event Failed to Register" }, JsonRequestBehavior.AllowGet);

        }


        public async Task<JsonResult> InitializeEventPayment(string Fullname, string phoneNo, string email, string NoOfPersons
        , string TicketCategory, string Amount, string comments, int TicketType, string TicketCategoryName, string CouponValue, string ReferalId)
        {

            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;

            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);
            ct.TicketCategory = TicketCategory;
            ct.Amount = Amount;
            //ct.comments = comments;
            ct.TicketType = TicketType;
            //ct.Validated = Validated;
            ct.ReferalId = ReferalId;

            var BatchCounter = oEventClassModel.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReference(BatchCounter);
            var rtn = oEventClassModel.SaveTicketDetails(ct, Reference);
            if (rtn.sErrorText == "Success")
            {

                var tk = oEventClassModel.GetClientProfileDetails("001");
                int PayStackAmount = oEventClassModel.CalculatePayStackAmount(Convert.ToInt32(TicketCategory), TicketCategoryName, Convert.ToInt32(NoOfPersons), CouponValue);

                PayStackRequestModel rt = new PayStackRequestModel();
                rt.amount = PayStackAmount;
                //rt.amount = 5;
                rt.email = email;

                rt.firstName = Fullname;
                rt.lastName = Fullname;


                var paystackTransactionAPI = new PaystackTransaction(tk.ClientPayStackSecretKey);
                var response = await paystackTransactionAPI.InitializeTransaction(rt.email, rt.amount, rt.firstName, rt.lastName, tk.TicketPlanetFlightCallBackUrl, Reference,  null, null, true);
                //Note that callback url is optional
                if (response.status == true)
                {
                    await oEventClassModel.UpdatePayStackReference(Reference, response.data.reference);
                    return Json(new { error = false, result = response }, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);

        }

        public async Task<JsonResult> FlutterwaveEventPayment(string Fullname, string phoneNo, string email, string NoOfPersons
        , string TicketCategory, string Amount, string comments, int TicketType, string TicketCategoryName, string CouponValue, string Validated, string ReferalId)
        {

            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;

            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);
            ct.TicketCategory = TicketCategory;
            ct.Amount = Amount;
            //ct.Amount = "5.00";
            //ct.comments = comments;
            ct.TicketType = TicketType;
            ct.ReferalId = ReferalId == null ? "" : ReferalId;
            //ct.Validated = Validated;

            var BatchCounter = oEventClassModel.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReferenceFLW(BatchCounter);
            var rtn = oEventClassModel.SaveTicketDetails(ct, Reference);
            if (rtn.sErrorText == "Success")
            {

                var tk = oEventClassModel.GetClientProfileDetails("002");
                int flutterwaveAmount = oEventClassModel.CalculateFlutterAmount(Convert.ToInt32(TicketCategory), TicketCategoryName, Convert.ToInt32(NoOfPersons), CouponValue);

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

        public async Task<JsonResult> FlutterwaveDubaiPayment(string Fullname, string phoneNo, string email, string NoOfPersons
        , string TicketCategory, string Amount, string comments, int TicketType, string TicketCategoryName, string cardCategory, string Validated, string ReferalId)
        {

            oGenericViewModel.rv = new ReturnValues();

            TicketRequestModel ct = new TicketRequestModel();
            ct.Fullname = Fullname;
            ct.phoneNo = phoneNo;

            ct.email = email;
            ct.NoOfPersons = Convert.ToInt32(NoOfPersons);
            ct.TicketCategory = TicketCategory;
            ct.Amount = Amount;
            //ct.Amount = "5.00";
            //ct.comments = comments;
            ct.TicketType = TicketType;
            ct.ReferalId = ReferalId == null ? "" : ReferalId;
            //ct.Validated = Validated;

            var BatchCounter = oEventClassModel.GetCurrentCounter();
            var Reference = RefferenceGenerator.GenerateReferenceFLW(BatchCounter);
            var rtn = oEventClassModel.SaveTicketDetails(ct, Reference);
            if (rtn.sErrorText == "Success")
            {

                var tk = oEventClassModel.GetClientProfileDetails("002");
                int flutterwaveAmount = oEventClassModel.CalculateFlutterTravelsAmount(Amount, Convert.ToInt32(TicketCategory), TicketCategoryName, Convert.ToInt32(NoOfPersons), cardCategory);

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

        [HttpPost]
        public async Task<JsonResult> updateFlutterPayment(string reference, string flwRef)
        {
            //var tk = oEventClassModel.GetClientProfileDetails("002");

            if (reference != "" && flwRef != "")
            {
                await oEventClassModel.UpdateFlutterReference(reference, flwRef);
                //Insert Into Sms
                oEventClassModel.SaveIntoTransactionLog(flwRef);
                //oMoviesModelClass.UpdateFlutterWaveReference(reference, fltRef);
                return Json(new { error = false }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> paymentConfirmationFlw(string reference, string flwRef)
        {
            var tk = oEventClassModel.GetClientProfileDetails("002");
            if (tk == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (flwRef == "")
            {
                return RedirectToAction("Index", "Home");
            }

            if (flwRef != "" && reference != "")
            {

                oGenericViewModel.tk_EventCustomers = oEventClassModel.GetCustomerInfo(flwRef);
                oGenericViewModel.TransactionRef = flwRef;
                oGenericViewModel.tk_Event = oEventClassModel.GetEventInfo(Convert.ToInt32(oGenericViewModel.tk_EventCustomers.EventId));
                //var BatchCounter = oEventClassModel.GetCurrentCounter();
                oGenericViewModel.eventCategoryName = oEventClassModel.GetTicketCategory((int)oGenericViewModel.tk_EventCustomers.TicketCategory);
                oGenericViewModel.EventImagePath = oEventClassModel.GetEventImagePath((int)oGenericViewModel.tk_EventCustomers.EventId);


                var res = oEventClassModel.SendEvent((oGenericViewModel.tk_EventCustomers));
                if (res != null)
                {
                    BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailPlain(oGenericViewModel.tk_EventCustomers.Email, "Payment Receipt - " + oGenericViewModel.tk_EventCustomers.ReferenceNo, res, null, "enwakire@ticketplanet.ng"));

                }

                return View(oGenericViewModel);
                //return Json(new { error = false, location_url = tk.TicketPlanetEventCallBackUrl }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }


        }


        [Route("PaymentConfirmation/Events")]
        public async Task<ActionResult> PaymentConfirmation()
        {
            var tk = oEventClassModel.GetClientProfileDetails("001");
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
                    oEventClassModel.UpdatePaymentStatus(tranxRef);

                    oGenericViewModel.tk_EventCustomers = oEventClassModel.GetCustomerInfo(tranxRef);
                    oGenericViewModel.tk_Event = oEventClassModel.GetEventInfo(Convert.ToInt32(oGenericViewModel.tk_EventCustomers.EventId));
                    oGenericViewModel.TransactionRef = tranxRef;
                    oGenericViewModel.eventCategoryName = oEventClassModel.GetTicketCategory((int)oGenericViewModel.tk_EventCustomers.TicketCategory);
                    oGenericViewModel.EventImagePath = oEventClassModel.GetEventImagePath((int)oGenericViewModel.tk_EventCustomers.EventId);
                    //Insert Into Transaction Log
                    oEventClassModel.SaveIntoTransactionLog(tranxRef);

                    var res = oEventClassModel.SendEvent((oGenericViewModel.tk_EventCustomers));
                    if (res != null)
                    {
                        BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailPlain(oGenericViewModel.tk_EventCustomers.Email, "Payment Receipt - " + oGenericViewModel.tk_EventCustomers.ReferenceNo, res, null, "enwakire@ticketplanet.ng"));

                    }

                    return View(oGenericViewModel);
                }
            }

            return View(oGenericViewModel);
        }

        [Route("FathersDayPaymentConfirmation/Events")]
        public async Task<ActionResult> FathersDayPaymentConfirmation()
        {
            var tk = oEventClassModel.GetClientProfileDetails("001");
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
                    oEventClassModel.UpdatePaymentStatus(tranxRef);

                    oGenericViewModel.tk_EventCustomers = oEventClassModel.GetCustomerInfo(tranxRef);
                    oGenericViewModel.tk_Event = oEventClassModel.GetEventInfo(Convert.ToInt32(oGenericViewModel.tk_EventCustomers.TicketCategory));
                    oGenericViewModel.TransactionRef = tranxRef;


                    //Insert Into Transaction Log
                    oEventClassModel.SaveIntoTransactionLog(tranxRef);

                    return View(oGenericViewModel);
                }
            }

            return View(oGenericViewModel);
        }
    }
}