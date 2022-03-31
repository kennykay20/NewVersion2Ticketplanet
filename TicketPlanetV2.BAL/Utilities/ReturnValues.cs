using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketPlanetV2.BAL.Utilities
{
    public  class ReturnValues
    {
        public string MerchantCode { get; set; }
        public int? nErrorCode { set; get; }
        public string sErrorText { set; get; }
        public int UserId { set; get; }
        public int? RoleId { set; get; }
        public string LastLoginDate { get; set; }

        public string fullname { set; get; }
        public int? CustNo { set; get; }
        public string TransactionRef { get; set; }
        public string origRef { get; set; }
    }
}
