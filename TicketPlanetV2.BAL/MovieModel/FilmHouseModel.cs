using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TicketPlanetV2.BAL.MovieModel
{
    public class FilmHouseModel
    {


        public List<CinemaChainSite> GetCinemaChainSites()
        {
            //First get cinema chains
            //Select nigeria (in production)
            //use nigeria chain id and get chain sites


            IEnumerable<System.Web.Mvc.SelectListItem> items = null;
            try
            {
                string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetSiteChain?cinemaChainId=93cb8143-6264-e811-80c3-0004ffb07dad";
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
                var obj = JsonConvert.DeserializeObject<List<CinemaChainSite>>(input);

                return obj;
            }
            catch (Exception ex)
            {
                return new List<CinemaChainSite>();

            }
        }


        public List<Film> GetCinemaChainFilms()
        {
            //First get cinema chains
            //Select nigeria (in production)
            //use nigeria chain id and get chain sites


            try
            {

                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri("http://filmhouse.ticketplanet.ng//");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HttpResponseMessage response = client.GetAsync("api/ReferenceData/GetFilmsCinemaChain?filmCinemaChain=93cb8143-6264-e811-80c3-0004ffb07dad").Result;  // Blocking call!    
                //if (response.IsSuccessStatusCode)
                //{
                //    var ressponse = response.Content.ReadAsAsync<List<Film>>().Result;
                //    return ressponse;
                //}


                string url = "http://filmhouse.ticketplanet.ng/api/ReferenceData/GetFilmsCinemaChain?filmCinemaChain=93cb8143-6264-e811-80c3-0004ffb07dad";
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
                //string input = result1.Replace("\\", string.Empty);
                //  input = result1.Trim('"');
                // var kk = JsonConvert.DeserializeObject<Film>(input);
                return JsonConvert.DeserializeObject<List<Film>>(result1);

            }
            catch (Exception ex)
            {
                return new List<Film>();

            }

            return new List<Film>();
        }


        //GetShowTimes
        public async Task<List<ShowTime>> GetMovieShowTimes(string siteId, string movieId)
        {

            try
            {

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://filmapi.ticketplanet.ng/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("api/ReferenceData/GetShowTimes?siteId=" + siteId).Result;
                if (response.IsSuccessStatusCode)
                {
                    var ressponse = response.Content.ReadAsAsync<List<ShowTime>>().Result;
                    ressponse = ressponse.Where(a => a.filmId == movieId).ToList();
                    return ressponse;
                }
            }
            catch (Exception ex)
            {
                return new List<ShowTime>();
            }
            return new List<ShowTime>();
        }

        public List<ShowTime> GetDummyShowTimes(string siteId)
        {
            return new List<ShowTime>()
            {
                new ShowTime()
                {
                    id = new Guid().ToString("N"),
                    siteId = siteId,
                    startTimeUtc = DateTime.UtcNow,
                    seatsAvailable = 20,

                },
                new ShowTime()
                {
                    id = new Guid().ToString("N"),
                    siteId = siteId,
                    startTimeUtc = DateTime.UtcNow,
                    seatsAvailable = 20,

                },
                new ShowTime()
                {
                    id = new Guid().ToString("N"),
                    siteId = siteId,
                    startTimeUtc = DateTime.UtcNow,
                    seatsAvailable = 20,

                },
                new ShowTime()
                {
                    id = new Guid().ToString("N"),
                    siteId = siteId,
                    startTimeUtc = DateTime.UtcNow,
                    seatsAvailable = 20,

                }
            };
        }

        public List<DateTime> GetDistinctDates(List<ShowTime> Showtimes)
        {
            List<DateTime> Dates = Showtimes.ToList().Select(r => r.startTimeUtc.Date).Distinct().ToList();


            return Dates;
        }




    }

    public class ShowTime
    {
        public string id { get; set; }
        public int screenId { get; set; }
        public string screenName { get; set; }
        public DateTime startTime { get; set; }
        public DateTime startTimeUtc { get; set; }
        public bool isAllocatedSeating { get; set; }
        public string siteId { get; set; }
        public string cinemaChainId { get; set; }
        public IList<object> attributes { get; set; }
        public string filmId { get; set; }
        public string cinemaChainFilmId { get; set; }
        public int seatsAvailable { get; set; }
        public DateTime lastUpdatedUtc { get; set; }
    }

    public class Film
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<string> actors { get; set; }
        public List<string> directors { get; set; }
        public List<string> genres { get; set; }
        public string plot { get; set; }
        public string classification { get; set; }
        public DateTime releaseDate { get; set; }
        public int runtime { get; set; }
        public List<string> writers { get; set; }
        public int productionYear { get; set; }
        public string mxfReleaseId { get; set; }
    }

    public class Address
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
    }

    public class Location
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }



    public class TimeZone
    {
        public string name { get; set; }
        public double currentOffsetFromUtcInHours { get; set; }
    }

    public class CinemaChainSite
    {
        public string id { get; set; }
        public string name { get; set; }
        public Address address { get; set; }
        public Location location { get; set; }
        public string cinemaChainId { get; set; }
        public TimeZone timeZone { get; set; }
    }

    public class Items
    {
        public bool enabled { get; set; }
    }

    public class Refunds
    {
        public bool enabled { get; set; }
        public bool canRefundAfterShowtimeStart { get; set; }
        public bool partialRefundsEnabled { get; set; }
    }

    public class Loyalty
    {
        public bool enabled { get; set; }
    }

    public class SeatSwaps
    {
        public bool enabled { get; set; }
    }

    public class BookingEdits
    {
        public SeatSwaps seatSwaps { get; set; }
    }

    public class Configuration
    {
        public Items items { get; set; }
        public Refunds refunds { get; set; }
        public Loyalty loyalty { get; set; }
        public BookingEdits bookingEdits { get; set; }
    }

    public class CinemaChain
    {
        public string id { get; set; }
        public string name { get; set; }
        public string countryCode { get; set; }
        public Configuration configuration { get; set; }
    }
}
