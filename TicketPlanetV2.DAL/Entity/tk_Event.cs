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
    
    public partial class tk_Event
    {
        public int Itbid { get; set; }
        public string EventId { get; set; }
        public string EventTitle { get; set; }
        public Nullable<int> EventCategory { get; set; }
        public string EventImageName { get; set; }
        public string EventDescription { get; set; }
        public Nullable<decimal> TicketAmount { get; set; }
        public Nullable<int> TicketCategory { get; set; }
        public string TicketCategoryDescription { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string EventDuration { get; set; }
        public string EventLocation { get; set; }
        public string EventTime { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string IsFreeEvent { get; set; }
    }
}
