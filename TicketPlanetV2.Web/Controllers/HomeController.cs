using Hangfire;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TicketPlanetV2.BAL.EventModel;
using TicketPlanetV2.BAL.GenericModel.ViewModel;
using TicketPlanetV2.BAL.MovieModel;
using TicketPlanetV2.BAL.Utilities;
using TicketPlanetV2.DAL.Entity;

namespace TicketPlanetV2.Web.Controllers
{
    public class HomeController : Controller
    {
        private GenericViewModel oGenericViewModel;
        private MoviesModelClass oMoviesModelClass;
        private EventClassModel oEventClassModel;
        public HomeController()
        {
            oGenericViewModel = new GenericViewModel();
            oMoviesModelClass = new MoviesModelClass();
            oEventClassModel = new EventClassModel();
        }

        public async Task<ActionResult> Index()

        {
            oGenericViewModel.EventList = await oEventClassModel.ListofEvents();
            oGenericViewModel.sliderImages =await oMoviesModelClass.GetLoadingImages();
            return View(oGenericViewModel);
        }
        public async Task<ActionResult> Contact()
        {
           
            return View(oGenericViewModel);
        }

        [HttpPost]
        public ActionResult Contact(string name, string emailfrom, string comment)
        {
            try
            {

                BackgroundJob.Enqueue(() => EmailNotificationMail.SendEmailContact(emailfrom, "contact@ticketplanet.ng", "Contact Us from " + name, comment, "enwakire@ticketplanet.ng;info@ticketplanet.ng", "peze@ticketplanet.ng"));
                //pascal.ezeh@ticketplanet.ng
                return Json(new { error = false,  message = "Message sent"}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }

            return Json(new { error = true});
        }

        public ActionResult DubaiTourTicket(int? TicketType)
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
            return View(oGenericViewModel);
        }

        public ActionResult style()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult TermsAndCondition()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> checkCardBin(string cardNumber)
        {
            var check = await oEventClassModel.GetCardBin(cardNumber);

           
            return Json(new { error = false, result = check }, JsonRequestBehavior.AllowGet);
        }

    }
}