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
    
    public partial class tk_Careers
    {
        public int Id { get; set; }
        public string CareerID { get; set; }
        public string PositionName { get; set; }
        public string PositionDescription { get; set; }
        public Nullable<System.DateTime> ClosingDate { get; set; }
        public string ContactEmail { get; set; }
        public Nullable<int> userId { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> dateCreated { get; set; }
    }
}
