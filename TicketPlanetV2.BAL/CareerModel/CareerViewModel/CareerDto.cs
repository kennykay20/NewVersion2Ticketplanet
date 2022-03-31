using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketPlanetV2.BAL.CareerModel.CareerViewModel
{
   public  class CareerDto
    {
        public class CareerProperties
        {

            public string CareerID { get; set; }
            public string PositionName { get; set; }
            public string PositionDescription { get; set; }
            public string ClosingDate { get; set; }
            public string ContactEmail { get; set; }
            public int userId { get; set; }
            public string Status { get; set; }
            public string dateCreated { get; set; }

        }
    }
}
