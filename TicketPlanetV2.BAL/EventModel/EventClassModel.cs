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
using System.Net.Mail;

namespace TicketPlanetV2.BAL.EventModel
{
    public class EventClassModel
    {
       
        IDbConnection db = null;
        private readonly ILocationRepository repotk_Location;
        private readonly IEventRepository repoEvent;
        private readonly IClientProfileRepository repotk_ClientProfile;
        private readonly ITransactionLogRepository repotk_TranLog;
        private readonly IEventCustomerRepository repoEventCustomer;
        public GenericViewModel oGenericViewModel;
        private readonly IBatchCounterRpository repotk_BatchCounter;
        private readonly ISmsRepository repoSms;
        private readonly IPassengerRepository repotk_Passenger;
        private readonly IContactInformationRepository repotk_ContactInfo;
        private readonly IEventCategoryRepository repoEventCategory;
        private readonly IFreeEventsRepository repoFreeEvents;
        private readonly ISeatMappingRepository repotk_SeatMapping;
        private readonly IFreeEventCustomersRepository repoFreeEventCustomers;
        private readonly ICouponsRepository repoCoupon;
        private readonly ICouponCodeAssignmentRepository repoCouponAssign;
        private readonly ICouponsSetUpRepository repoCouponSetUp;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDbFactory idbfactory;

        public EventClassModel()
        {
            idbfactory = new DbFactory();
            unitOfWork = new UnitOfWork(idbfactory);
            repotk_Location = new LocationRepository(idbfactory);
            repoEvent = new EventRepository(idbfactory);
            repoEventCustomer = new EventCustomerRepository(idbfactory);
            repotk_ClientProfile = new ClientProfileRepository(idbfactory);
            repoEventCategory = new EventCategoryRepository(idbfactory);
            repotk_ContactInfo = new ContactInformationRepository(idbfactory);
            repotk_Passenger = new PassengerRepository(idbfactory);
            repotk_SeatMapping = new SeatMappingRepository(idbfactory);
            repotk_TranLog = new TransactionLogRepository(idbfactory);
            repoFreeEvents = new FreeEventsRepository(idbfactory);
            oGenericViewModel = new GenericViewModel();
            repoFreeEventCustomers = new FreeEventCustomersRepository(idbfactory);
           
            repoSms = new SmsRepository(idbfactory);
            repotk_BatchCounter = new BatchCounterRpository(idbfactory);
            repoCoupon = new CouponsRepository(idbfactory);
            repoCouponAssign = new CouponCodeAssignmentRepository(idbfactory);
            repoCouponSetUp = new CouponsSetUpRepository(idbfactory);
            db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        }

        public List<tk_FreeEvents> ListOfFreeEvents()
        {

            var d = (from i in repoFreeEvents.GetManyNonAsync(o => o.Status == "Active")

                     select new tk_FreeEvents
                     {
                         EventTitle = i.EventTitle,
                         EventImageName = i.EventImageName,
                         EventDescription = i.EventDescription,
                         EventTime = i.EventTime,
                         EventId = i.EventId,
                         StartDate = i.StartDate,
                         EventLocation = i.EventLocation,
                         EventImage = i.EventImage,
                         Status = i.Status,
                         Itbid = i.Itbid,
                         UserId = i.UserId,

                     }).ToList();


            return d;


        }
        public class FlutterWaveRequestModel
        {
            public string email { get; set; }
            public int amount { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string publicKey { get; set; }
            public string secretKey { get; set; }
            public string Reference { get; set; }
            public string phoneNo { get; set; }
            public string redirectUrl { get; set; }
        }

        public class PayStackRequestModel
        {
            public string email { get; set; }
            public int amount { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }



        }


        public async Task<List<tk_Event>> ListofEvents()
        {

            var d = (from i in await repoEvent.GetMany(o => o.Status == "Active")
                     orderby i.Itbid descending

                     select new tk_Event
                     {
                         EventTitle = i.EventTitle,
                         EventImageName = i.EventImageName,
                         EventDescription = i.EventDescription,
                         EventTime = i.EventTime,
                         EventId = i.EventId,
                         StartDate = i.StartDate,
                         EventLocation = i.EventLocation,
                         TicketCategoryDescription = i.TicketCategoryDescription,
                         Status = i.Status,
                         Itbid = i.Itbid,
                         UserId = i.UserId

                     }).ToList();


            return d;


        }
        public IEnumerable<SelectListItem> ListOfFathersDayTicketCatgory(int categoryId)
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoEventCategory.GetManyNonAsync(o => o.Status == "Active" && o.CategoryId == categoryId).AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.CategoryName,
                     Value = p.Itbid.ToString()

                 });
            return items;
        }

        public IEnumerable<SelectListItem> ListOfTicketCategory(int categoryId)
        {

            IEnumerable<System.Web.Mvc.SelectListItem> items = repoEventCategory.GetManyNonAsync(o => o.Status == "Active" && o.CategoryId == categoryId).AsEnumerable()
                 .Select(p => new System.Web.Mvc.SelectListItem
                 {
                     Text = p.CategoryName,
                     Value = p.Itbid.ToString()

                 });
            return items;
        }

        public async Task<bool> GetCardBin(string cardNumber)
        {
            string url = "https://api.flutterwave.com/v3/card-bins/" + cardNumber;
            string bearer = "Bearer FLWSECK-b4b0dfa202eeb53dadf25da3f54dafaf-X";
            string secretKey = "";

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json;";
            httpWebRequest.Headers.Add("Authorization", bearer);
            
            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(httpResponse.GetResponseStream(), enc);
            Root check = new Root();
            string result = string.Empty;

            result = await responseStream.ReadToEndAsync();

            string vals = result.Replace(@"\", "");
            //string result = response.Content.ReadAsStringAsync().Result;
            check = JsonConvert.DeserializeObject<Root>(result);
            //check = JsonConvert.SerializeObject(result);
            httpResponse.Close();
            if(check != null)
            {
                if(check.data.card_type.ToUpper() == "VERVE")
                {
                    return true;
                }
            }
           

            return false;
        }

        

        public class Data
        {
            public string issuing_country { get; set; }
            public string bin { get; set; }
            public string card_type { get; set; }
            public string issuer_info { get; set; }
        }

        public class Root
        {
            public string status { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
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
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;
                                }
                                else
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + "N" + setUp.CouponValue + " has Successully Been Applied";
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
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;
                                }
                                else
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + "N" + setUp.CouponValue + " has Successully Been Applied";

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
                                    returnValues.CouponValue = FormattedAmount((decimal)setUp.CouponValue);
                                    return returnValues;
                                }
                                else
                                {
                                    returnValues.nErrorCode = 1;
                                    returnValues.CouponAgentId = (int)rtn.AgentID;
                                    returnValues.sErrorText = "Discount of " + "₦" + setUp.CouponValue + " has Successully Been Applied";
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

        public int GetCurrentCounter()
        {

            var counter = repotk_BatchCounter.GetNonAsync(null).BatchNo;
            return (int)counter;
        }

        public tk_ClientProfile GetClientProfileDetails(string code)
        {

            tk_ClientProfile tk = repotk_ClientProfile.GetNonAsync(x => x.ClientCode == code);
            return tk;

        }

        public PriceObject GetTicketAmount(int TicketCategoryID, string TicketCategoryName, int NoOfPersons, string CouponValue, string eventName)
        {
            var rtv = new PriceObject();
            var ticketId = repoEventCategory.GetNonAsync(o => o.Itbid == TicketCategoryID && o.CategoryName == TicketCategoryName);
            if (ticketId != null)
            {
                if (!string.IsNullOrEmpty(CouponValue))
                {
                    var setUp = repoCouponSetUp.GetNonAsync(null);
                    if (setUp.CouponType == "P")
                    {
                        rtv.OrigAmount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                        rtv.Amount = FormattedAmount((Convert.ToDecimal(CouponValue) / 100) * Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                        rtv.nErrorCode = 0;
                        return rtv;
                    }
                    else
                    {
                        rtv.OrigAmount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                        rtv.Amount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons) - Convert.ToDecimal(CouponValue));
                        rtv.nErrorCode = 0;
                        return rtv;


                    }

                }
                else
                {
                    //rtv.OrigAmount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                    //rtv.Amount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons) - Convert.ToDecimal(CouponValue));
                    //rtv.nErrorCode = 0;

                    if (eventName == "Mask and Mysteries")
                    {
                        if (TicketCategoryName == "VIP single")
                        {
                            if (NoOfPersons == 2)
                            {
                                rtv.Amount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons * 0.8M));
                                //rtv.discountPercentage = Convert.ToDecimal(0.8);
                                rtv.nErrorCode = 0;
                                return rtv;
                            }
                            else
                            {
                                rtv.Amount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                                rtv.nErrorCode = 0;
                                return rtv;
                            }
                        }
                        else
                        {
                            rtv.Amount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                            rtv.nErrorCode = 0;
                            return rtv;
                        }

                    }
                    else
                    {
                        rtv.Amount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                        rtv.nErrorCode = 0;
                        return rtv;
                    }

                    //if(ticketId.ExtraCharges != null)
                    //{
                    //    rtv.Amount = FormattedAmount(Convert.ToDecimal((ticketId.Amount + ticketId.ExtraCharges) * NoOfPersons));
                    //    rtv.nErrorCode = 1;
                    //}
                    //else
                    //{
                    //    rtv.Amount = FormattedAmount(Convert.ToDecimal(ticketId.Amount * NoOfPersons));
                    //    rtv.nErrorCode = 1;
                    //}

                    //return rtv;

                }


            }

            return rtv;
        }

        public string GetFathersDayTicketAmount(int TicketCategoryID)
        {
            var ticketId = repoEventCategory.GetNonAsync(o => o.Itbid == TicketCategoryID);
            if (ticketId != null)
            {
                return FormattedAmount(Convert.ToDecimal(ticketId.Amount));
            }

            return null;
        }
     

        public EventObjects GetEventInfo(int? itbid)
        {

            var resp = new EventObjects();

            var rtv = repoFreeEvents.GetNonAsync(o => o.Itbid == itbid);

            if (rtv != null)
            {

                resp.eventDate = rtv.StartDate;
                resp.eventName = rtv.EventTitle;
                resp.eventLocation = rtv.EventLocation;

            }
            return resp;


        }
        public class EventObjects
        {
            public string eventName { get; set; }
            public DateTime? eventDate { get; set; }
            public string eventLocation { get; set; }

        }

        public tk_FreeEventCustomers SaveFreeEventCustomer(GenericViewModel model, int ticketID)
        {
            var cust = new tk_FreeEventCustomers();
            string tranRef = string.Empty;
            try
            {



                if (model != null)
                {


                    cust.DateCreated = DateTime.Now;
                    cust.Email = model.FreeEventCustomerEmail;
                    cust.EventId = ticketID;
                    cust.Fullname = model.FreeEventCustomerName;
                    cust.PhoneNo = model.FreeEventCustomerPhoneNo;
                    cust.NoOfPersons = model.FreeEventNoofPersons;
                    cust.IsEmailSent = "N";
                    cust.UserId = 1;
                    cust.Status = "SUCCESSFULL";
                    cust.Retry = 0;
                    cust.ReferenceNo = RefferenceGenerator.GenerateReference(GetCurrentCounter());
                    repoFreeEventCustomers.Add(cust);
                    var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
                    if (retV1)
                    {
                        var counter = repotk_BatchCounter.GetNonAsync(null);
                        counter.BatchNo = counter.BatchNo + 1;
                        repotk_BatchCounter.Update(counter);
                        var rtv = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                        // Save SMS Table
                        // Insert Into PromoLog Table
                        var sms = new tk_Sms();
                        sms.DateCreated = DateTime.Now;
                        sms.Retry = 0;
                        sms.Message = "Dear " + cust.Fullname + ", Your Ticket Planet Ref is: " + cust.ReferenceNo + " this RefNo  is required for Confirmation at the Event";
                        sms.PhoneNo = cust.PhoneNo;
                        sms.IsSent = "N";
                        repoSms.Add(sms);
                        var retV3 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                        return cust;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
            return cust;

        }


        public string GetFreeEventName(int itbid)
        {
            return repoFreeEvents.GetNonAsync(o => o.Itbid == itbid).EventTitle;
        }

        public string GetFreeEventLocation(int itbid)
        {
            return repoFreeEvents.GetNonAsync(o => o.Itbid == itbid).EventLocation;
        }
        public string GetFreeEventDescription(int itbid)
        {
            return repoFreeEvents.GetNonAsync(o => o.Itbid == itbid).EventDescription;
        }

        public string FormattedAmount(decimal amount)
        {
            return amount.ToString("N2", CultureInfo.InvariantCulture);
        }
        public string GetEventImagePath(int EventId)
        {
            return repoEvent.GetNonAsync(o => o.Itbid == EventId).EventImageName;
        }

        public string GetEventName(int EventId)
        {
            return repoEvent.GetNonAsync(o => o.Itbid == EventId).EventTitle;
        }
        public string GetEventDescription(int EventId)
        {
            return repoEvent.GetNonAsync(o => o.Itbid == EventId).EventDescription;
        }
        public string EventFeeCategory(int EventId)
        {
            return repoEvent.GetNonAsync(o => o.Itbid == EventId).TicketCategoryDescription;
        }

        //public string EventExtraCharges(int itbid)
        //{
        //    var events = repoEvent.Get(o => o.Itbid == itbid).EventId;
        //    decimal? details = 0;
        //    int eventValue = Convert.ToInt16(events);
        //    decimal charges = 70;

        //    if(events != null)
        //    {

        //        details = repoEventCategory.Get(o => o.CategoryId == eventValue);
        //    }

        //    var values =  details.Value != 0 ? details : null;

        //    return values.ToString();
        //}

        public byte[] GetFreeEventImage(int itbid)
        {
            return repoFreeEvents.GetNonAsync(o => o.Itbid == itbid).EventImage;
        }
        //public byte[] GetEventImage(int EventId)
        //{
        //    return repoEvent.Get(o => o.Itbid == EventId).EventImage;
        //}
        public string GetTicketCategoryDescription(int EventId)
        {
            return repoEvent.GetNonAsync(o => o.Itbid == EventId).TicketCategoryDescription;
        }
        public string GetStartDate(int EventId)
        {
            var startDate = repoEvent.GetNonAsync(o => o.Itbid == EventId).StartDate;

            return string.Format("{0:ddd, MMM d, yyyy}", startDate);
        }
        public string GetEventLocation(int EventId)
        {
            return repoEvent.GetNonAsync(o => o.Itbid == EventId).EventLocation;
        }
        public string GetTicketCategory(int itbid)
        {
            if (itbid != 0)
            {

                var details = repoEventCategory.GetNonAsync(o => o.Itbid == itbid).CategoryName;
                return details;
            }
            else
            {
                return null;
            }

        }

        public string GetEventType(int eventID)
        {
            var res = repoEvent.GetNonAsync(o => o.Itbid == eventID);
            if (res != null)
            {
                return res.IsFreeEvent;

            }
            return null;
        }



        public string GetEventTime(int EventId)
        {
            return repoEvent.GetNonAsync(o => o.Itbid == EventId).EventTime;
        }

        public ReturnValues SaveTicketDetails(TicketRequestModel ctReqest, string Reference)
        {
            var returnValues = new ReturnValues();
            var counter = repotk_BatchCounter.GetAllNonAsync().FirstOrDefault();
            try
            {
                var eventCust = new tk_EventCustomers();
                eventCust.DateCreated = DateTime.UtcNow;
                eventCust.Email = ctReqest.email;
                eventCust.EventId = ctReqest.TicketType;
                eventCust.NoOfPersons = ctReqest.NoOfPersons;
                eventCust.PhoneNo = ctReqest.phoneNo;
                eventCust.ReferenceNo = Reference;
                eventCust.TicketCategory = Convert.ToInt32(ctReqest.TicketCategory);
                eventCust.Status = "PENDING";
                eventCust.IsEmailSent = "N";
                eventCust.Retry = 0;
                eventCust.Fullname = ctReqest.Fullname;
                eventCust.UnitPrice = repoEventCategory.GetNonAsync(o => o.Itbid == eventCust.TicketCategory).Amount;
                eventCust.TotalAmount = eventCust.UnitPrice * eventCust.NoOfPersons;
                eventCust.referalId = ctReqest.ReferalId == null ? null : ctReqest.ReferalId.ToString();
                //eventCust.Validated = ctReqest.Validated;
                repoEventCustomer.Add(eventCust);
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
        public tk_EventCustomers SaveFreeTicketDetails(TicketRequestModel ctReqest, string Reference)
        {
            var returnValues = new tk_EventCustomers();
            var counter = repotk_BatchCounter.GetAllNonAsync().FirstOrDefault();
            try
            {

                var eventCust = new tk_EventCustomers();
                eventCust.DateCreated = DateTime.UtcNow;
                eventCust.Email = ctReqest.email;
                eventCust.EventId = ctReqest.TicketType;
                eventCust.NoOfPersons = ctReqest.NoOfPersons;
                eventCust.PhoneNo = ctReqest.phoneNo;
                eventCust.ReferenceNo = Reference;
                eventCust.Status = "SUCCESSFULL";
                eventCust.IsEmailSent = "N";
                eventCust.Retry = 0;
                eventCust.Fullname = ctReqest.Fullname;

                repoEventCustomer.Add(eventCust);
                var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
                if (retV1)
                {
                    //Update batch Counter
                    counter.BatchNo = counter.BatchNo + 1;
                    repotk_BatchCounter.Update(counter);
                    var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                    returnValues.Itbid = eventCust.Itbid;
                    returnValues.Status = eventCust.Status;

                }
                else
                {
                    returnValues.Itbid = 0;
                }
                return returnValues;

            }
            catch (Exception ex)
            {


            }
            return returnValues;

        }

        public ReturnValues SaveFathersTicketDetails(TicketRequestModel ctReqest, string Reference)
        {
            var returnValues = new ReturnValues();
            var counter = repotk_BatchCounter.GetAllNonAsync().FirstOrDefault();
            try
            {

                int id = Convert.ToInt32(ctReqest.TicketCategory);
                var eventCust = new tk_EventCustomers();
                eventCust.DateCreated = DateTime.Now;
                eventCust.Email = ctReqest.email;
                eventCust.EventId = ctReqest.TicketType;
                //eventCust.NoOfPersons = ctReqest.NoOfPersons;
                eventCust.PhoneNo = ctReqest.phoneNo;
                eventCust.ReferenceNo = Reference;
                eventCust.TicketCategory = Convert.ToInt32(ctReqest.TicketCategory);
                eventCust.NoOfPersons = Convert.ToInt32(ctReqest.TicketCategory) == 9 ? 5 : 2;
                eventCust.Status = "PENDING";
                eventCust.IsEmailSent = "N";
                eventCust.Retry = 0;
                eventCust.Fullname = ctReqest.Fullname;
                eventCust.UnitPrice = repoEventCategory.GetNonAsync(o => o.Itbid == id).Amount;
                eventCust.TotalAmount = eventCust.UnitPrice;

                repoEventCustomer.Add(eventCust);
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

        public int CalculateFathersDayPayStackAmount(int TicketCategoryName)
        {

            var payStackAmount = repoEventCategory.GetNonAsync(o => o.Itbid == TicketCategoryName).Amount;
            return (Convert.ToInt32(payStackAmount) * 100);
        }

        public int CalculatePayStackAmount(int category, string TicketCategoryName, int NoOfPerson, string CouponValue)
        {

            var payStackAmount = repoEventCategory.GetNonAsync(o => o.CategoryName == TicketCategoryName && o.Itbid == category).Amount;


            if (!string.IsNullOrEmpty(CouponValue))
            {
                var setUp = repoCouponSetUp.GetNonAsync(null);
                if (setUp.CouponType == "P")
                {

                    int amt = Convert.ToInt32((Convert.ToDecimal(CouponValue) / 100) * (Convert.ToInt32(payStackAmount) * NoOfPerson));
                    return Convert.ToInt32((amt * 100));

                }
                else
                {

                    int amt = Convert.ToInt32((Convert.ToInt32(payStackAmount) * NoOfPerson) - Convert.ToDecimal(CouponValue));
                    return Convert.ToInt32(amt * 100);



                }

            }
            else
            {

                return (Convert.ToInt32(payStackAmount) * NoOfPerson * 100);
            }
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


            var result = await db.QueryAsync<int>(sql: "UpdateTranLog",

                param: param, commandType: CommandType.StoredProcedure);

            return 1;


        }

        public async Task<string> UpdateFlutterReference(string TicketPlanetReference, string payStackReference)
        {

            var ContactDetails = repoEventCustomer.GetManyNonAsync(o => o.ReferenceNo == TicketPlanetReference);
            if (ContactDetails != null)
            {
                foreach (var item in ContactDetails)
                {
                    await UpdateTransLog(payStackReference, TicketPlanetReference);

                }

            }
            return null;
        }

        //public async Task<int> UpdatePayStackTrans(string payStackReference, string Validated)
        //{

        //    repoEventCustomer.Update(item);
        //    var ret = unitOfWork.Commit(1) > 0 ? true : false;
        //    return 1;
        //}
        public async Task<string> UpdatePayStackReference(string TicketPlanetReference, string payStackReference)
        {

            var ContactDetails = repoEventCustomer.GetManyNonAsync(o => o.ReferenceNo == TicketPlanetReference);
            if (ContactDetails != null)
            {
                foreach (var item in ContactDetails)
                {
                    //await UpdateTransLog(payStackReference, TicketPlanetReference);
                    item.PayStackReferenceNo = payStackReference;
                    ///////item.Status = "SUCCESSFULL";
                    item.Validated = "N";
                    repoEventCustomer.Update(item);
                    var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                }
            }
            return null;

        }

        public void UpdateFlutterWaveReference(string TicketPlanetReference, string flwReference)
        {

            var ContactDetails = repoEventCustomer.GetManyNonAsync(o => o.ReferenceNo == TicketPlanetReference);
            foreach (var item in ContactDetails)
            {
                item.PayStackReferenceNo = flwReference;
                item.Validated = "N";
                item.Status = "SUCCESSFULL";
                repoEventCustomer.Update(item);
                var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
            }
        }
        public int CalculateFlutterAmount(int category, string TicketCategoryName, int NoOfPerson, string CouponValue)
        {
            var flutterAmount = repoEventCategory.GetNonAsync(o => o.CategoryName == TicketCategoryName && o.Itbid == category).Amount;

            if (!string.IsNullOrEmpty(CouponValue))
            {
                var setUp = repoCouponSetUp.GetNonAsync(null);
                if (setUp.CouponType == "P")
                {

                    int amt = Convert.ToInt32((Convert.ToDecimal(CouponValue) / 100) * (Convert.ToInt32(flutterAmount) * NoOfPerson));
                    return Convert.ToInt32((amt));

                }
                else
                {

                    int amt = Convert.ToInt32((Convert.ToInt32(flutterAmount) * NoOfPerson) - Convert.ToDecimal(CouponValue));
                    return Convert.ToInt32(amt);
                }

            }
            else
            {

                return (Convert.ToInt32(flutterAmount) * NoOfPerson);
            }

        }

        public int CalculateFlutterTravelsAmount(string amount, int category, string TicketCategoryName, int NoOfPerson, string cardCategory)
        {
            var flutterAmount = repoEventCategory.GetNonAsync(o => o.CategoryName == TicketCategoryName && o.Itbid == category).Amount;

            if (cardCategory == "Y")
            {
                decimal? val = Convert.ToDecimal(amount);

                return (Convert.ToInt32(val) * NoOfPerson);

            }
            else
            {

                return (Convert.ToInt32(flutterAmount) * NoOfPerson);
            }

        }

        public void UpdatePaymentStatus(string payStackReference)
        {
            using (var con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {

                con.Open();

                string command = "Update tk_eventCustomers set Status = 'SUCCESSFULL' where PayStackReferenceNo =" + "'" + payStackReference + "'";
                SqlCommand com2 = new SqlCommand(command, con);
                int rtv = com2.ExecuteNonQuery();
                con.Close();

            }


            var item = repoEventCustomer.GetNonAsync(o => o.PayStackReferenceNo.Trim() == payStackReference.Trim());

            item.Status = "SUCCESSFULL";
            //item.Validated = "N";
            repoEventCustomer.Update(item);
            var ret = unitOfWork.CommitNonAsync(1) > 0 ? true : false;


        }

        public tk_EventCustomers GetCustomerInfo(string TranRef)
        {

            var custList = repoEventCustomer.GetNonAsync(o => o.PayStackReferenceNo == TranRef);
            return custList;
        }

        public tk_Event GetEventInfo(int eventId)
        {

            var custList = repoEvent.GetNonAsync(o => o.Itbid == eventId);
            return custList;
        }

        public ReturnValues SaveIntoSms(string tranRef)
        {
            var rtv = new ReturnValues();
            var tranlog = new tk_TransactionLog();
            var TranDetails = repoEventCustomer.GetNonAsync(o => o.PayStackReferenceNo == tranRef);

            tranlog.ContactFullname = TranDetails.Fullname;
            tranlog.ContactPhoneNo = TranDetails.PhoneNo;
            tranlog.ContactEmail = TranDetails.Email;
            tranlog.PayStackReference = TranDetails.PayStackReferenceNo;
            tranlog.ReferenceNo = TranDetails.ReferenceNo;
            tranlog.TransactionDate = DateTime.UtcNow;
            tranlog.TotalAmount = TranDetails.TotalAmount;
            tranlog.Price = TranDetails.UnitPrice;
            // mails 
            var mails = new tk_Sms();
            mails.DateCreated = DateTime.UtcNow;
            mails.Retry = 0;
            mails.Message = "Dear " + tranlog.ContactFullname + ", Thank You For Your Ticket Purchase From <a href= 'https://www.ticketplanet.ng'> Ticket Planet</ a >.We Are Happy To Have You On Board!. Buy Movie Tickets Up To 3 Times This Month For FilmHouse Cinema Or IMax And Receive 5% Off Any Event Ticket Purchase Of Your Choice And Stay Tuned For For More Exciting Offers And Discount From Us!. Feel Free To Contact Us On 09070111115 For Any Questions And To Join The Whatsapp Group Where Special Offers And Discounts Are Shared.";
            mails.PhoneNo = tranlog.ContactPhoneNo;
            mails.IsSent = "N";
            repoSms.Add(mails);
            var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
            // Insert Into PromoLog Table
            var sms = new tk_Sms();
            sms.DateCreated = DateTime.UtcNow;
            sms.Retry = 0;
            sms.Message = "Dear " + tranlog.ContactFullname + ", Your Ticket Planet Ref is: " + tranlog.ReferenceNo + " this RefNo  is required for Confirmation at the Event";
            sms.PhoneNo = tranlog.ContactPhoneNo;
            sms.IsSent = "N";
            repoSms.Add(sms);
            var retV3 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

            rtv.nErrorCode = 0;

            return rtv;
        }

        public  void SearchAllEventEmails()
        {
            try
            {
                var events = repoEventCustomer.GetAllNonAsync().ToList();
                if (events.Count() > 0 && events != null) 
                {
                    foreach (var p in events)
                    {
                        if (p.IsEmailSent == "N" && p.Status == "SUCCESSFULL" && p.TicketCategory != null && p.Retry < 2) 
                        {
                           var res = SendEvent(p);
                           EmailNotificationMail.SendEmailPlain(oGenericViewModel.tk_EventCustomers.Email, "Payment Receipt - " + oGenericViewModel.tk_EventCustomers.ReferenceNo, res, null, "enwakire@ticketplanet.ng");
                        }

                        p.IsEmailSent = "Y";
                        repoEventCustomer.Update(p);
                        var retV = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
                    }
                
                
                }

            }
            catch (Exception)
            {

                throw;
            }
        }



        public string SendEvent(tk_EventCustomers msg)
        {
            try
            {
                var category = repoEventCategory.GetNonAsync(o => o.Itbid == msg.TicketCategory);
                var eventName = repoEvent.GetNonAsync(o => o.Itbid == msg.EventId);
                if (category != null && eventName != null) 
                {
                    string message = SmartObject.PopulateUserBody(3);
                    message = message.Replace("{{EventName}}", eventName.EventTitle)
                                    .Replace("{{user}}", msg.Fullname)
                                    .Replace("{{TKReference}}", msg.ReferenceNo == "" ? "" : msg.ReferenceNo)
                                    .Replace("{{ReferenceNo}}", msg.PayStackReferenceNo == "" ? "" : msg.PayStackReferenceNo)
                                    .Replace("{{EventDate}}", eventName.StartDate== null?"Unavailable": oGenericViewModel.FormatDate(eventName.StartDate))
                                    .Replace("{{EventCategory}}", category.CategoryName)
                                    .Replace("{{EventVenue}}", eventName.EventLocation)
                                    .Replace("{{EventTime}}", eventName.EventTime)
                                    .Replace("{{ContactFullname}}", msg.Fullname == "" ? "" : msg.Fullname)
                                    .Replace("{{ContactEmail}}", msg.Email == "" ? "" : msg.Email)
                                    .Replace("{{ContactPhoneNo}}", msg.PhoneNo == "" ? "" : msg.PhoneNo)
                                    .Replace("{{Units}}", msg.NoOfPersons.ToString())
                                    .Replace("{{Amount}}", msg.TotalAmount == null ? "No Fee" : FormattedAmount((decimal)msg.TotalAmount));

                    return message;
                }



                return null;
            }
            catch (Exception)
            {

                return null;
            }


        }
        public ReturnValues SaveIntoTransactionLog(string tranRef)
        {

            var rtv = new ReturnValues();
            var tranlog = new tk_TransactionLog();
            var TranDetails = repoEventCustomer.GetNonAsync(o => o.PayStackReferenceNo == tranRef);
            tranlog.ContactFullname = TranDetails.Fullname;
            tranlog.ContactPhoneNo = TranDetails.PhoneNo;
            tranlog.ContactEmail = TranDetails.Email;
            tranlog.PayStackReference = TranDetails.PayStackReferenceNo;
            tranlog.ReferenceNo = TranDetails.ReferenceNo;
            tranlog.TransactionDate = DateTime.Now;
            tranlog.TotalAmount = TranDetails.TotalAmount;
            tranlog.Price = TranDetails.UnitPrice;
            tranlog.Status = "SUCCESSFULL";
            tranlog.DateCreated = DateTime.Now;

            repotk_TranLog.Add(tranlog);
            var retV = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
            if (retV)
            {
                // mails 
                var mails = new tk_Sms();
                mails.DateCreated = DateTime.Now;
                mails.Retry = 0;
                mails.Message = "Dear " + tranlog.ContactFullname + ", Thank You For Your Ticket Purchase From <a href= 'https://www.ticketplanet.ng'> Ticket Planet</ a >.We Are Happy To Have You On Board!. Buy Movie Tickets Up To 3 Times This Month For FilmHouse Cinema Or IMax And Receive 5% Off Any Event Ticket Purchase Of Your Choice And Stay Tuned For For More Exciting Offers And Discount From Us!. Feel Free To Contact Us On 09070111115 For Any Questions And To Join The Whatsapp Group Where Special Offers And Discounts Are Shared.";
                mails.PhoneNo = tranlog.ContactPhoneNo;
                mails.IsSent = "N";
                repoSms.Add(mails);
                var retV1 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;
                // Insert Into PromoLog Table
                var sms = new tk_Sms();
                sms.DateCreated = DateTime.Now;
                sms.Retry = 0;
                sms.Message = "Dear " + tranlog.ContactFullname + ", Your Ticket Planet Ref is: " + tranlog.ReferenceNo + " this RefNo  is required for Confirmation at the Event";
                sms.PhoneNo = tranlog.ContactPhoneNo;
                sms.IsSent = "N";
                repoSms.Add(sms);
                var retV3 = unitOfWork.CommitNonAsync(1) > 0 ? true : false;

                rtv.nErrorCode = 0;
            }
            return rtv;
        }

        public string TestSendMessage(string from, string to)
        {
            string send = "";

            var bodys = "<p style='font-size:12px;'><b>Hello Firstname</b></p>< p > Thank You For Your Ticket Purchase From<a href= 'https://www.ticketplanet.ng'> Ticket Planet</ a >.We Are Happy To Have You On Board!</p><p> Buy Movie Tickets Up To 3 Times This Month For FilmHouse Cinema Or IMax And Receive 5 % Off Any Event Ticket Purchase Of Your Choice And Stay Tuned For For More Exciting Offers And Discount From Us!</p> <p>Feel Free To Contact Us On 09070111115 For Any Questions And To Join The Whatsapp Group Where Special Offers And Discounts Are Shared.</p>";
            MailMessage mm = new MailMessage(from, to);
            mm.Subject = "";
            mm.Body = bodys;
            mm.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential();
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);

            send = "Your message sent";

            return send;
        }
    }
    
    
}
