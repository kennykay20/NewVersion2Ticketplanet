﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
 

    public class EventCategoryRepository : Repository<tk_EventCategory>, IEventCategoryRepository
    {
        public EventCategoryRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IEventCategoryRepository : IRepository<tk_EventCategory>
    {

    }
}
