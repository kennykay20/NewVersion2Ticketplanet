using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TicketPlanetV2.DAL.Context;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanetV2.DAL.Implementation
{
    public class DbFactory : Disposable, IDbFactory
    {
        //This class implement the Init() Interface by returning the Initialization of the DbContext called TicketPlanetContext;
        TicketPlanetContext dbContext;

        public TicketPlanetContext Init()
        {
            return dbContext ?? (dbContext = new TicketPlanetContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }


    }
}
