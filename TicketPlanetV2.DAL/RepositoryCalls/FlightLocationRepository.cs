using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketPlanetV2.DAL.Entity;
using TicketPlanetV2.DAL.Implementation;
using TicketPlanetV2.DAL.Interfaces;

namespace TicketPlanet.DAL.RepositoryCalls
{
    public class FlightLocationRepository : Repository<tk_Flight_Location>, IFlightLocationRepository
    {
        public FlightLocationRepository(IDbFactory dbFactory)
            : base(dbFactory) { }
    }

    public interface IFlightLocationRepository : IRepository<tk_Flight_Location>
    {

    }
}
