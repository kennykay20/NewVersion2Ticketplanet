//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TicketPlanetV2.DAL.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class tk_CinemaTransactionLogOld
    {
        public int Itbid { get; set; }
        public string ReferenceNo { get; set; }
        public string PayStackReference { get; set; }
        public string ContactFullname { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNo { get; set; }
        public string ContactCategory { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string ViewType { get; set; }
        public Nullable<int> Units { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<int> CinemaCompanyLocation { get; set; }
        public Nullable<int> CinemaCompany { get; set; }
        public string MovieName { get; set; }
        public string MovieDate { get; set; }
        public string MovieTime { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string Verified { get; set; }
        public Nullable<int> VerifiedBy { get; set; }
        public Nullable<System.DateTime> DateVerified { get; set; }
    }
}
